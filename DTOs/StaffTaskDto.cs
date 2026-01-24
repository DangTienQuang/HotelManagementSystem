using System.Collections.Generic;

namespace DTOs
{
    public class StaffTaskDto
    {
        public IEnumerable<RoomCleaningDto> MyCleaningTasks { get; set; } = new List<RoomCleaningDto>();
        public IEnumerable<RoomMaintenanceDto> MyMaintenanceTasks { get; set; } = new List<RoomMaintenanceDto>();
    }
}
