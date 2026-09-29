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
        return View("Booking-form", tour);
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
        Status = "Pending Validation",
        BookedAt = DateTime.Now // <--- Properly initialized BookedAt timestamp
    };

    _context.Set<Booking>().Add(newRecord);
    await _context.SaveChangesAsync();
   
    ViewBag.BookingId = newRecord.Id; 
    ViewBag.ReferenceNumber = trackingReference;
    ViewBag.TourName = tour.TourName;
    ViewBag.TotalPaid = totalAmount;
    ViewBag.TotalAmount = totalAmount; // <--- Siguraduhing maipapasa ang eksaktong total amount dito para mabasa ng confirmation view
    
    ViewBag.Tour = tour;
    return View("Booking-confirmation", tour);
}


[HttpGet]
public async Task<IActionResult> Status(int id)
{
    var booking = await _context.Bookings
        .Include(b => b.Tour)
        .FirstOrDefaultAsync(b => b.Id == id);

    if (booking == null)
    {
        return NotFound();
    }

    // Kalkulahin ang dynamic slots filled mula sa mga validated o confirmed bookings para sa tour na ito
    int slotsFilled = await _context.Bookings
        .Where(b => b.TourId == booking.TourId && (b.Status == "Validated" || b.Status == "Confirmed" || b.Status == "Approved"))
        .SumAsync(b => 1 + (string.IsNullOrEmpty(b.CompanionNames) ? 0 : b.CompanionNames.Split(',').Length));

    ViewBag.DynamicSlotsFilled = slotsFilled;

    return View("Booking-status", booking);
}

[HttpPost]
[Route("Booking/UploadConfirmationPoP")]
public async Task<IActionResult> UploadConfirmationPoP(int bookingId, IFormFile popFile)
{
    var booking = await _context.Bookings
        .Include(b => b.Tour)
        .FirstOrDefaultAsync(b => b.Id == bookingId);

    if (booking == null)
    {
        return NotFound();
    }

    if (popFile != null && popFile.Length > 0)
    {
        // 1. Gumawa ng folder kung wala pa sa wwwroot/uploads/payments
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "payments");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // 2. Gumawa ng unique filename para iwas magkabaliktad o magkapatong ang file
        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(popFile.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // 3. I-save ang file pisikal sa server
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await popFile.CopyToAsync(fileStream);
        }

        // 4. I-save ang path sa tamang database column name mo
        booking.ProofOfPaymentUrl = "/uploads/payments/" + uniqueFileName;
        
        // Optional: Pwede mo ring i-update ang status kung kinakailangan
        // booking.Status = "Pending Validation"; 

        _context.Update(booking);
        await _context.SaveChangesAsync();
    }

    TempData["Message"] = "Matagumpay na na-upload ang patunay ng bayad!";
    
    return RedirectToAction("ViewStatus", new { id = bookingId });
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


//REFUND REQUEST


    [HttpGet]
    [Route("Tour/RefundRequests")]
    public async Task<IActionResult> RefundRequests()
    {
        // Kunin ang lahat ng bookings na may refund request
        var refundBookings = await _context.Bookings
            .Include(b => b.Tour)
            .Where(b => b.RefundStatus == "Requested")
            .ToListAsync();

        return View("RefundRequests", refundBookings);
    }

[HttpPost]
[Route("Booking/SubmitRefundRequest")]
public async Task<IActionResult> SubmitRefundRequest(int bookingId, string refundReason, string payoutChannel, string notes)
{
    var booking = await _context.Bookings
        .Include(b => b.Tour)
        .FirstOrDefaultAsync(b => b.Id == bookingId);

    if (booking == null)
    {
        return NotFound();
    }

    booking.RefundStatus = "Requested";
    booking.RefundReason = refundReason;
    booking.RefundPayoutChannel = payoutChannel;
    booking.RefundNotes = notes;
    booking.RefundRequestedAt = DateTime.Now;

    _context.Update(booking);
    await _context.SaveChangesAsync();

    TempData["Message"] = "Matagumpay na naisumite ang iyong kahilingan sa refund!";
    
    return RedirectToAction("ViewStatus", new { id = bookingId });
}
}
