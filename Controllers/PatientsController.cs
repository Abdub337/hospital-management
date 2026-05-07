using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HPMS.Data;
using HPMS.Models;

namespace HPMS.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Patient/Index
        public async Task<IActionResult> Index()
        {
            var patients = await _context.Patients.ToListAsync();
            return View(patients);
        }

        // GET: Patient/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Patient/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Patient patient)
        {
            try
            {
                // Generate unique patient number
                patient.PatientNumber = "PAT-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                patient.CreatedAt = DateTime.Now;
                patient.IsActive = true;
                patient.IsDeleted = false;

                if (string.IsNullOrEmpty(patient.Country))
                {
                    patient.Country = "Kenya";
                }

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Patient registered successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(patient);
            }
        }

        // GET: Patient/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patient/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patient/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            try
            {
                patient.UpdatedAt = DateTime.Now;
                _context.Update(patient);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Patient updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(patient);
            }
        }

        // GET: Patient/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Patient deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Patient/Search
        public async Task<IActionResult> Search(string searchTerm)
        {
            ViewBag.SearchTerm = searchTerm;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                var allPatients = await _context.Patients.ToListAsync();
                return View("Index", allPatients);
            }

            var patients = await _context.Patients
                .Where(p => p.FullName.Contains(searchTerm) ||
                            p.PatientNumber.Contains(searchTerm) ||
                            p.Phone.Contains(searchTerm) ||
                            (p.Email != null && p.Email.Contains(searchTerm)))
                .ToListAsync();

            ViewBag.SearchResultCount = patients.Count;
            return View("Index", patients);
        }
    }
}