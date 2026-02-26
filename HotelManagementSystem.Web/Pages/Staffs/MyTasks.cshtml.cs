using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HotelManagementSystem.Data.Context;
using HotelManagementSystem.Data.Models;

namespace HotelManagementSystem.Web.Pages.Staff
{
    public class MyTasksModel : PageModel
    {
        private readonly HotelManagementDbContext _context;
        public MyTasksModel(HotelManagementDbContext context) => _context = context;

        // Dùng đường dẫn đầy đủ để tránh trùng tên namespace Staff
        public List<HotelManagementSystem.Data.Models.RoomCleaning> MyCleaningTasks { get; set; } = new();

        public async Task OnGetAsync()
        {
            // CÁCH SỬA LỖI: Lấy Claim rồi mới lấy Value, tránh dùng hàm rút gọn FindFirstValue
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            string userIdStr = claim != null ? claim.Value : string.Empty;

            if (int.TryParse(userIdStr, out int currentUserId))
            {
                MyCleaningTasks = await _context.RoomCleanings
                    .Include(c => c.Room)
                    .Where(c => c.CleanedBy == currentUserId && (c.Status == "In Progress" || c.Status == "Assigned"))
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostCompleteAsync(int taskId)
        {
            var task = await _context.RoomCleanings.Include(t => t.Room).FirstOrDefaultAsync(t => t.Id == taskId);
            if (task != null)
            {
                task.Status = "Completed";
                if (task.Room != null)
                {
                    task.Room.Status = "Available";
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}