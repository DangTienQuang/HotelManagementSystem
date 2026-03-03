using HotelManagementSystem.Data.Context;
using HotelManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Business
{
    public class CheckOutService
    {
        private readonly HotelManagementDbContext _context;
        public CheckOutService(HotelManagementDbContext context) => _context = context;

        // Thêm tham số staffId vào đây
        public async Task<bool> ExecuteCheckOut(int reservationId, int staffId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var checkInfo = await _context.CheckInOuts
                    .Include(c => c.Reservation)
                    .ThenInclude(r => r.Room)
                    .FirstOrDefaultAsync(c => c.ReservationId == reservationId && c.CheckOutTime == null);

                if (checkInfo == null)
                {
                    // Fallback: reservation was checked in without a CheckInOut record
                    var res = await _context.Reservations
                        .Include(r => r.Room)
                        .FirstOrDefaultAsync(r => r.Id == reservationId && r.Status == "CheckedIn");

                    if (res == null || res.Room == null) return false;

                    checkInfo = new CheckInOut
                    {
                        ReservationId = reservationId,
                        CheckInTime = res.CheckInDate,
                        TotalAmount = 0
                    };
                    checkInfo.Reservation = res;
                    _context.CheckInOuts.Add(checkInfo);
                }

                checkInfo.CheckOutTime = DateTime.Now;
                checkInfo.CheckOutBy = staffId;

                var stayDuration = (checkInfo.CheckOutTime.Value - checkInfo.CheckInTime!.Value).Days;
                if (stayDuration <= 0) stayDuration = 1;

                var roomAmount = stayDuration * checkInfo.Reservation.Room.Price;
                var serviceAmount = await _context.ReservationServices
                    .Where(s => s.ReservationId == reservationId)
                    .SumAsync(s => s.Quantity * s.UnitPrice);

                checkInfo.TotalAmount = roomAmount + serviceAmount;

                checkInfo.Reservation.Status = "Completed";
                checkInfo.Reservation.Room.Status = "Available";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}