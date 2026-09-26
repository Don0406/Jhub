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
        if (b == null || b.Tour == null) return Json(new { success = false, message = "Record not found" });

        if (b.Status == "Validated") return Json(new { success = false, message = "Booking already validated" });

        if ((b.Tour.SlotsFilled + b.Slots) > b.Tour.TotalCapacity)
        {
            return Json(new { success = false, message = "Not enough slots available in this tour." });
        }

        b.Status = "Validated";
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
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    // --- NET PAYABLE ---
    [HttpGet]
    [Route("Tour/CalculateNetPayables")]
    public async Task<IActionResult> CalculateNetPayables()
    {
        var tours = await _context.Tours
            .Include(t => t.Bookings)
            .ToListAsync();

        decimal totalRevenue = tours.SelectMany(t => t.Bookings ?? new List<Booking>())
                                    .Where(b => b.Status == "Validated")
                                    .Sum(b => b.TotalAmount);

        decimal totalCosts = _context.Vendors
            .Where(v => v.TourId != null)
            .Sum(v => v.Payables);
        
        decimal netPayable = totalRevenue - totalCosts;

        return Json(new { 
            TotalRevenue = totalRevenue, 
            TotalOperationalCosts = totalCosts, 
            NetPayable = netPayable 
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

        ViewBag.TourStatuses = tourStatuses;
        return View("~/Views/Home/Operator-dashboard.cshtml", tours);
    }

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

        foreach (var t in tours)
        {
            t.SlotsFilled = t.Bookings != null 
                ? t.Bookings.Where(b => b.Status == "Validated").Sum(b => b.Slots) 
                : 0;
        }

        return View("~/Views/Home/Tour-management.cshtml", tours);
    }

    // --- ARCHIVE TOUR MANAGEMENT ---
    [HttpPost]
    [Route("Tour/ArchiveTour")]
    public async Task<IActionResult> ArchiveTour(int id)
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
            .Where(t => t.Status == "Archived")
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
            
            var selectedVendor = await _context.Vendors.FindAsync(firstSelectedVendorId);
            if (selectedVendor != null)
            {
                selectedVendor.TourId = newTour.Id;
                _context.Vendors.Update(selectedVendor);
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
