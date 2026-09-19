using System.ComponentModel.DataAnnotations;

namespace _UPDATED__FrontEnd_Capstone.Models;

public class User
{
    [Key] 
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Sex { get; set; } = string.Empty;

    [Required]
    public int Age { get; set; }

    [Required]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = "joiner"; 

    public string Status { get; set; } = "Active";
}
