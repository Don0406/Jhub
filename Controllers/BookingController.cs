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
public async Task<IActionResult> TourDiscovery(string? search, string? landscape, string? intensity, string? activity)
{
    var query = _context.Tours
        .Where(t => t.Status != "Archived")
        .AsQueryable();

    if (!string.IsNullOrEmpty(search))
    {
        var searchLower = search.ToLower();
        query = query.Where(t => 
            (t.TourName != null && t.TourName.ToLower().Contains(searchLower)) ||
            (t.Destination != null && t.Destination.ToLower().Contains(searchLower)) ||
            (t.Activities != null && t.Activities.ToLower().Contains(searchLower)) ||
            (t.LandscapeType != null && t.LandscapeType.ToLower().Contains(searchLower)) ||
            (t.Description != null && t.Description.ToLower().Contains(searchLower))
        );
    }

    if (!string.IsNullOrEmpty(landscape))
    {
        query = query.Where(t => t.LandscapeType == landscape);
    }

    if (!string.IsNullOrEmpty(intensity))
    {
        query = query.Where(t => t.IntensityLevel == intensity);
    }

    if (!string.IsNullOrEmpty(activity))
    {
        query = query.Where(t => t.Activities != null && t.Activities.Contains(activity));
    }
    
    var activeTours = await query.ToListAsync();
    return View("Tour-discovery", activeTours);
}
    [HttpGet]
    [Route("Booking/Details")]
    public async Task<IActionResult> Details(int id)
    {
    
        var tour = await _context.Tours
            .Include(t => t.Bookings)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tour == null) return NotFound();

    
        int actualSlotsFilled = tour.Bookings != null 
            ? tour.Bookings.Where(b => b.Status == "Validated").Sum(b => b.Slots) 
            : 0;

        
        ViewBag.DynamicSlotsFilled = actualSlotsFilled;

        return View("Tour-details", tour);
    }

    [HttpGet]
    [Route("Booking/Booking-form")]
    public async Task<IActionResult> BookingForm(int tourId)
    {
        var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == tourId);
        if (tour == null) return NotFound();
        
        ViewBag.Tour = tour;
        return View("Booking-form");
    }

   [HttpPost]
    [Route("Booking/SubmitBooking")]
    public async Task<IActionResult> SubmitBooking(int tourId, string leadName, int leadAge, string leadEmail, string leadPhone, string specialCategory, string companionNames, int slots, decimal totalAmount)
    {
        // 1. Kunin ang tour kasama ang mga bookings para mabilang ang slots
        var tour = await _context.Tours
            .Include(t => t.Bookings)
            .FirstOrDefaultAsync(t => t.Id == tourId);
            
        if (tour == null) return NotFound();

        // 2. Kalkulahin ang totoong slots na puno na (Validated status lang)
        int actualSlotsFilled = tour.Bookings != null 
            ? tour.Bookings.Where(b => b.Status == "Validated").Sum(b => b.Slots) 
            : 0;

        int availableSeats = tour.TotalCapacity - actualSlotsFilled;

        // 3. I-block kung ang gustong i-book ay mas marami kaysa sa natitirang slots
        if (slots > availableSeats)
        {
            ViewBag.Tour = tour;
            ViewBag.Error = $"Paumanhin, ang natitirang slot na lamang para sa tour na ito ay {availableSeats}. Hindi na ma-aakomodate ang {slots} na slots.";
            return View("Booking-form", tour); // Sinigurado rin na may Model na pinapasa dito
        }

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
            Status = "Pending Validation"
        };
    
        _context.Set<Booking>().Add(newRecord);
        await _context.SaveChangesAsync();
       
        ViewBag.BookingId = newRecord.Id; 
        ViewBag.ReferenceNumber = trackingReference;
        ViewBag.TourName = tour.TourName;
        ViewBag.TotalPaid = totalAmount;
        ViewBag.TotalAmount = totalAmount;
        
        // ITINGIN DITO: Ipinapasa na natin ang 'tour' model at ang ViewBag.Tour para mabasa ng Confirmation view katulad ng Booking-form
        ViewBag.Tour = tour;
        return View("Booking-confirmation", tour);
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

    // 1. Hanapin ang booking
    var booking = await _context.Set<Booking>()
        .Include(b => b.Tour)
        .ThenInclude(t => t.Bookings) // <--- I-include ang lahat ng bookings ng tour na 'to
        .FirstOrDefaultAsync(b => b.ReferenceNumber == referenceNumber.Trim() && b.LeadEmail == email.Trim());

    if (booking == null)
    {
        ViewBag.Error = "No booking record found.";
        return View("Track-booking");
    }

    // 2. I-compute ang total SlotsFilled dynamic para sa tour na ito
    if (booking.Tour != null)
    {
        int actualSlotsFilled = booking.Tour.Bookings != null 
            ? booking.Tour.Bookings.Where(b => b.Status == "Validated").Sum(b => b.Slots) 
            : 0;
        
        // I-set sa ViewBag para magamit sa View
        ViewBag.DynamicSlotsFilled = actualSlotsFilled;
    }

    return View("Booking-status", booking);
}
}
