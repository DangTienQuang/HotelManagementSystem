using BLL.Interfaces;
using DAL.Interfaces;
using DTOs;
using DTOs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class RoomMaintenanceService : IRoomMaintenanceService
    {
        private readonly IRoomMaintenanceRepository _maintenanceRepo;
        private readonly IGenericRepository<Room> _roomRepo;

        public RoomMaintenanceService(IRoomMaintenanceRepository maintenanceRepo, IGenericRepository<Room> roomRepo)
        {
            _maintenanceRepo = maintenanceRepo;
            _roomRepo = roomRepo;
        }

        public async Task<IEnumerable<RoomMaintenanceDto>> GetAllMaintenanceTasksAsync()
        {
            var tasks = await _maintenanceRepo.GetAllMaintenanceTasksAsync();
            return tasks.Select(t => new RoomMaintenanceDto
            {
                Id = t.Id,
                RoomId = t.RoomId,
                RoomNumber = t.Room.RoomNumber,
                StaffId = t.StaffId,
                StaffName = t.Staff?.FullName ?? "Unassigned",
                Reason = t.Reason,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Status = t.Status
            });
        }

        public async Task<IEnumerable<RoomMaintenanceDto>> GetTasksByStaffAsync(int staffUserId)
        {
            var tasks = await _maintenanceRepo.GetMaintenanceTasksByStaffIdAsync(staffUserId);
            return tasks.Select(t => new RoomMaintenanceDto
            {
                Id = t.Id,
                RoomId = t.RoomId,
                RoomNumber = t.Room.RoomNumber,
                StaffId = t.StaffId,
                StaffName = t.Staff?.FullName ?? "Me",
                Reason = t.Reason,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Status = t.Status
            });
        }

        public async Task CreateMaintenanceTaskAsync(int roomId, int staffUserId, string reason)
        {
            // 1. Create the maintenance ticket
            var task = new RoomMaintenance
            {
                RoomId = roomId,
                StaffId = staffUserId,
                Reason = reason,
                StartDate = DateTime.UtcNow,
                Status = "InProgress"
            };

            await _maintenanceRepo.AddAsync(task);

            // 2. Update Room Status
            var room = await _roomRepo.GetByIdAsync(roomId);
            if (room != null)
            {
                room.Status = "Maintenance";
                await _roomRepo.UpdateAsync(room);
            }
        }

        public async Task CompleteMaintenanceTaskAsync(int maintenanceId)
        {
            var task = await _maintenanceRepo.GetByIdAsync(maintenanceId);
            if (task != null && task.Status != "Completed")
            {
                // 1. Close ticket
                task.Status = "Completed";
                task.EndDate = DateTime.UtcNow;
                await _maintenanceRepo.UpdateAsync(task);

                // 2. Free the room
                // Note: We need to check if there are other active maintenance tasks for this room before freeing it?
                // For simplicity as per request: just free it.
                var room = await _roomRepo.GetByIdAsync(task.RoomId);
                if (room != null)
                {
                    room.Status = "Available";
                    await _roomRepo.UpdateAsync(room);
                }
            }
        }
    }
}
