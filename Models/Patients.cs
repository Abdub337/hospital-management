using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HPMS.Models
{
    [Table("Patients")]
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [StringLength(50)]
        public string PatientNumber { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Title { get; set; }

        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(20)]
        public string? MaritalStatus { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(20)]
        public string? AlternatePhone { get; set; }

        [StringLength(200)]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? ZipCode { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(5)]
        public string? BloodGroup { get; set; }

        public string? Allergies { get; set; }

        public string? ChronicConditions { get; set; }

        public string? PreviousSurgeries { get; set; }

        public string? FamilyMedicalHistory { get; set; }

        public byte[]? SSN { get; set; }

        public byte[]? InsuranceNumber { get; set; }

        [StringLength(200)]
        public string? InsuranceProvider { get; set; }

        [StringLength(200)]
        public string? EmergencyContactName { get; set; }

        [StringLength(100)]
        public string? EmergencyContactRelation { get; set; }

        [StringLength(20)]
        public string? EmergencyContactPhone { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? CreatedByUserId { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedByUserId { get; set; }
    }
}