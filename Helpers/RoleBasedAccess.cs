using Microsoft.AspNetCore.Authorization;

namespace HPMS.Helpers
{
    public static class RoleBasedAccess
    {
        public const string Admin = "Admin";
        public const string Doctor = "Doctor";
        public const string Nurse = "Nurse";
        public const string Receptionist = "Receptionist";
        public const string LabTechnician = "LabTechnician";
        public const string Pharmacist = "Pharmacist";

        public const string AdminOnly = "Admin";
        public const string AdminDoctor = "Admin,Doctor";
        public const string AdminReceptionist = "Admin,Receptionist";
        public const string MedicalStaff = "Admin,Doctor,Nurse";
    }

    public class AuthorizeAdminAttribute : AuthorizeAttribute
    {
        public AuthorizeAdminAttribute()
        {
            Roles = RoleBasedAccess.AdminOnly;
        }
    }

    public class AuthorizeAdminDoctorAttribute : AuthorizeAttribute
    {
        public AuthorizeAdminDoctorAttribute()
        {
            Roles = RoleBasedAccess.AdminDoctor;
        }
    }

    public class AuthorizeMedicalStaffAttribute : AuthorizeAttribute
    {
        public AuthorizeMedicalStaffAttribute()
        {
            Roles = RoleBasedAccess.MedicalStaff;
        }
    }

    public class AuthorizeAdminReceptionistAttribute : AuthorizeAttribute
    {
        public AuthorizeAdminReceptionistAttribute()
        {
            Roles = RoleBasedAccess.AdminReceptionist;
        }
    }
}