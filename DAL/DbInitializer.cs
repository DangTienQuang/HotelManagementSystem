using DTOs.Entities;
using System;
using System.Linq;

namespace DAL
{
    [Obsolete("Use IDatabaseSeeder service instead")]
    public static class DbInitializer
    {
        public static void Initialize(HotelDbContext context)
        {
            context.Database.EnsureCreated();
        }

        public static void SeedData(HotelDbContext context, string adminPasswordHash)
        {
            if (context.Users.Any())
            {
                return;
            }

            var adminUser = new User
            {
                Username = "admin",
                PasswordHash = adminPasswordHash,
                Role = "Staff",
                FullName = "System Administrator",
                Email = "admin@hotel.com",
                CreatedAt = DateTime.UtcNow
            };

            var staff = new Staff
            {
                User = adminUser,
                Position = "Manager",
                Shift = "Day",
                HireDate = DateTime.Now
            };

            context.Users.Add(adminUser);
            context.Staffs.Add(staff);
            context.SaveChanges();
        }
    }
}



