using HPMS.Data;
using HPMS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HPMS.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string _reportPath = @"C:\HPMS_Reports";

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;

            // Create reports folder if it doesn't exist
            if (!Directory.Exists(_reportPath))
            {
                Directory.CreateDirectory(_reportPath);
            }
        }

        // GET: Reports/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: Reports/PatientReport
        [AuthorizeAdmin]
        public async Task<IActionResult> PatientReport(DateTime? fromDate, DateTime? toDate)
        {
            var startDate = fromDate ?? DateTime.Today.AddDays(-30);
            var endDate = toDate ?? DateTime.Today;

            var patients = await _context.Patients
                .Where(p => p.CreatedAt.Date >= startDate.Date && p.CreatedAt.Date <= endDate.Date)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var fileName = $"Patient_Report_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.pdf";
            var filePath = Path.Combine(_reportPath, fileName);

            // Generate PDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Header().Text("Hospital Management System - Patient Report").Bold().FontSize(20);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().Text($"Report Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
                        table.Cell().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}");
                        table.Cell().ColumnSpan(2).PaddingTop(10);

                        table.Cell().ColumnSpan(2).Table(innerTable =>
                        {
                            innerTable.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(30);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(1);
                                cols.RelativeColumn(2);
                            });

                            innerTable.Header(header =>
                            {
                                header.Cell().Text("#").Bold();
                                header.Cell().Text("Patient Number").Bold();
                                header.Cell().Text("Full Name").Bold();
                                header.Cell().Text("Gender").Bold();
                                header.Cell().Text("Phone").Bold();
                            });

                            int count = 1;
                            foreach (var patient in patients)
                            {
                                innerTable.Cell().Text(count.ToString());
                                innerTable.Cell().Text(patient.PatientNumber);
                                innerTable.Cell().Text(patient.FullName);
                                innerTable.Cell().Text(patient.Gender ?? "N/A");
                                innerTable.Cell().Text(patient.Phone);
                                count++;
                            }
                        });

                        table.Cell().ColumnSpan(2).PaddingTop(10);
                        table.Cell().Text($"Total Patients: {patients.Count}").Bold();
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                    });
                });
            });

            document.GeneratePdf(filePath);

            TempData["Success"] = $"Patient report saved to: {filePath}";
            return RedirectToAction(nameof(Index));
        }

        // GET: Reports/AppointmentReport
        [AuthorizeAdminDoctor]
        public async Task<IActionResult> AppointmentReport(DateTime? fromDate, DateTime? toDate)
        {
            var startDate = fromDate ?? DateTime.Today.AddDays(-30);
            var endDate = toDate ?? DateTime.Today;

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.AppointmentDateTime.Date >= startDate.Date && a.AppointmentDateTime.Date <= endDate.Date)
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();

            var fileName = $"Appointment_Report_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.pdf";
            var filePath = Path.Combine(_reportPath, fileName);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Header().Text("Hospital Management System - Appointment Report").Bold().FontSize(20);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().Text($"Report Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
                        table.Cell().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}");
                        table.Cell().ColumnSpan(2).PaddingTop(10);

                        table.Cell().ColumnSpan(2).Table(innerTable =>
                        {
                            innerTable.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(30);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(1);
                            });

                            innerTable.Header(header =>
                            {
                                header.Cell().Text("#").Bold();
                                header.Cell().Text("Appointment #").Bold();
                                header.Cell().Text("Patient").Bold();
                                header.Cell().Text("Date & Time").Bold();
                                header.Cell().Text("Type").Bold();
                                header.Cell().Text("Status").Bold();
                            });

                            int count = 1;
                            foreach (var apt in appointments)
                            {
                                innerTable.Cell().Text(count.ToString());
                                innerTable.Cell().Text(apt.AppointmentNumber);
                                innerTable.Cell().Text(apt.Patient?.FullName ?? "N/A");
                                innerTable.Cell().Text(apt.AppointmentDateTime.ToString("yyyy-MM-dd HH:mm"));
                                innerTable.Cell().Text(apt.Type);
                                innerTable.Cell().Text(apt.Status);
                                count++;
                            }
                        });

                        table.Cell().ColumnSpan(2).PaddingTop(10);
                        table.Cell().Text($"Total Appointments: {appointments.Count}").Bold();

                        var completedCount = appointments.Count(a => a.Status == "Completed");
                        var cancelledCount = appointments.Count(a => a.Status == "Cancelled");
                        var scheduledCount = appointments.Count(a => a.Status == "Scheduled");

                        table.Cell().ColumnSpan(2).PaddingTop(5);
                        table.Cell().Text($"Completed: {completedCount} | Cancelled: {cancelledCount} | Scheduled: {scheduledCount}");
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                    });
                });
            });

            document.GeneratePdf(filePath);

            TempData["Success"] = $"Appointment report saved to: {filePath}";
            return RedirectToAction(nameof(Index));
        }
    }
}