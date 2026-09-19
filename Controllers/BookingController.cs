using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _UPDATED__FrontEnd_Capstone.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace _UPDATED__FrontEnd_Capstone.Controllers;

public class BookingController : Controller
{
    private readonly AppDbContext _context;

    public BookingController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("Booking/Tour-discovery")]
    [Route("tour-discovery")]
    public async Task<IActionResult> TourDiscovery(string? search)
    {
        var query = _context.Tours
            .Include(t => t.Bookings)
            .Where(t => t.Status == "Active")
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(t =>
                t.TourName.Contains(search) || t.Destination.Contains(search));
        }

        var tours = await query
            .OrderByDescending(t => t.IsBoosted)
            .ThenBy(t => t.DepartureDate)
            .ToListAsync();

        // Recompute live SlotsFilled so badges are correct
        foreach (var t in tours)
        {
            t.SlotsFilled = t.Bookings != null
                ? t.Bookings
                    .Where(b => b.Status == "Validated" || b.Status == "Pending Validation")
                    .Sum(b => b.Slots)
                : 0;
        }

        return View("Tour-discovery", tours);
    }

    [HttpGet]
    [Route("Booking/Details")]
    public async Task<IActionResult> Details(int id)
    {
        var tour = await _context.Tours
            .Include(t => t.Bookings)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tour == null) return NotFound();

        // Count BOTH validated and pending-in-flight so no race window
        int actualSlotsFilled = tour.Bookings != null
            ? tour.Bookings
                .Where(b => b.Status == "Validated" || b.Status == "Pending Validation")
                .Sum(b => b.Slots)
            : 0;

        ViewBag.DynamicSlotsFilled = actualSlotsFilled;
        return View("Tour-details", tour);
    }

    [HttpGet]
    [Route("Booking/Booking-form")]
    public async Task<IActionResult> BookingForm(int tourId)
    {
        var tour = await _context.Tours
            .Include(t => t.Bookings)
            .FirstOrDefaultAsync(t => t.Id == tourId);
        if (tour == null) return NotFound();

        // Live guard — show error if it's already full
        int reserved = tour.Bookings != null
            ? tour.Bookings
                .Where(b => b.Status == "Validated" || b.Status == "Pending Validation")
                .Sum(b => b.Slots)
            : 0;

        if (reserved >= tour.TotalCapacity)
        {
            TempData["BookingError"] = "This tour is fully booked.";
            return RedirectToAction("Details", new { id = tourId });
        }

        ViewBag.Tour = tour;
        return View("Booking-form");
    }

    [HttpPost]
    [Route("Booking/SubmitBooking")]
    public async Task<IActionResult> SubmitBooking(
        int tourId, string leadName, int leadAge, string leadEmail,
        string leadPhone, string specialCategory, string companionNames,
        int slots, decimal totalAmount)
    {
        var tour = await _context.Tours
            .Include(t => t.Bookings)
            .FirstOrDefaultAsync(t => t.Id == tourId);
        if (tour == null) return NotFound();

        // SLOT GUARD — includes in-flight pending bookings
        int slotsReserved = tour.Bookings != null
            ? tour.Bookings
                .Where(b => b.Status == "Validated" || b.Status == "Pending Validation")
                .Sum(b => b.Slots)
            : 0;

        int remaining = tour.TotalCapacity - slotsReserved;

        if (remaining <= 0)
        {
            TempData["BookingError"] = "This tour is fully booked. No slots remaining.";
            return RedirectToAction("Details", new { id = tourId });
        }
        if (slots > remaining)
        {
            TempData["BookingError"] = $"Only {remaining} slot(s) remaining for this tour.";
            return RedirectToAction("Details", new { id = tourId });
        }
        if (slots < 1) slots = 1;

        var randomSignature = new Random().Next(10000, 99999);
        var trackingReference = $"JH-{randomSignature}";

        var newRecord = new Booking
        {
            ReferenceNumber = trackingReference,
            TourId = tourId,
            LeadName = leadName,
            LeadAge = leadAge,
            LeadEmail = leadEmail,
            LeadPhone = leadPhone,
            SpecialCategory = specialCategory,
            CompanionNames = companionNames,
            Slots = slots,
            TotalAmount = totalAmount,
            Status = "Pending Validation",
            BookingDate = DateTime.Now
        };

        _context.Set<Booking>().Add(newRecord);
        await _context.SaveChangesAsync();

        ViewBag.BookingId = newRecord.Id;
        ViewBag.ReferenceNumber = trackingReference;
        ViewBag.TourName = tour.TourName;
        ViewBag.TotalPaid = totalAmount;
        return View("Booking-confirmation");
    }

    [HttpPost]
    [Route("Booking/UploadConfirmationPoP")]
    public async Task<IActionResult> UploadConfirmationPoP(int bookingId, IFormFile popFile)
    {
        if (bookingId == 0 || popFile == null) return RedirectToAction("Track");

        var booking = await _context.Set<Booking>().FindAsync(bookingId);
        if (booking != null)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(popFile.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await popFile.CopyToAsync(stream);
            }

            booking.ProofOfPaymentUrl = "/uploads/" + fileName;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Track");
    }

    [HttpGet]
    [Route("Booking/Track")]
    public IActionResult Track()
    {
        return View("Track-booking");
    }

    [HttpPost]
    [Route("Booking/TrackStatus")]
    public async Task<IActionResult> TrackStatus(string referenceNumber, string email)
    {
        if (string.IsNullOrEmpty(referenceNumber) || string.IsNullOrEmpty(email))
        {
            ViewBag.Error = "Please fill in all fields.";
            return View("Track-booking");
        }

        var booking = await _context.Set<Booking>()
            .Include(b => b.Tour)
            .ThenInclude(t => t!.Bookings)
            .FirstOrDefaultAsync(b => b.ReferenceNumber == referenceNumber.Trim()
                                   && b.LeadEmail == email.Trim());

        if (booking == null)
        {
            ViewBag.Error = "No booking record found.";
            return View("Track-booking");
        }

        if (booking.Tour != null)
        {
            int actualSlotsFilled = booking.Tour.Bookings != null
                ? booking.Tour.Bookings
                    .Where(b => b.Status == "Validated" || b.Status == "Pending Validation")
                    .Sum(b => b.Slots)
                : 0;
            ViewBag.DynamicSlotsFilled = actualSlotsFilled;
        }

        return View("Booking-status", booking);
    }
}
