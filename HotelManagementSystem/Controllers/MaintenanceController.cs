using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace HotelManagementSystem.Controllers
{
    [Authorize]
    public class MaintenanceController : Controller
    {
        private readonly IRoomMaintenanceService _maintenanceService;
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;

        public MaintenanceController(
            IRoomMaintenanceService maintenanceService,
            IRoomService roomService,
            IUserService userService)
        {
            _maintenanceService = maintenanceService;
            _roomService = roomService;
            _userService = userService;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var tasks = await _maintenanceService.GetAllMaintenanceTasksAsync();
            return View(tasks);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            var staff = await _userService.GetStaffUsersAsync();

            ViewBag.Rooms = new SelectList(rooms, "Id", "RoomNumber");
            ViewBag.Staff = new SelectList(staff, "Id", "FullName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(int roomId, int staffUserId, string reason)
        {
            await _maintenanceService.CreateMaintenanceTaskAsync(roomId, staffUserId, reason);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Staff, Administrator")]
        public async Task<IActionResult> Complete(int id)
        {
            await _maintenanceService.CompleteMaintenanceTaskAsync(id);

            if (User.IsInRole("Administrator"))
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index", "StaffDashboard");
        }
    }
}
