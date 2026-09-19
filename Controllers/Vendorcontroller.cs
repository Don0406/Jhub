using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _UPDATED__FrontEnd_Capstone.Models;
using System.Threading.Tasks;
using System.Globalization;

namespace _UPDATED__FrontEnd_Capstone.Controllers;

public class VendorController : Controller
{
    private readonly AppDbContext _context;
    public VendorController(AppDbContext context) { _context = context; }

    [HttpGet]
    [Route("Vendor/Management")]
    public async Task<IActionResult> Management()
    {
        var vendors = await _context.Vendors.Include(v => v.Tour).ToListAsync();
        return View("~/Views/VendorManagement/Vendor-management.cshtml", vendors);
    }

    [HttpPost]
    [Route("Vendor/Create")]
    public async Task<IActionResult> Create(string vendorName, string serviceType, string contactInfo, string paymentTerms, string payables, string status)
    {
        // I-parse ang string na may commas papuntang decimal
        decimal parsedPayables = decimal.Parse(payables.Replace(",", ""), CultureInfo.InvariantCulture);

        var vendor = new Vendor { 
            VendorName = vendorName, 
            ServiceType = serviceType, 
            ContactInfo = contactInfo, 
            PaymentTerms = paymentTerms, 
            Payables = parsedPayables,
            Status = status 
        };

        _context.Vendors.Add(vendor);
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Management));
    }

    [HttpPost]
[Route("Vendor/Delete")]
public async Task<IActionResult> Delete(int id)
{
    // Siguraduhing kasama sa 'Include' yung Tour property
    var vendor = await _context.Vendors.Include(v => v.Tour).FirstOrDefaultAsync(v => v.Id == id);
    
    if (vendor != null)
    {
        // Kung ang 'Tour' ay iisang object lang (hindi listahan)
        if (vendor.Tour != null)
        {
            vendor.Tour.VendorId = null; // I-unlink ang tour
        }
        
        _context.Vendors.Remove(vendor);
        await _context.SaveChangesAsync();
    }
    return RedirectToAction(nameof(Management));
}
}
