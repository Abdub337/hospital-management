using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace HPMS.Models
{
    [Table("AspNetRoles")]
    public class Role : IdentityRole<int>
    {
    }
}