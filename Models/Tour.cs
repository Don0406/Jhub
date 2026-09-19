using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _UPDATED__FrontEnd_Capstone.Models;

public class Tour
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string TourName { get; set; } = string.Empty;

    [Required]
    public string Destination { get; set; } = string.Empty;

    [Required]
    public DateTime DepartureDate { get; set; }

    [Required]
    public DateTime ReturnDate { get; set; }

    public string? Description { get; set; }

    [Required]
    public decimal BasePrice { get; set; }

    [Required]
    public decimal OperationalCost { get; set; }

    [Required]
    public int TotalCapacity { get; set; }

    public int SlotsFilled { get; set; } = 0;

    [Required]
    public int MinParticipants { get; set; }

    [Required]
    public DateTime Deadline { get; set; }

    public string? VehicleAssignment { get; set; }
    public string? AccommodationAssignment { get; set; }
    public string? ImageUrl { get; set; }
    public string? ItineraryHighlights { get; set; }

    public string Status { get; set; } = "Pending";

    // ===== BOOST / FEATURED =====
    public bool IsBoosted { get; set; } = false;
    public DateTime? BoostedUntil { get; set; }
    // ============================

    public int? VendorId { get; set; }

    [ForeignKey("VendorId")]
    public virtual Vendor? Vendor { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
