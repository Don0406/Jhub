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

    public TourController(AppDbContext context) { _context = context; }

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

    [HttpPost]
    [Route("User/ApproveBooking")]
    public async Task<IActionResult> ApproveBooking(int id)
    {
        var b = await _context.Set<Booking>()
            .Include(x => x.Tour)
            .ThenInclude(t => t!.Bookings)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (b == null || b.Tour == null)
            return Json(new { success = false, message = "Record not found" });

        if (b.Status == "Validated")
            return Json(new { success = false, message = "Booking already validated" });

        // Recompute from live Validated bookings — do NOT trust cached SlotsFilled
        int validatedSlots = b.Tour.Bookings
            .Where(x => x.Status == "Validated" && x.Id != b.Id)
            .Sum(x => x.Slots);

        if (validatedSlots + b.Slots > b.Tour.TotalCapacity)
            return Json(new { success = false, message = "Not enough slots available in this tour." });

        b.Status = "Validated";
        b.Tour.SlotsFilled = validatedSlots + b.Slots;
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpPost]
    [Route("User/RejectBooking")]
    public async Task<IActionResult> RejectBooking(int id)
    {
        var b = await _context.Set<Booking>()
            .Include(x => x.Tour)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (b == null) return Json(new { success = false });

        if (b.Status == "Validated" && b.Tour != null)
        {
            b.Tour.SlotsFilled -= b.Slots;
            if (b.Tour.SlotsFilled < 0) b.Tour.SlotsFilled = 0;
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
            .Include(t => t.Vendor)
            .ToListAsync();

        decimal totalRevenue = 0m;
        decimal totalCosts = 0m;

        foreach (var t in tours)
        {
            var validatedBookings = t.Bookings?
                .Where(b => b.Status == "Validated")
                .ToList() ?? new List<Booking>();

            // Skip tours with no validated revenue
            if (validatedBookings.Count == 0) continue;

            decimal revenue = validatedBookings.Sum(b => b.TotalAmount);
            decimal cost = t.Vendor?.Payables ?? 0m;

            totalRevenue += revenue;
            totalCosts += cost;
        }

        decimal netPayable = totalRevenue - totalCosts;

        return Json(new
        {
            totalRevenue,
            totalOperationalCosts = totalCosts,
            netPayable
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
            FullName = fullName,
            Email = email,
            Role = role,
            Status = "Active",
            PhoneNumber = "N/A",
            Sex = "N/A",
            Age = 18,
            Password = "defaultpassword123"
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
            t.SlotsFilled = t.Bookings != null
                ? t.Bookings
                    .Where(b => b.Status == "Validated" || b.Status == "Pending Validation")
                    .Sum(b => b.Slots)
                : 0;
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

        if (!string.IsNullOrEmpty(search))
            q = q.Where(t => t.TourName.Contains(search) || t.Destination.Contains(search));

        if (!string.IsNullOrEmpty(status) && status != "All Statuses")
            q = q.Where(t => t.Status == status);

        var tours = await q.Include(t => t.Vendor)
                           .Include(t => t.Bookings)
                           .ToListAsync();

        foreach (var t in tours)
        {
            t.SlotsFilled = t.Bookings != null
                ? t.Bookings
                    .Where(b => b.Status == "Validated" || b.Status == "Pending Validation")
                    .Sum(b => b.Slots)
                : 0;
        }

        return View("~/Views/Home/Tour-management.cshtml", tours);
    }

    // --- CREATE TOUR ---
    [HttpGet("Tour/Create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Vendors = await _context.Vendors.ToListAsync();
        return View("~/Views/Home/Create-tour.cshtml");
    }

    [HttpPost("Tour/Create")]
    public async Task<IActionResult> Create(
        string tourName,
        string destination,
        DateTime departureDate,
        DateTime returnDate,
        decimal basePrice,
        int totalCapacity,
        List<int> vendorIds,
        string? itineraryHighlights,
        string? shortDescription,
        IFormFile imageFile)
    {
        string filePath = "/images/default.jpg";
        if (imageFile != null && imageFile.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var path = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            filePath = "/images/" + fileName;
        }

        // Auto-fill itinerary from short description if empty
        if (string.IsNullOrWhiteSpace(itineraryHighlights) &&
            !string.IsNullOrWhiteSpace(shortDescription))
        {
            itineraryHighlights = GenerateItineraryFromDescription(shortDescription, destination);
        }

        var newTour = new Tour {
            TourName = tourName,
            Destination = destination,
            DepartureDate = departureDate,
            ReturnDate = returnDate,
            BasePrice = basePrice,
            TotalCapacity = totalCapacity,
            ImageUrl = filePath,
            ItineraryHighlights = itineraryHighlights,
            Status = "Active",
            SlotsFilled = 0
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

        return RedirectToAction("Management");
    }

    private string GenerateItineraryFromDescription(string shortDesc, string destination)
    {
        var cleaned = shortDesc.Trim().TrimEnd('.');
        var parts = cleaned
            .Split(new[] { ',', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => p.Length > 0)
            .ToList();

        if (parts.Count == 0)
            parts.Add($"{destination} sightseeing");

        return string.Join("; ", parts);
    }

    [HttpPost]
    [Route("Tour/UpdateFull")]
    public async Task<IActionResult> UpdateFull(int id, string tourName, string destination,
        DateTime departureDate, DateTime returnDate, decimal basePrice,
        int totalCapacity, string status, string? itineraryHighlights)
    {
        var t = await _context.Tours.FindAsync(id);
        if (t != null)
        {
            t.TourName = tourName; t.Destination = destination;
            t.DepartureDate = departureDate; t.ReturnDate = returnDate;
            t.BasePrice = basePrice; t.TotalCapacity = totalCapacity;
            t.Status = status; t.ItineraryHighlights = itineraryHighlights;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Management");
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
        return RedirectToAction("Management");
    }

    // --- BOOST TOGGLE ---
    [HttpPost]
    [Route("Tour/ToggleBoost")]
    public async Task<IActionResult> ToggleBoost(int id)
    {
        var t = await _context.Tours.FindAsync(id);
        if (t == null) return Json(new { success = false, message = "Tour not found" });

        t.IsBoosted = !t.IsBoosted;
        t.BoostedUntil = t.IsBoosted ? DateTime.Now.AddDays(7) : (DateTime?)null;
        await _context.SaveChangesAsync();

        return Json(new { success = true, isBoosted = t.IsBoosted });
    }

    [HttpGet]
    [Route("Tour/VendorManagement")]
    public IActionResult VendorManagement() => RedirectToAction("Management", "Vendor");
}
