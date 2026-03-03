using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Data.Context;
using HotelManagementSystem.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HotelManagementSystem.Web.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly HotelManagementDbContext _context;
        public CreateModel(HotelManagementDbContext context) => _context = context;

        [BindProperty]
        public Reservation Reservation { get; set; } = new();

        public Room? SelectedRoom { get; set; }
        public List<Customer> CustomerList { get; set; } = new();
        public List<HotelService> ActiveServices { get; set; } = new();

        [BindProperty]
        public List<int> SelectedServiceIds { get; set; } = new();

        public async Task OnGetAsync(int? roomId)
        {
            // Load danh sách khách hàng để chọn từ Dropdown
            CustomerList = await _context.Customers.ToListAsync();
            ActiveServices = await _context.HotelServices
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();

            if (roomId.HasValue)
            {
                SelectedRoom = await _context.Rooms.FindAsync(roomId.Value);
                if (SelectedRoom != null)
                {
                    Reservation.RoomId = SelectedRoom.Id;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CustomerList = await _context.Customers.ToListAsync();
            ActiveServices = await _context.HotelServices
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();

            // Gán các giá trị mặc định — walk-in trực tiếp nên đặt thẳng vào trạng thái đã nhận phòng
            Reservation.Status = "CheckedIn";
            Reservation.CreatedAt = DateTime.Now;

            _context.Reservations.Add(Reservation);
            await _context.SaveChangesAsync();

            if (SelectedServiceIds.Any())
            {
                var selectedServices = await _context.HotelServices
                    .Where(s => s.IsActive && SelectedServiceIds.Contains(s.Id))
                    .ToListAsync();

                foreach (var service in selectedServices)
                {
                    _context.ReservationServices.Add(new ReservationService
                    {
                        ReservationId = Reservation.Id,
                        HotelServiceId = service.Id,
                        Quantity = 1,
                        UnitPrice = service.Price,
                        AddedAt = DateTime.Now
                    });
                }
            }

            // Đánh dấu phòng là Occupied và tạo bản ghi CheckInOut
            var room = await _context.Rooms.FindAsync(Reservation.RoomId);
            if (room != null)
                room.Status = "Occupied";

            var adminId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : (int?)null;
            _context.CheckInOuts.Add(new CheckInOut
            {
                ReservationId = Reservation.Id,
                CheckInTime = DateTime.Now,
                CheckInBy = adminId,
                TotalAmount = 0
            });

            await _context.SaveChangesAsync();
            return RedirectToPage("/Index");
        }
    }
}
