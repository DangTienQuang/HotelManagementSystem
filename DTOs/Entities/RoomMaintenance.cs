using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTOs.Entities
{
    public class RoomMaintenance
    {
        public int Id { get; set; }

        public int RoomId { get; set; }
        public virtual Room Room { get; set; } = null!;

        public int? StaffId { get; set; }
        [ForeignKey("StaffId")]
        public virtual User? Staff { get; set; }

        public string Reason { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "InProgress"; // InProgress, Completed
    }
}
