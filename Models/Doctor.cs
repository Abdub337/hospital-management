using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HPMS.Models
{
    [Table("Doctors")]
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string DoctorNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Specialization { get; set; } = string.Empty;

        [StringLength(200)]
        public string? SubSpecialization { get; set; }

        [Required]
        [StringLength(100)]
        public string LicenseNumber { get; set; } = string.Empty;

        public int? YearsOfExperience { get; set; }

        [StringLength(500)]
        public string? Qualification { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal ConsultationFee { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? EmergencyFee { get; set; }

        [StringLength(20)]
        public string? RoomNumber { get; set; }

        public int? FloorNumber { get; set; }

        [StringLength(100)]
        public string? AvailableDays { get; set; }

        public TimeSpan? AvailableTimeStart { get; set; }

        public TimeSpan? AvailableTimeEnd { get; set; }

        public int? SlotDuration { get; set; }

        public bool IsAvailable { get; set; }

        public int? MaxPatientsPerDay { get; set; }

        public string? Bio { get; set; }

        [StringLength(500)]
        public string? ProfileImage { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}