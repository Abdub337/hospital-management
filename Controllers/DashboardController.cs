using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HPMS.Data;
using HPMS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HPMS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var role = user?.Role ?? "User";

            ViewBag.TotalPatients = await _context.Patients.CountAsync();
            ViewBag.ActivePatients = await _context.Patients.CountAsync(p => p.IsActive == true);
            ViewBag.TodayAppointments = await _context.Appointments
                .Where(a => a.AppointmentDateTime.Date == DateTime.Today && a.Status != "Cancelled")
                .CountAsync();
            ViewBag.UpcomingAppointments = await _context.Appointments
                .Where(a => a.AppointmentDateTime > DateTime.Now && a.Status != "Cancelled")
                .CountAsync();
            ViewBag.UserRole = role;

            var recentAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.AppointmentDateTime.Date == DateTime.Today)
                .OrderBy(a => a.AppointmentDateTime)
                .Take(10)
                .ToListAsync();

            ViewBag.RecentAppointments = recentAppointments;

            var recentPatients = await _context.Patients
                .OrderByDescending(p => p.CreatedAt)
                .Take(10)
                .ToListAsync();

            return View(recentPatients);
        }

        // GET: Dashboard/Search - Quick search from dashboard
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            var patients = await _context.Patients
                .Where(p => p.FullName.Contains(searchTerm) ||
                            p.PatientNumber.Contains(searchTerm) ||
                            p.Phone.Contains(searchTerm))
                .Take(20)
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SearchResultCount = patients.Count;
            return View(patients);
        }
    }
}