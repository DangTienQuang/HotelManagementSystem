using AutoMapper;
using BLL.Interfaces;
using DAL.Interfaces;
using DTOs;
using DTOs.Entities;
using DTOs.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class StaffDashboardService : IStaffDashboardService
    {
        private readonly IGenericRepository<Room> _roomRepository;
        private readonly IMapper _mapper;

        public StaffDashboardService(IGenericRepository<Room> roomRepository, IMapper mapper)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
        }

        public async Task<StaffDashboardDto> GetDashboardDataAsync()
        {
            var rooms = await _roomRepository.GetAllAsync();
            var roomDtos = _mapper.Map<System.Collections.Generic.List<RoomDto>>(rooms);

            return new StaffDashboardDto
            {
                // Các biến đếm (Count)
                OccupiedRoomsCount = roomDtos.Count(r => r.Status == "Occupied"),
                ReservedRoomsCount = roomDtos.Count(r => r.Status == "Reserved"),
                AvailableRoomsCount = roomDtos.Count(r => r.Status == "Available"),
                MaintenanceRoomsCount = roomDtos.Count(r => r.Status == "Maintenance"),
                CleaningRoomsCount = roomDtos.Count(r => r.Status == "Cleaning"),

                // Các danh sách chi tiết (List) -> QUAN TRỌNG
                OccupiedRooms = roomDtos.Where(r => r.Status == "Occupied"),
                ReservedRooms = roomDtos.Where(r => r.Status == "Reserved"),
                CleaningRooms = roomDtos.Where(r => r.Status == "Cleaning"),
                MaintenanceRooms = roomDtos.Where(r => r.Status == "Maintenance") // Dòng này sửa lỗi logic hiển thị
            };
        }
    }
}