using BLL.Service;
using DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HotelManagementSystem.Models;

namespace HotelManagementSystem.Controllers
{
    public class FrontDeskController : Controller
    {
        private readonly FrontDeskService _frontDeskService;
        private readonly IReservationRepository _reservationRepo;

        public FrontDeskController(FrontDeskService service, IReservationRepository repo)
        {
            _frontDeskService = service;
            _reservationRepo = repo;
        }

        // Dashboard: Shows Arrivals & In-House Guests
        public IActionResult Index()
        {
            var model = new FrontDeskViewModel
            {
                Arrivals = _reservationRepo.GetTodayArrivals(),
                InHouse = _reservationRepo.GetActiveReservations()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CheckIn(int id)
        {
            try
            {
                // 1. Safe Retrieval of User ID
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // If ID is missing, force them to log in again
                if (string.IsNullOrEmpty(userIdString))
                {
                    return RedirectToAction("Login", "Account");
                }

                int staffId = int.Parse(userIdString);

                _frontDeskService.CheckInGuest(id, staffId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult CheckOut(int id)
        {
            try
            {
                // 1. Safe Retrieval of User ID
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdString))
                {
                    return RedirectToAction("Login", "Account");
                }

                int staffId = int.Parse(userIdString);

                _frontDeskService.CheckOutGuest(id, staffId);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public async Task<IActionResult> CheckoutPreview(int id)
        {
            var reservation = await _reservationRepo.GetReservationWithDetailsAsync(id);
            if (reservation == null) return NotFound();

            // 1. Find the active Check-In record (the one without a checkout time)
            var checkInRecord = reservation.CheckInOuts.FirstOrDefault(c => c.CheckOutTime == null);
            if (checkInRecord == null || checkInRecord.CheckInTime == null)
            {
                TempData["Error"] = "No active check-in record found for this guest.";
                return RedirectToAction("Index");
            }

            // 2. Calculate the Bill Preview
            var checkInTime = checkInRecord.CheckInTime.Value;
            var checkOutTime = DateTime.Now;

            // Logic: Round up to the next full day. Minimum 1 day.
            var duration = checkOutTime - checkInTime;
            int daysStayed = (int)Math.Ceiling(duration.TotalDays);
            if (daysStayed < 1) daysStayed = 1;

            decimal roomPrice = reservation.Room.Price;
            decimal totalPrice = roomPrice * daysStayed;

            // 3. Pass data to View using a simple ViewModel or ViewBag
            ViewBag.DaysStayed = daysStayed;
            ViewBag.TotalPrice = totalPrice;
            ViewBag.CheckInTime = checkInTime;
            ViewBag.CheckOutTime = checkOutTime;

            return View(reservation);
        }

    }

}
