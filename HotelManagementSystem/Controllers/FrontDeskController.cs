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

    }

}
