using HotelManagementSystem.Business;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagementSystem.Web.Pages // THÊM DÒNG NÀY
{
    public class BookingModel : PageModel
    {
        private readonly BookingService _service;

        [BindProperty]
        public BookingRequest RequestData { get; set; } = new();

        public BookingModel(BookingService service) => _service = service;
        public void OnGet(int? roomId) // Thêm tham số roomId
        {
            RequestData = new BookingRequest
            {
                RoomId = roomId ?? 0, // Nếu có roomId truyền tới thì tự điền vào form
                CheckInDate = DateTime.Now,
                CheckOutDate = DateTime.Now.AddDays(1)
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await _service.ProcessBooking(RequestData);
            if (result) return RedirectToPage("Index"); // Chuyển về Index nếu chưa có trang Success

            ModelState.AddModelError("", "Đặt phòng không thành công! Vui lòng kiểm tra lại trạng thái phòng.");
            return Page();
        }
    }
}