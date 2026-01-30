namespace SaleManagementAPI.DTOs.Auth
{
    public class RegisterResponseDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string Role { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
