using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HPMS.Data;
using HPMS.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HPMS.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Doctor/Index
        public async Task<IActionResult> Index()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .ToListAsync();
            return View(doctors);
        }

        // GET: Doctor/Register
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var users = await _context.Users
                .Where(u => u.Role == "Doctor")
                .ToListAsync();
            ViewBag.UserList = users;
            return View();
        }

        // POST: Doctor/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Doctor doctor)
        {
            if (doctor.UserId == 0)
            {
                TempData["Error"] = "Please select a user";
                var users = await _context.Users.Where(u => u.Role == "Doctor").ToListAsync();
                ViewBag.UserList = users;
                return View(doctor);
            }

            doctor.DoctorNumber = "DOC-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            doctor.IsAvailable = true;

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Doctor registered successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}