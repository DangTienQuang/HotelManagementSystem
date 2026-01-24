using System;

namespace DTOs
{
    public class RoomMaintenanceDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;

        public int? StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
