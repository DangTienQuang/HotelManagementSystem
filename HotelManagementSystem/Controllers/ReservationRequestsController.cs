using Microsoft.AspNetCore.Mvc;
using BLL.Service;
using DAL.Interfaces; // Or DAL.Interface depending on your namespace

namespace HotelManagementSystem.Controllers
{
    public class ReservationRequestsController : Controller
    {
        private readonly FrontDeskService _frontDeskService;
        private readonly IReservationRepository _reservationRepo;

        public ReservationRequestsController(FrontDeskService service, IReservationRepository repo)
        {
            _frontDeskService = service;
            _reservationRepo = repo;
        }

        public IActionResult Index()
        {
            // Get all "Pending" reservations
            var pendingList = _reservationRepo.GetPendingReservations();
            return View(pendingList);
        }

        [HttpPost]
        public IActionResult Confirm(int id)
        {
            try
            {
                _frontDeskService.ConfirmReservation(id);
                TempData["Message"] = "Reservation Confirmed! It is now visible at the Front Desk.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Cancel(int id)
        {
            try
            {
                _frontDeskService.CancelReservation(id);
                TempData["Message"] = "Reservation Cancelled.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}