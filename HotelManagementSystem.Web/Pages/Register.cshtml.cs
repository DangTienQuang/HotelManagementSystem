using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HotelManagementSystem.Data.Context;
using HotelManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Web.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly HotelManagementDbContext _context;

        public RegisterModel(HotelManagementDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new();

        public class RegisterInput
        {
            [Required]
            public string FullName { get; set; } = string.Empty;

            [Required]
            public string Phone { get; set; } = string.Empty;

            [Required]
            public string IdentityNumber { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Address { get; set; } = string.Empty;

            [Required]
            public string Username { get; set; } = string.Empty;

            [Required]
            [MinLength(6)]
            public string Password { get; set; } = string.Empty;

            [Required]
            [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet()
        {
            ModelState.Clear();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var username = Input.Username.Trim();
            var existedUsername = await _context.Customers.AnyAsync(c => c.Username == username)
                                  || await _context.Users.AnyAsync(u => u.Username == username);
            if (existedUsername)
            {
                ModelState.AddModelError("Input.Username", "Tên đăng nhập đã tồn tại.");
                return Page();
            }

            try
            {
                var customer = new Customer
                {
                    FullName = Input.FullName.Trim(),
                    Phone = Input.Phone.Trim(),
                    IdentityNumber = Input.IdentityNumber.Trim(),
                    Email = Input.Email.Trim(),
                    Address = Input.Address.Trim(),
                    Username = username,
                    PasswordHash = Input.Password,
                    CreatedAt = DateTime.Now
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi: " + (ex.InnerException?.Message ?? ex.Message));
                return Page();
            }
        }
    }
}
