using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleManagementAPI.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = default!;

    // Lưu HASH, không lưu plain text
    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = default!;

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "Staff"; // Admin, Staff...

    [Required]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
