using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _UPDATED__FrontEnd_Capstone.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace _UPDATED__FrontEnd_Capstone.Controllers;

public class TourController : Controller
{
    private readonly AppDbContext _context;
    private readonly GeminiService _geminiService;
    public TourController(AppDbContext context, GeminiService geminiService)
    {
        _context = context;
        _geminiService = geminiService; 
    }
    
    // --- BOOKING QUEUE ---
    [HttpGet]
    [Route("User/BookingQueue")]
    public async Task<IActionResult> BookingQueueProcess()
    {
        var bookings = await _context.Set<Booking>()
                                     .Include(b => b.Tour)
                                     .OrderByDescending(b => b.BookingDate)
                                     .ToListAsync();
        
        return View("~/Views/User/Booking-queue.cshtml", bookings);
    }
    
  [HttpGet]
  [Route("User/BookingHistory")]
public async Task<IActionResult> BookingHistory()
{
    var bookings = await _context.Set<Booking>()
                            .Include(b => b.Tour)
        
                            .Where(b => b.Status == "Validated" || b.Status == "Rejected")
                            .OrderByDescending(b => b.BookingDate)
                            .ToListAsync();
    
    return View("~/Views/User/Booking-history.cshtml", bookings);
}

    // --- DELETE SINGLE BOOKING FROM HISTORY ---
    [HttpGet]
    [Route("Tour/DeleteBooking/{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _context.Set<Booking>().FindAsync(id);
        if (booking != null)
        {
            _context.Set<Booking>().Remove(booking);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(BookingHistory));
    }

    // --- DELETE ALL BOOKINGS FROM HISTORY ---
    [HttpGet]
    [Route("Tour/DeleteAllBookings")]
    public async Task<IActionResult> DeleteAllBookings()
    {
        var historyBookings = await _context.Set<Booking>()
            .Where(b => b.Status == "Validated" || b.Status == "Rejected")
            .ToListAsync();

        if (historyBookings.Any())
        {
            _context.Set<Booking>().RemoveRange(historyBookings);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(BookingHistory));
    }

[HttpPost]
[Route("User/ApproveBooking")]
public async Task<IActionResult> ApproveBooking(int id)
{
    var b = await _context.Set<Booking>().Include(b => b.Tour).FirstOrDefaultAsync(x => x.Id == id);
    if (b == null || b.Tour == null) 
        return Json(new { success = false, message = "Record not found" });

    if (b.Status == "Validated" || b.Status == "Approved") 
        return Json(new { success = false, message = "Booking is already approved or validated." });

    if ((b.Tour.SlotsFilled + b.Slots) > b.Tour.TotalCapacity)
    {
        return Json(new { success = false, message = "Not enough slots available in this tour." });
    }

    b.Status = "Validated";
    b.IsApproved = true;
    b.Tour.SlotsFilled += b.Slots;

    await _context.SaveChangesAsync();
    return Json(new { success = true });
}
 

[HttpPost]
[Route("User/RejectBooking")]
public async Task<IActionResult> RejectBooking(int id)
{
    var b = await _context.Set<Booking>().Include(b => b.Tour).FirstOrDefaultAsync(x => x.Id == id);
    if (b == null) return Json(new { success = false });

    if (b.Status == "Validated" && b.Tour != null)
    {
        b.Tour.SlotsFilled -= b.Slots;
    }

    b.Status = "Rejected";
    b.IsApproved = false;
    
    await _context.SaveChangesAsync();
    return Json(new { success = true });
}
    // --- TOTAL PAYABLE ---
    [HttpGet]
[Route("Tour/CalculateNetPayables")]
public async Task<IActionResult> CalculateNetPayables()
{
    var activeTours = await _context.Tours
        .Where(t => t.Status != "Archived" && !t.IsComplete && !t.IsCancelled)
        .ToListAsync();

    decimal totalOperationalCosts = activeTours.Sum(t => t.OperationalCost);

    return Json(new { 
        TotalOperationalCosts = totalOperationalCosts 
    });
}
    
    // --- USER MANAGEMENT ---
    [HttpGet]
    [Route("User/UserManagement")]
    public async Task<IActionResult> UserManagement()
    {
        return View("~/Views/User/User-management.cshtml", await _context.Users.ToListAsync());
    }
    
    [HttpPost]
    [Route("User/CreateUser")]
    public async Task<IActionResult> CreateUser(string fullName, string email, string role)
    {
        _context.Users.Add(new User { 
            FullName = fullName, Email = email, Role = role, Status = "Active", Password = "defaultpassword123" 
        });
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(UserManagement));
    }

