namespace SaleManagementAPI.DTOs.Auth
{
    public class LoginResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;

        public string Token { get; set; }
    }
}
