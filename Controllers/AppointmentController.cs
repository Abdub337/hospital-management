using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HPMS.Data;
using HPMS.Models;
using HPMS.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HPMS.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();

            return View(appointments);
        }

        [AuthorizeAdminReceptionist]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var patients = await _context.Patients.Where(p => p.IsActive == true).ToListAsync();
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Where(d => d.IsAvailable == true)
                .ToListAsync();

            ViewBag.PatientList = new SelectList(patients, "PatientId", "FullName");
            ViewBag.DoctorList = new SelectList(doctors, "DoctorId", "User.FullName");

            return View();
        }

        [AuthorizeAdminReceptionist]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.AppointmentNumber = $"APT-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                appointment.CreatedAt = DateTime.Now;
                appointment.Status = "Scheduled";
                appointment.Duration = 30;

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Appointment scheduled successfully! Appointment #: {appointment.AppointmentNumber}";
                return RedirectToAction(nameof(Index));
            }

            var patients = await _context.Patients.Where(p => p.IsActive == true).ToListAsync();
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Where(d => d.IsAvailable == true)
                .ToListAsync();

            ViewBag.PatientList = new SelectList(patients, "PatientId", "FullName");
            ViewBag.DoctorList = new SelectList(doctors, "DoctorId", "User.FullName");

            return View(appointment);
        }

        [AuthorizeMedicalStaff]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            var patients = await _context.Patients.Where(p => p.IsActive == true).ToListAsync();
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Where(d => d.IsAvailable == true)
                .ToListAsync();

            ViewBag.PatientList = new SelectList(patients, "PatientId", "FullName", appointment.PatientId);
            ViewBag.DoctorList = new SelectList(doctors, "DoctorId", "User.FullName", appointment.DoctorId);

            return View(appointment);
        }

        [AuthorizeMedicalStaff]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.AppointmentId) return NotFound();

            if (ModelState.IsValid)
            {
                appointment.UpdatedAt = DateTime.Now;
                _context.Update(appointment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Appointment updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            var patients = await _context.Patients.Where(p => p.IsActive == true).ToListAsync();
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Where(d => d.IsAvailable == true)
                .ToListAsync();

            ViewBag.PatientList = new SelectList(patients, "PatientId", "FullName", appointment.PatientId);
            ViewBag.DoctorList = new SelectList(doctors, "DoctorId", "User.FullName", appointment.DoctorId);

            return View(appointment);
        }

        [AuthorizeMedicalStaff]
        [HttpGet]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null) return NotFound();

            return View(appointment);
        }

        [AuthorizeMedicalStaff]
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = "Cancelled";
                appointment.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Appointment cancelled successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}