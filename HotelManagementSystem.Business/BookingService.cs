using HotelManagementSystem.Data.Context;
using HotelManagementSystem.Data.Models; // Lưu ý đúng namespace chứa file Reservation bạn vừa gửi
using HotelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Business
{
    public class BookingService
    {
        private readonly HotelManagementDbContext _context;

        public BookingService(HotelManagementDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ProcessBooking(BookingRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Kiểm tra phòng có tồn tại và đang trống không
                var room = await _context.Rooms.FindAsync(request.RoomId);
                if (room == null || room.Status != "Available") return false;

                // 2. Tạo đối tượng Reservation với đúng tên thuộc tính từ DB
                var reservation = new Reservation
                {
                    RoomId = request.RoomId,
                    CustomerId = request.CustomerId,
                    CheckInDate = request.CheckInDate,  // Đã sửa từ StartDate
                    CheckOutDate = request.CheckOutDate, // Đã sửa từ EndDate
                    Status = "Confirmed",
                    CreatedAt = DateTime.Now // Bổ sung vì DB bạn có cột này
                };

                _context.Reservations.Add(reservation);

                // 3. Cập nhật trạng thái phòng sang "Reserved" hoặc "Occupied"
                room.Status = "Reserved";
                _context.Rooms.Update(room);

                // 4. Lưu thay đổi
                await _context.SaveChangesAsync();

                // Xác nhận hoàn tất Workflow
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                // Nếu có bất kỳ lỗi nào (rớt mạng, sai dữ liệu), hệ thống tự Rollback
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}