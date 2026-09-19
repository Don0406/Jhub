using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _UPDATED__FrontEnd_Capstone.Models
{
    public class Vendor
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string VendorName { get; set; } = string.Empty;
        
        [Required]
        public string ServiceType { get; set; } = string.Empty;
        
        [Required]
        public string ContactInfo { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "LONGTEXT")] 
        public string PaymentTerms { get; set; } = string.Empty;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Payables { get; set; } = 0.00m;
        
        public string Status { get; set; } = "Active";

        public int? TourId { get; set; }
        


        [ForeignKey("TourId")]
        public virtual Tour? Tour { get; set; }
    }
}
