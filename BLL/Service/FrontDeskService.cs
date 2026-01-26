using DAL;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class FrontDeskService
    {
        private readonly HotelDbContext _context;

        public FrontDeskService(HotelDbContext context)
        {
            _context = context;
        }

        // --- CHECK IN LOGIC ---
        // Allowed Status Transition: Reserved -> Staying
        public void CheckInGuest(int reservationId, int staffId)
        {
            var reservation = _context.Reservations
                                      .Include(r => r.Room)
                                      .FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
                throw new Exception("Reservation not found.");

            // VALIDATION: Strict Status Check
            // Pending, Cancelled, Left, Staying cannot be checked in again.
            if (reservation.Status != "Reserved")
            {
                throw new InvalidOperationException($"Cannot Check-In. Current Status is '{reservation.Status}'. Guest must be 'Reserved' to Check-In.");
            }

            // VALIDATION: Prevent double check-in records
            if (_context.CheckInOuts.Any(c => c.ReservationId == reservationId))
            {
                throw new InvalidOperationException("This reservation already has a Check-In record.");
            }

            var now = DateTime.Now;

            // 1. Update Reservation
            reservation.Status = "Staying";
            reservation.CheckInDate = now; // Set actual Check-In time

            // 2. Update Room
            if (reservation.Room != null)
            {
                reservation.Room.Status = "Occupied";
            }

            // 3. Create CheckInOut Record
            var checkInRecord = new CheckInOut
            {
                ReservationId = reservationId,
                CheckInBy = staffId,
                CheckInTime = now
            };

            _context.CheckInOuts.Add(checkInRecord);
            _context.SaveChanges();
        }

        // --- CHECK OUT LOGIC ---
        // Allowed Status Transition: Staying -> Left
        public void CheckOutGuest(int reservationId, int staffId)
        {
            var reservation = _context.Reservations
                                      .Include(r => r.Room)
                                      .FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null)
                throw new Exception("Reservation not found.");

            // VALIDATION 1: Strict Status Check
            if (reservation.Status != "Staying")
            {
                throw new InvalidOperationException($"Cannot Check-Out. Current Status is '{reservation.Status}'. Guest must be 'Staying' to Check-Out.");
            }

            // Find the active CheckIn record
            var record = _context.CheckInOuts.FirstOrDefault(c => c.ReservationId == reservationId && c.CheckOutTime == null);
            if (record == null)
            {
                throw new Exception("Critical Error: Guest is marked 'Staying' but no active Check-In record was found.");
            }

            var now = DateTime.Now;

            // VALIDATION 2: Date Check (Check-Out must be AFTER Check-In)
            if (record.CheckInTime.HasValue && now <= record.CheckInTime.Value)
            {
                throw new InvalidOperationException($"Invalid Check-Out Time. Check-Out ({now}) cannot be before or equal to Check-In ({record.CheckInTime}).");
            }

            // 1. Update CheckInOut Record
            record.CheckOutTime = now;
            record.CheckOutBy = staffId;

            // 2. Calculate Money (Duration * Price)
            if (record.CheckInTime.HasValue && reservation.Room != null)
            {
                var timeSpan = now - record.CheckInTime.Value;

                // Logic: Count days. If less than 1 day, charge minimum 1 day.
                int daysStayed = (int)Math.Ceiling(timeSpan.TotalDays);
                if (daysStayed < 1) daysStayed = 1;

                record.TotalAmount = reservation.Room.Price * daysStayed;
            }

            // 3. Update Reservation
            reservation.Status = "Left";
            reservation.CheckOutDate = now;

            // 4. Update Room (Mark as Dirty)
            if (reservation.Room != null)
            {
                reservation.Room.Status = "Dirty";
            }

            _context.SaveChanges();
        }
    }
}
