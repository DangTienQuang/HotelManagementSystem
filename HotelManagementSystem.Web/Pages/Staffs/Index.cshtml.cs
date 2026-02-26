using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Data.Context;
using HotelManagementSystem.Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagementSystem.Web.Pages.Staffs
{
    [Authorize(Roles = "Admin")] // Chỉ Admin mới được vào xem danh sách này
    public class IndexModel : PageModel
    {
        private readonly HotelManagementDbContext _context;

        public IndexModel(HotelManagementDbContext context)
        {
            _context = context;
        }

        public IList<HotelManagementSystem.Data.Models.Staff> StaffList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Lấy danh sách nhân viên và nạp luôn thông tin User liên quan
            StaffList = await _context.Staffs
                .Include(s => s.User)
                .OrderByDescending(s => s.User.CreatedAt)
                .ToListAsync();
        }

        // Hàm xử lý Xóa nhân viên (dành cho Admin dọn dẹp data test)
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var staff = await _context.Staffs.Include(s => s.User).FirstOrDefaultAsync(m => m.Id == id);

            if (staff != null)
            {
                _context.Staffs.Remove(staff);
                if (staff.User != null) _context.Users.Remove(staff.User);

                await _context.SaveChangesAsync();
                TempData["Message"] = "Đã xóa nhân viên thành công.";
            }

            return RedirectToPage("./Index");
        }
    }
}