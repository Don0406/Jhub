using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _UPDATED__FrontEnd_Capstone.Models;

public class Booking
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string ReferenceNumber { get; set; } = string.Empty;
    
    [Required]
    public int TourId { get; set; }
    
    [Required]
    public string LeadName { get; set; } = string.Empty;
    
    [Required]
    public int LeadAge { get; set; }
    
    [Required]
    public string LeadEmail { get; set; } = string.Empty;
    
    [Required]
    public string LeadPhone { get; set; } = string.Empty;
    
    [Required]
    public string SpecialCategory { get; set; } = string.Empty;
    
    public string? CompanionNames { get; set; }

    [Required]
    public int Slots { get; set; } 
    
    [Required]
    public decimal TotalAmount { get; set; }
    
    [Required]
    public string Status { get; set; } = "Pending Validation";
    
    public DateTime BookingDate { get; set; } = DateTime.Now;

    public string? ProofOfPaymentUrl { get; set; }
    
    [ForeignKey("TourId")]
    public virtual Tour? Tour { get; set; }

    
}
