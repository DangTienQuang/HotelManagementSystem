using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystem.Data.Context;

namespace HotelManagementSystem.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HotelManagementDbContext _context;

        public LoginModel(HotelManagementDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginInput LoginData { get; set; } = new();

        public class LoginInput
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check User table (Staff / Technician / Admin)
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == LoginData.Username &&
                    u.PasswordHash == LoginData.Password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal);

                if (user.Role == "Staff")
                    return RedirectToPage("/Staffs/MyTasks");

                if (user.Role == "Technician")
                    return RedirectToPage("/Staffs/MaintenanceTasks");

                return RedirectToPage("/Index");
            }

            // Check Customer table
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c =>
                    c.Username == LoginData.Username &&
                    c.PasswordHash == LoginData.Password);

            if (customer != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, customer.Username),
                    new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                    new Claim(ClaimTypes.Role, "Consumer")
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal);

                return RedirectToPage("/MyBookings");
            }

            // If login fails
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return Page();
        }
    }
}