    [HttpPost]
    [Route("User/DeleteUser")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var u = await _context.Users.FindAsync(id);
        if (u != null) { _context.Users.Remove(u); await _context.SaveChangesAsync(); }
        return RedirectToAction(nameof(UserManagement));
    }

    [HttpPost]
    [Route("User/EditUser")]
    public async Task<IActionResult> EditUser(int id, string fullName, string email, string role)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.FullName = fullName; user.Email = email; user.Role = role;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(UserManagement));
    }

    // --- TOUR MANAGEMENT ---
    [HttpGet]
    [Route("Home/OperatorDashboard")]
    public async Task<IActionResult> OperatorDashboard() 
    {
        var tours = await _context.Tours
            .Include(t => t.Bookings) 
            .AsNoTracking()
            .ToListAsync();
   
        foreach (var t in tours)
        {
            int totalSlotsUsed = t.Bookings != null 
                ? t.Bookings.Where(b => b.Status == "Validated").Sum(b => b.Slots) 
                : 0;

            t.SlotsFilled = totalSlotsUsed;
        }

        var tourStatuses = tours.ToDictionary(t => t.Id, t => {
            double occ = t.TotalCapacity > 0 ? (double)t.SlotsFilled / t.TotalCapacity : 0;
            if (t.Status == "Cancelled") return "NO-GO";
            if (occ >= 0.9) return "FULL";
            if (occ >= 0.5) return "GO";
            return "WATCH";
        });

        // --- CORRECTED CANCELLATION RISK METRICS ---
        // Low: 1% to 35%
        // Medium: 36% to 70%
        // High: 71% to 100% (Kasama na rito ang 3 out of 4 o 75%)
        var activeTours = tours.Where(x => x.Status != "Archived" && !x.IsCancelled).ToList();

        var lowRiskList = new List<string>();
        var medRiskList = new List<string>();
        var highRiskList = new List<string>();

        foreach (var active in activeTours)
        {
            string primaryKeyword = !string.IsNullOrEmpty(active.TourName) 
                ? active.TourName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? active.Destination 
                : active.Destination;

            if (string.IsNullOrEmpty(primaryKeyword)) continue;

            var matchingTours = tours.Where(t => 
                (!string.IsNullOrEmpty(t.TourName) && t.TourName.StartsWith(primaryKeyword, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(t.Destination) && t.Destination.Equals(primaryKeyword, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            int totalWithKeyword = matchingTours.Count;
            int cancelledCount = matchingTours.Count(t => t.IsCancelled);

            if (totalWithKeyword > 0)
            {
                double cancelRatio = (double)cancelledCount / totalWithKeyword;
                int percent = (int)Math.Round(cancelRatio * 100);

                // Low Risk: 1% to 35%
                if (cancelRatio > 0 && cancelRatio <= 0.35)
                {
                    lowRiskList.Add($"{active.TourName} ({cancelledCount}/{totalWithKeyword} cancelled - {percent}%)");
                }
                // Medium Risk: 36% to 70%
                else if (cancelRatio > 0.35 && cancelRatio <= 0.70)
                {
                    medRiskList.Add($"{active.TourName} ({cancelledCount}/{totalWithKeyword} cancelled - {percent}%)");
                }
                // High Risk: 71% to 100% (Pasok na rito ang 75% o 3 out of 4)
                else if (cancelRatio > 0.70)
                {
                    highRiskList.Add($"{active.TourName} ({cancelledCount}/{totalWithKeyword} cancelled - {percent}%)");
                }
            }
        }

        ViewBag.TourStatuses = tourStatuses;
        ViewBag.LowRiskList = lowRiskList;
        ViewBag.MedRiskList = medRiskList;
        ViewBag.HighRiskList = highRiskList;
        ViewBag.HasAnyRisk = lowRiskList.Any() || medRiskList.Any() || highRiskList.Any();

        return View("~/Views/Home/Operator-dashboard.cshtml", tours);
    }

    
 // --- TOUR MANAGEMENT EDITING ---
[HttpGet]
[Route("Tour/Management")]
public async Task<IActionResult> Management(string? search, string? status)
{
    var q = _context.Tours.AsQueryable();

    q = q.Where(t => t.Status != "Archived");

    if (!string.IsNullOrEmpty(search)) 
        q = q.Where(t => t.TourName.Contains(search) || t.Destination.Contains(search));
    
    if (!string.IsNullOrEmpty(status) && status != "All Statuses") 
        q = q.Where(t => t.Status == status);

    q = q.OrderByDescending(t => t.IsBoosted).ThenByDescending(t => t.Id);

    var tours = await q.Include(t => t.Vendor)
                        .Include(t => t.Bookings)
                        .ToListAsync();

    var vendorPayablesMap = new Dictionary<int, decimal>();

    foreach (var t in tours)
    {
        // Gamitin muna kung ano talaga ang naka-save na OperationalCost sa Tour table
        decimal totalCost = t.OperationalCost;

        // Kung sakaling 0 pero may vendor payables, saka lang natin kunin sa Vendor table
        if (totalCost == 0)
        {
            if (t.Vendor != null)
            {
                totalCost += t.Vendor.Payables;
            }

            var linkedVendorsSum = await _context.Vendors
                .Where(v => v.TourId == t.Id)
                .SumAsync(v => (decimal?)v.Payables) ?? 0;

            totalCost += linkedVendorsSum;
        }

        if (totalCost > 0)
        {
            vendorPayablesMap[t.Id] = totalCost;
        }

        t.SlotsFilled = t.Bookings != null 
            ? t.Bookings.Where(b => b.Status == "Validated").Sum(b => b.Slots) 
            : 0;
    }
    
    ViewBag.VendorPayablesMap = vendorPayablesMap;

    return View("~/Views/Home/Tour-management.cshtml", tours);
}

    // --- ARCHIVE TOUR MANAGEMENT ---
    [HttpPost]
    [Route("Tour/MarkAsComplete")]
public async Task<IActionResult> MarkAsComplete(int id)
{
    var t = await _context.Tours.FindAsync(id);
    if (t != null)
    {
        t.Status = "Archived";
        t.ArchivedAt = DateTime.Now; 
        await _context.SaveChangesAsync();
    }
    return Redirect(Url.Action("Management", "Tour") ?? "/Tour/Management");
}

[HttpPost]
[Route("Tour/RestoreTour")]
public async Task<IActionResult> RestoreTour(int id)
{
    var t = await _context.Tours.FindAsync(id);
    if (t != null)
    {
        t.Status = "Active"; 
        t.ArchivedAt = null; 
        await _context.SaveChangesAsync();
    }
    return Redirect(Url.Action("ArchivedTours", "Tour") ?? "/Tour/ArchivedTours");
}


//Permanently delete all
    [HttpPost]
    [Route("Tour/DeleteAllArchivedTours")]
    public async Task<IActionResult> DeleteAllArchivedTours()
    {
        var archivedTours = await _context.Tours
            .Where(t => t.Status == "Completed" || t.Status == "Archived")
            .ToListAsync();

        if (archivedTours.Any())
        {
            _context.Tours.RemoveRange(archivedTours);
            await _context.SaveChangesAsync();
        }
        return Redirect(Url.Action("ArchivedTours", "Tour") ?? "/Tour/ArchivedTours");
    }

// --- PERMANENTLY DELETE ARCHIVED TOUR ---
    [HttpPost]
    [Route("Tour/DeleteArchivedTour")]
    public async Task<IActionResult> DeleteArchivedTour(int id)
    {
        var t = await _context.Tours.FindAsync(id);
        if (t != null)
        {
            _context.Tours.Remove(t);
            await _context.SaveChangesAsync();
        }
        return Redirect(Url.Action("ArchivedTours", "Tour") ?? "/Tour/ArchivedTours");
    }
    
    [HttpGet]
    [Route("Tour/ArchivedTours")]
    public async Task<IActionResult> ArchivedTours()
    {
    var archivedTours = await _context.Tours
        .Where(t => t.Status == "Completed" || t.Status == "Archived")
        .Include(t => t.Vendor)
        .Include(t => t.Bookings)
        .ToListAsync();

    bool changesMade = false;

    foreach (var t in archivedTours.ToList())
    {
        if (t.ArchivedAt.HasValue)
        {
            var daysInArchive = (DateTime.Now.Date - t.ArchivedAt.Value.Date).Days;
            if (daysInArchive >= 365)
            {
                _context.Tours.Remove(t);
                archivedTours.Remove(t);
                changesMade = true;
            }
        }
    }

    if (changesMade)
    {
        await _context.SaveChangesAsync();
    }

    foreach (var t in archivedTours)
    {
        t.SlotsFilled = t.Bookings != null 
            ? t.Bookings.Where(b => b.Status == "Validated").Sum(b => b.Slots) 
            : 0;
    }

    return View("~/Views/Home/Archived-tours.cshtml", archivedTours);
}

 



    // --- CREATE TOUR ---
    [HttpGet("Tour/Create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Vendors = await _context.Vendors
            .Select(v => new {
                Id = v.Id,
                VendorName = v.VendorName,
                ServiceType = v.ServiceType
            })
            .ToListAsync();
            
        return View("~/Views/Home/Create-tour.cshtml");
    }

   [HttpPost("Tour/Create")]
    public async Task<IActionResult> Create(
        string tourName, 
        string destination, 
        string? landscapeType,      
        string? intensityLevel,     
        string? activities,         
        string? shortDescription,   
        DateTime departureDate, 
        DateTime returnDate, 
        decimal basePrice, 
        int totalCapacity, 
        List<int> vendorIds, 
        string? itineraryHighlights, 
        IFormFile imageFile)
    {
        string filePath = "/images/default.jpg";

        if (imageFile != null && imageFile.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var path = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            filePath = "/images/" + fileName;
        }

        // --- KUNIN ANG SUM NG PAYABLES NG MGA NAPILING VENDORS ---
        decimal totalOperationalCost = 0;
        if (vendorIds != null && vendorIds.Count > 0)
        {
            totalOperationalCost = await _context.Vendors
                .Where(v => vendorIds.Contains(v.Id))
                .SumAsync(v => (decimal?)v.Payables) ?? 0;
        }

        var newTour = new Tour { 
            TourName = tourName, 
            Destination = destination, 
            LandscapeType = landscapeType,      
            IntensityLevel = intensityLevel,    
            Activities = activities,            
            Description = shortDescription,     
            DepartureDate = departureDate, 
            ReturnDate = returnDate, 
            BasePrice = basePrice, 
            OperationalCost = totalOperationalCost, // <--- Dito naisama ang sum ng payables bilang operational cost
            TotalCapacity = totalCapacity, 
            ImageUrl = filePath, 
            ItineraryHighlights = itineraryHighlights, 
            Status = "Active",
            IsBoosted = false 
        };

        _context.Tours.Add(newTour);
        await _context.SaveChangesAsync();

        if (vendorIds != null && vendorIds.Count > 0)
        {
            int firstSelectedVendorId = vendorIds.FirstOrDefault();

            newTour.VendorId = firstSelectedVendorId;
            _context.Tours.Update(newTour);
            
            // I-update ang mga vendors para ma-link sa TourId na ito
            foreach (var vId in vendorIds)
            {
                var vendor = await _context.Vendors.FindAsync(vId);
                if (vendor != null)
                {
                    vendor.TourId = newTour.Id;
                    _context.Vendors.Update(vendor);
                }
            }

            await _context.SaveChangesAsync();
        }

        return Redirect(Url.Action("Management", "Tour") ?? "/Tour/Management");
    }
    
    [HttpPost]
    [Route("Tour/UpdateFull")]
    public async Task<IActionResult> UpdateFull(int id, string tourName, string destination, DateTime departureDate, DateTime returnDate, decimal basePrice, int totalCapacity, string status, string? itineraryHighlights)
    {
        var t = await _context.Tours.FindAsync(id);
        if (t != null)
        {
            t.TourName = tourName; t.Destination = destination; t.DepartureDate = departureDate; t.ReturnDate = returnDate; t.BasePrice = basePrice; t.TotalCapacity = totalCapacity; t.Status = status; t.ItineraryHighlights = itineraryHighlights;
            await _context.SaveChangesAsync();
        }
        return Redirect(Url.Action("Management", "Tour") ?? "/Tour/Management");
    }

    [HttpPost]
    [Route("Tour/DeleteTour")]
    public async Task<IActionResult> DeleteTour(int id)
    {
        var t = await _context.Tours.FindAsync(id);
        if (t != null)
        {
            _context.Tours.Remove(t);
            await _context.SaveChangesAsync();
        }
        return Redirect(Url.Action("Management", "Tour") ?? "/Tour/Management");
    }
    
    [HttpGet]
    [Route("Tour/VendorManagement")]
    public IActionResult VendorManagement() => RedirectToAction("Management", "Vendor");

    // --- BOOST TOUR FEATURE ENDPOINT ---
    [HttpPost]
    [Route("Tour/ToggleBoost")]
    public async Task<IActionResult> ToggleBoost(int id)
    {
        var tour = await _context.Tours.FindAsync(id);
        if (tour != null)
        {
            tour.IsBoosted = !tour.IsBoosted; 
            tour.BoostedAt = tour.IsBoosted ? DateTime.Now : null;
            await _context.SaveChangesAsync();
        }
        return Redirect(Url.Action("Management", "Tour") ?? "/Tour/Management");
    }




    // --- CANCEL TOUR FEATURE ENDPOINT ---
    [HttpPost]
    [Route("Tour/CancelTour")]
    public async Task<IActionResult> CancelTour(int id)
    {
        var tour = await _context.Tours.FindAsync(id);
        if (tour != null)
        {
            tour.IsCancelled = true;
            tour.CancelledAt = DateTime.Now;
            
            // Move directly to archive and update status
            tour.Status = "Archived";
            tour.ArchivedAt = DateTime.Now;
            
            await _context.SaveChangesAsync();
        }
        return Redirect(Url.Action("Management", "Tour") ?? "/Tour/Management");
    }

    
    // --- AI GENERATION ENDPOINT ---
    [HttpPost]
    [Route("Tour/GenerateTourWithAI")]
    public async Task<IActionResult> GenerateTourWithAI([FromBody] PromptDto request)
    {
        if (string.IsNullOrEmpty(request?.Prompt))
        {
            return BadRequest(new { success = false, message = "Wala kang inilagay na prompt." });
        }

        try
        {
            var jsonResult = await _geminiService.GenerateTourDataAsync(request.Prompt);
            
            if (string.IsNullOrEmpty(jsonResult))
            {
                return StatusCode(500, new { success = false, message = "Nabigong kumuha ng data mula kay Gemini." });
            }

            return Content(jsonResult, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    public class PromptDto
    {
        public string Prompt { get; set; }
    }
}
