using DAL;
using DTOs.Entities;
using DTOs.Enums; // Required for Enums
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BLL.Service
{
    public class FrontDeskService
    {
        private readonly HotelDbContext _context;

        public FrontDeskService(HotelDbContext context)
        {
            _context = context;
        }

        public void ConfirmReservation(int reservationId)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == reservationId);
            if (reservation == null) throw new Exception("Reservation not found");

            // FIX: Use Enum 'Pending'
            if (reservation.Status != ReservationStatus.Pending)
            {
                throw new InvalidOperationException($"Cannot confirm. Status is {reservation.Status}. Only Pending can be confirmed.");
            }

            // FIX: Use Enum 'Confirmed' (matches "Reserved" concept)
            reservation.Status = ReservationStatus.Confirmed;
            _context.SaveChanges();
        }

        public void CancelReservation(int reservationId)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == reservationId);
            if (reservation == null) throw new Exception("Reservation not found");

            // FIX: Use Enum 'Cancelled'
            reservation.Status = ReservationStatus.Cancelled;
            _context.SaveChanges();
        }

        public void CheckInGuest(int reservationId, int staffId)
        {
            var reservation = _context.Reservations
                                      .Include(r => r.Room)
                                      .FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null) throw new Exception("Reservation not found.");

            // FIX: Check Enum 'Confirmed'
            if (reservation.Status != ReservationStatus.Confirmed)
            {
                throw new InvalidOperationException($"Guest must be 'Confirmed' to Check-In. Current: {reservation.Status}");
            }

            if (_context.CheckInOuts.Any(c => c.ReservationId == reservationId))
                throw new InvalidOperationException("Check-In record already exists.");

            var now = DateTime.Now;

            // 1. Update Reservation: Use 'CheckedIn'
            reservation.Status = ReservationStatus.CheckedIn;
            reservation.CheckInDate = now;

            // 2. Update Room: Use 'Occupied'
            if (reservation.Room != null)
            {
                reservation.Room.Status = RoomStatus.Occupied;
            }

            // 3. Create Record
            _context.CheckInOuts.Add(new CheckInOut
            {
                ReservationId = reservationId,
                CheckInBy = staffId,
                CheckInTime = now
            });
            _context.SaveChanges();
        }

        public void CheckOutGuest(int reservationId, int staffId)
        {
            var reservation = _context.Reservations
                                      .Include(r => r.Room)
                                      .FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null) throw new Exception("Reservation not found.");

            // FIX: Check Enum 'CheckedIn'
            if (reservation.Status != ReservationStatus.CheckedIn)
            {
                throw new InvalidOperationException("Guest must be 'CheckedIn' to Check-Out.");
            }

            var record = _context.CheckInOuts.FirstOrDefault(c => c.ReservationId == reservationId && c.CheckOutTime == null);
            if (record == null) throw new Exception("No active check-in record found.");

            var now = DateTime.Now;
            if (record.CheckInTime.HasValue && now <= record.CheckInTime.Value)
                throw new InvalidOperationException("Check-Out time cannot be before Check-In time.");

            // 1. Update Record
            record.CheckOutTime = now;
            record.CheckOutBy = staffId;

            // 2. Calculate Money
            if (record.CheckInTime.HasValue && reservation.Room != null)
            {
                var days = (int)Math.Ceiling((now - record.CheckInTime.Value).TotalDays);
                record.TotalAmount = reservation.Room.Price * (days < 1 ? 1 : days);
            }

            // 3. Update Reservation: Use 'CheckedOut'
            reservation.Status = ReservationStatus.CheckedOut;
            reservation.CheckOutDate = now;

            // 4. Update Room: Use 'Cleaning' (Matches your Enum)
            if (reservation.Room != null)
            {
                reservation.Room.Status = RoomStatus.Cleaning;
            }

            _context.SaveChanges();
        }
    }
}