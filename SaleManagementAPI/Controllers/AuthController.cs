using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaleManagementAPI.Data;
using SaleManagementAPI.DTOs.Auth;
using SaleManagementAPI.Models;

namespace SaleManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthController(AppDbContext db, 
                        IPasswordHasher<User> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
        {
            if (ModelState.IsValid == false)
                return ValidationProblem(ModelState);

            var username = dto.UserName.Trim().ToLower();

            var exists = await _db.Users.AnyAsync(u => u.Username == username);

            if (exists)
                return Conflict(new
                {
                    message = "Username already exists."
                });

            var user = new User
            {
                Username = username,
                Role = "Staff",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var response = new RegisterResponseDTO
            {
                Id = user.Id,
                Username = username,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            return CreatedAtAction(nameof(Register), response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var username = dto.UserName.Trim().ToLower();

            //Kiểm tra xem username có tồn tại không
            var user = await _db.Users.FirstOrDefaultAsync(
                                            u => u.Username == username);
            if (user == null)
                return Unauthorized(new LoginResponseDTO
                {
                    Success = false,
                    Message = "Invalid Username."
                });

            if (!user.IsActive)
                return StatusCode(StatusCodes.Status403Forbidden,
                    new LoginResponseDTO
                    {
                        Success = false,
                        Message = "User is inactive."
                    });

            //Kiểm tra mật khẩu
            var verifyResult = _passwordHasher.VerifyHashedPassword(
                               user, user.PasswordHash, dto.Password);

            if (verifyResult == PasswordVerificationResult.Failed)
                return Unauthorized(new LoginResponseDTO
                {
                    Success = false,
                    Message = "Password is incorrect."
                });

            return Ok(new LoginResponseDTO
            {
                Success = true,
                Message = "Login succesful!"
            });
        }
    }
}
