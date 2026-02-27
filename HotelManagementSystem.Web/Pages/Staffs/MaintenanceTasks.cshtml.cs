using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Data.Context;
using HotelManagementSystem.Data.Models;
using System.Security.Claims;

namespace HotelManagementSystem.Web.Pages.Staffs
{
    public class MaintenanceTasksModel : PageModel
    {
        private readonly HotelManagementDbContext _context;

        public MaintenanceTasksModel(HotelManagementDbContext context)
        {
            _context = context;
        }

        public List<MaintenanceTask> MyTasks { get; set; } = new List<MaintenanceTask>();

        public async Task OnGetAsync()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdValue, out int userId))
            {
                // Sử dụng .Include(t => t.Staff) thay vì AssignedToNavigation
                MyTasks = await _context.MaintenanceTasks
                    .Include(t => t.Room)
                    .Include(t => t.Staff)
                    .Where(t => t.AssignedTo == userId && t.Status != "Completed")
                    .OrderByDescending(t => t.Priority == "High" ? 1 : 0)
                    .ThenBy(t => t.Deadline)
                    .ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            var task = await _context.MaintenanceTasks
                .Include(t => t.Room)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task != null)
            {
                task.Status = "Completed";
                task.CompletedAt = DateTime.Now;

                if (task.Room != null)
                {
                    task.Room.Status = "Available";
                }

                await _context.SaveChangesAsync();
                TempData["Message"] = $"Đã hoàn thành sửa chữa phòng {task.Room?.RoomNumber}";
            }

            return RedirectToPage();
        }
    }
}