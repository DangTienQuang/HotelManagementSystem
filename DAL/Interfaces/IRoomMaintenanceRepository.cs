using DTOs.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IRoomMaintenanceRepository : IGenericRepository<RoomMaintenance>
    {
        Task<IEnumerable<RoomMaintenance>> GetAllMaintenanceTasksAsync();
        Task<IEnumerable<RoomMaintenance>> GetMaintenanceTasksByStaffIdAsync(int staffUserId);
    }
}
