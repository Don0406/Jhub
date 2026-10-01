using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _UPDATED__FrontEnd_Capstone.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace _UPDATED__FrontEnd_Capstone.Controllers
{
    public class RefundController : Controller
    {
        private readonly AppDbContext _context;

        public RefundController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Refund/RedirectToRefunds")]
        public IActionResult RedirectToRefunds()
        {
            return RedirectToAction("RefundRequests");
        }

        [HttpGet]
        [Route("Refund/RefundRequests")]
        public async Task<IActionResult> RefundRequests()
        {
            var refundBookings = await _context.Bookings
                .Include(b => b.Tour)
                .Where(b => b.RefundStatus != null && b.RefundStatus != "")
                .OrderByDescending(b => b.RefundRequestedAt ?? b.BookingDate)
                .ToListAsync();

            return View("~/Views/User/RefundRequests.cshtml", refundBookings);
        }

[HttpPost]
[Route("Refund/RequestRefundFromQueue")]
public async Task<IActionResult> RequestRefundFromQueue(int bookingId, string refundReason, IFormFile paymentReceipt)
{
    var booking = await _context.Set<Booking>().Include(b => b.Tour).FirstOrDefaultAsync(x => x.Id == bookingId);
    if (booking == null) return Json(new { success = false, message = "Booking not found." });
    booking.IsArchived = true;
    // Kung galing sa Pending at nirefund/tinanggal, gawing "Rejected"
    if (booking.Status == "Pending Validation")
    {
        booking.Status = "Rejected";
    }
    // Kung galing sa Approved at nirefund, gawing "Validated" ayon sa iyong instruction
    else if (booking.Status == "Approved")
    {
        booking.Status = "Validated";
        if (booking.Tour != null)
        {
            // Ibalik ang slots dahil na-approve na ito dati at ngayon ay nirefund na
            booking.Tour.SlotsFilled = Math.Max(0, booking.Tour.SlotsFilled - booking.Slots);
        }
    }

    // Pwede mong i-save dito ang refundReason o receipt kung may mga field ka para rito sa database.
    await _context.SaveChangesAsync();

    return Json(new { success = true });
}

[HttpPost]
[Route("Tour/DeleteApprovedBooking")]
public async Task<IActionResult> DeleteApprovedBooking(int id)
{
    try
    {
        var booking = await _context.Bookings
            .Include(b => b.Tour)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return Json(new { success = false, message = "Booking not found." });
        }

        // Requirement 2: Return slots when deleted/removed from approved list
        if (booking.Tour != null && booking.Tour.SlotsFilled >= booking.Slots)
        {
            booking.Tour.SlotsFilled -= booking.Slots;
        }

        // Ensure status reflects as Validated in history
        booking.Status = "Validated"; 
        booking.RefundUpdatedAt = DateTime.Now;
        booking.IsArchived = true;

        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "Approved booking has been moved to history." });
    }

    catch (Exception ex)
    {
        return Json(new { success = false, message = "Error: " + ex.Message });
    }
}

[HttpPost]
[Route("Tour/CompleteTourBooking")]
public async Task<IActionResult> CompleteTourBooking(int id)
{
    try
    {
        var booking = await _context.Bookings
            .Include(b => b.Tour)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null)
        {
            return Json(new { success = false, message = "Booking not found." });
        }

        // Bawasan ang slots kapag tapos na ang tour
        if (booking.Tour != null && booking.Tour.SlotsFilled >= booking.Slots)
        {
            booking.Tour.SlotsFilled -= booking.Slots;
        }
        
        // Ilipat sa "Validated" para mawala sa Approved section pero manatili sa DB at lumabas sa Booking History
        booking.Status = "Validated";
        booking.RefundUpdatedAt = DateTime.Now;
        booking.IsArchived = true;

        await _context.SaveChangesAsync();

        return Json(new { success = true, message = "Tour completed and moved to history." });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = "Error: " + ex.Message });
    }
}


[HttpPost]
[Route("Refund/SubmitRefundRequest")]
public async Task<IActionResult> SubmitRefundRequest(int bookingId, string refundReason, string payoutChannel, string? notes)
{
    var booking = await _context.Bookings
        .Include(b => b.Tour) // Siguraduhing kasama ito para hindi mag-null ang Tour properties sa HTML mo
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

    await _context.SaveChangesAsync();

    TempData["Message"] = "Matagumpay na naisumite ang iyong bagong mensahe o kahilingan sa refund.";
    
    // Kunin din ang slotsFilled para hindi mag-error ang math sa HTML mo (kung ginagamit ito sa ViewBag)
    // Palitan mo na lang ng query kung paano mo kinukuha ang slotsFilled sa ibang part ng code mo.
    int slotsFilled = await _context.Bookings.CountAsync(b => b.TourId == booking.TourId && (b.Status == "Validated" || b.Status == "Confirmed"));
    ViewBag.DynamicSlotsFilled = slotsFilled;

    // Direktang i-render ang iyong Booking-status.cshtml nang walang redirection!
    return View("~/Views/Booking/Booking-status.cshtml", booking);
}
 
        [HttpPost]
        [Route("Refund/ProcessRefundOperator")]
        public async Task<IActionResult> ProcessRefundOperator(int bookingId, string refundStatus, string operatorMessage, IFormFile? refundProofFile)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                return Json(new { success = false, message = "Booking not found." });
            }

            booking.RefundStatus = refundStatus;
            booking.RefundOperatorMessage = operatorMessage;
            booking.RefundUpdatedAt = DateTime.Now;
            booking.IsArchived = true;

            if (refundProofFile != null && refundProofFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "refunds");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(refundProofFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await refundProofFile.CopyToAsync(fileStream);
                }

                booking.RefundProofPath = "/uploads/refunds/" + uniqueFileName;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Refund request updated successfully." });
        }
    }
}
