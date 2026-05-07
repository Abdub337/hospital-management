using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HPMS.Models
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string AppointmentNumber { get; set; } = string.Empty;

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime AppointmentDateTime { get; set; }

        public int Duration { get; set; } = 30;

        [StringLength(20)]
        public string Status { get; set; } = "Scheduled";

        [StringLength(50)]
        public string Type { get; set; } = "Regular";

        [StringLength(500)]
        public string? Reason { get; set; }

        public string? Symptoms { get; set; }

        public string? Notes { get; set; }

        public bool IsFirstVisit { get; set; } = false;

        public bool ReminderSent { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? CreatedByUserId { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedByUserId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor? Doctor { get; set; }
    }
}