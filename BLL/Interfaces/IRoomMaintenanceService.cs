using DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IRoomMaintenanceService
    {
        Task<IEnumerable<RoomMaintenanceDto>> GetAllMaintenanceTasksAsync();
        Task<IEnumerable<RoomMaintenanceDto>> GetTasksByStaffAsync(int staffUserId);
        Task CreateMaintenanceTaskAsync(int roomId, int staffUserId, string reason);
        Task CompleteMaintenanceTaskAsync(int maintenanceId);
    }
}
