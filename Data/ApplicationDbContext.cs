using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HPMS.Models;

namespace HPMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>().ToTable("Patients");
            modelBuilder.Entity<User>().ToTable("AspNetUsers");
            modelBuilder.Entity<Role>().ToTable("AspNetRoles");
            modelBuilder.Entity<Appointment>().ToTable("Appointments");
            modelBuilder.Entity<Doctor>().ToTable("Doctors");
        }
    }
}