using System.ComponentModel.DataAnnotations;

namespace SaleManagementAPI.DTOs.Auth
{
    public class RegisterRequestDTO
    {
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; } = default!;

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string Password { get; set; } = default!;
    }
}
