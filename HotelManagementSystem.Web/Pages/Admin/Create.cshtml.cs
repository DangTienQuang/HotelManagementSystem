using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Data.Context;
using HotelManagementSystem.Data.Models;

namespace HotelManagementSystem.Web.Pages.Admin
{
    public class CreateModel : PageModel
    {
        private readonly HotelManagementDbContext _context;
        public CreateModel(HotelManagementDbContext context) => _context = context;

        [BindProperty]
        public Reservation Reservation { get; set; } = new();

        public Room? SelectedRoom { get; set; }
        public List<Customer> CustomerList { get; set; } = new();

        public async Task OnGetAsync(int? roomId)
        {
            // Load danh sách khách hàng để chọn từ Dropdown
            CustomerList = await _context.Customers.ToListAsync();

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
            // Gán các giá trị mặc định
            Reservation.Status = "Booked";

            _context.Reservations.Add(Reservation);

            // Cập nhật trạng thái phòng sang 'Occupied'
            var room = await _context.Rooms.FindAsync(Reservation.RoomId);
            if (room != null)
            {
                room.Status = "Occupied";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("/Index");
        }
    }
}