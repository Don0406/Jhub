using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _UPDATED__FrontEnd_Capstone.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace _UPDATED__FrontEnd_Capstone.Controllers;

public class PredictiveAnalyticsController : Controller
{
    private readonly AppDbContext _context;

    public PredictiveAnalyticsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("Analytics/BookingVelocity/{tourId}")]
    public async Task<IActionResult> GetBookingVelocity(int tourId)
    {
        // 1. Kunin ang active tour na tinitingnan ngayon sa dashboard
        var activeTour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == tourId);
        if (activeTour == null)
        {
            return NotFound("Tour not found.");
        }

        string keyword = activeTour.Destination ?? activeTour.TourName;

        // 2. Hanapin ang lahat ng mga Archived tours na may parehong keyword
        var archivedTours = await _context.Tours
            .Where(t => t.Status == "Archived" && 
                        (t.Destination.Contains(keyword) || t.TourName.Contains(keyword)))
            .Include(t => t.Bookings)
            .ToListAsync();

        if (!archivedTours.Any())
        {
            return Json(new { 
                tourId = tourId,
                keyword = keyword,
                analyzedArchivedCount = 0,
                bookingRate = "No historical data",
                averageSlotsPerDay = 0
            });
        }

        double totalVelocitySum = 0;
        int validCount = 0;

        foreach (var archived in archivedTours)
        {
            // Kunin ang mga approved bookings ng archived tour
            var approvedBookings = archived.Bookings?.Where(b => b.IsApproved).OrderBy(b => b.BookedAt).ToList();
            
            if (approvedBookings != null && approvedBookings.Any())
            {
                var firstBookingDate = approvedBookings.First().BookedAt;

                // FIX: Kung null ang SlotsFilledAt sa DB, gamitin ang huling booking date o kaya ang CreatedAt/Archived date para hindi mag-null!
                DateTime filledDate;
                if (archived.SlotsFilledAt.HasValue)
                {
                    filledDate = archived.SlotsFilledAt.Value;
                }
                else
                {
                    // Fallback: Kunin ang petsa ng huling booking o ang pinakahuli sa listahan
                    filledDate = approvedBookings.Last().BookedAt;
                    
                    // Kung pareho silang naging saktong magkasabay, bigyan natin ng at least 1 day span para pwedeng makompyut
                    if (filledDate <= firstBookingDate)
                    {
                        filledDate = firstBookingDate.AddDays(1);
                    }
                }

                double daysActive = (filledDate - firstBookingDate).TotalDays;
                if (daysActive < 1) daysActive = 1; // Iwas division by zero

                // Gamitin ang totoong slots filled o i-sum ang approved bookings kung zero man ang slotsfilled
                int totalSlots = archived.SlotsFilled > 0 ? archived.SlotsFilled : approvedBookings.Sum(b => b.Slots);

                double velocity = totalSlots / daysActive;
                totalVelocitySum += velocity;
                validCount++;
            }
        }

        double comparativeBookingRate = validCount > 0 ? totalVelocitySum / validCount : 0;

        // SAFETY CAP: Hindi pwedeng lumampas sa kasalukuyang Total Capacity ng active tour ang velocity
        if (comparativeBookingRate > activeTour.TotalCapacity)
        {
            comparativeBookingRate = activeTour.TotalCapacity;
        }

        return Json(new { 
            tourId = tourId,
            keyword = keyword,
            analyzedArchivedCount = validCount,
            bookingRate = Math.Round(comparativeBookingRate, 2) + " slots/day",
            averageSlotsPerDay = Math.Round(comparativeBookingRate, 2)
        });
    }
}
