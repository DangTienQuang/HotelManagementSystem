using HotelManagementSystem.Data.Context;
using HotelManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagementSystem.Web
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new HotelManagementDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<HotelManagementDbContext>>()))
            {
                // Look for any reservations.
                if (context.Reservations.Any())
                {
                    return;   // DB has been seeded
                }

                var customer = new Customer
                {
                    FullName = "Nguyen Van A",
                    Phone = "0987654321",
                    Email = "nguyenvana@example.com",
                    Address = "123 Le Loi, TP.HCM",
                    IdentityNumber = "123456789",
                    CreatedAt = DateTime.Now
                };
                context.Customers.Add(customer);
                context.SaveChanges();

                var room = new Room
                {
                    RoomNumber = "101",
                    RoomType = "Standard",
                    Capacity = 2,
                    Price = 500000,
                    BasePrice = 500000,
                    Status = "Available"
                };
                context.Rooms.Add(room);
                context.SaveChanges();

                context.Reservations.AddRange(
                    new Reservation
                    {
                        CustomerId = customer.Id,
                        RoomId = room.Id,
                        CheckInDate = DateTime.Now.AddDays(1),
                        CheckOutDate = DateTime.Now.AddDays(2),
                        Status = "Pending",
                        CreatedAt = DateTime.Now
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
