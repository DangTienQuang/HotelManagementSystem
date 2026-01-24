using DAL.Interfaces;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class RoomMaintenanceRepository : GenericRepository<RoomMaintenance>, IRoomMaintenanceRepository
    {
        public RoomMaintenanceRepository(HotelDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RoomMaintenance>> GetAllMaintenanceTasksAsync()
        {
            return await _context.RoomMaintenances
                .Include(rm => rm.Room)
                .Include(rm => rm.Staff)
                .OrderByDescending(rm => rm.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<RoomMaintenance>> GetMaintenanceTasksByStaffIdAsync(int staffUserId)
        {
            return await _context.RoomMaintenances
                .Include(rm => rm.Room)
                .Include(rm => rm.Staff)
                .Where(rm => rm.StaffId == staffUserId && rm.Status == "InProgress")
                .OrderBy(rm => rm.StartDate)
                .ToListAsync();
        }
    }
}
