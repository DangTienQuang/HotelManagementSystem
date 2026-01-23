using DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationDto> CreateReservationAsync(CreateReservationDto reservationDto, string username);
        Task<IEnumerable<ReservationDto>> GetUserReservationsAsync(string username);
        Task<ReservationDto?> GetReservationByIdAsync(int id);
        Task CancelReservationAsync(int id);
        Task<bool> IsRoomAvailableAsync(int roomId, System.DateTime checkIn, System.DateTime checkOut);
    }
}
