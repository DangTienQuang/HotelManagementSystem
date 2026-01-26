using DTOs.Entities;

namespace HotelManagementSystem.Models
{
    public class FrontDeskViewModel
    {
        public IEnumerable<Reservation> Arrivals { get; set; } = new List<Reservation>();
        public IEnumerable<Reservation> InHouse { get; set; } = new List<Reservation>();
    }
}
