using Backend.Service.Contracts;
using Backend.Service.DataTransferObjects;
using Backend.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IRepositoryWrapper repository, IConfiguration configuration) : ControllerBase
    {

        [HttpPost("register")]
        [ProducesResponseType<UserEntity>(StatusCodes.Status200OK)]
        [ProducesResponseType<ErrorDTO>(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserDTO request)
        {

            var users = await repository.UserRepository.GetAllUsers(u => u.Username == request.Username);

            if (users.Any())
            {
                return BadRequest(new ErrorDTO { Message = "Username already exist." });
            }

            var user = new UserEntity();
            var hashedPassword = new PasswordHasher<UserEntity>().HashPassword(user, request.Password);

            user.Username = request.Username;
            user.PasswordHash = hashedPassword;

            repository.UserRepository.CreateUser(user);

            await repository.Save();

            return Ok(user);
        }

        [HttpPost("login")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        [ProducesResponseType<string>(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserDTO request)
        {
            if (request == null)
            {
                return BadRequest(new ErrorDTO { Message = "Empty request" });
            }

            var users = await repository.UserRepository.GetAllUsers(u => u.Username == request.Username);

            if (!users.Any())
            {
                return BadRequest(new ErrorDTO { Message = "Invalid Username or Password." });
            }

            var user = users.First();
            if (new PasswordHasher<UserEntity>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return BadRequest(new ErrorDTO { Message = "Invalid Username or Password." });
            }

            var token = new TokenResponseDTO
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user)
            };

            if (token is null || token.RefreshToken is null || token.AccessToken is null)
            {
                return BadRequest("Invalid Username or Password.");
            }

            return Ok(token);
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType<ErrorDTO>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ErrorDTO>(StatusCodes.Status401Unauthorized)]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(RefreshTokenDTO request)
        {

            var user = await repository.UserRepository.GetUserById(request.UserId);
            if (user is null || user.RefreshToken != request.RefreshToken || user.ResfreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest(new ErrorDTO { Message = "Invalid data." });
            }

            var token = new TokenResponseDTO
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshToken(user)
            };

            if (token is null || token.RefreshToken is null || token.AccessToken is null)
            {
                return Unauthorized(new ErrorDTO { Message = "Invalid refresh token," });
            }

            return Ok(token);
        }

        [Authorize]
        [HttpGet("auth")]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("You are authenticated");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are an admin");
        }

        private string CreateToken(UserEntity user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDesc = new JwtSecurityToken(
                    issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                    audience: configuration.GetValue<string>("AppSettings:Audience"),
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(5),
                    signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDesc);
        }
        private async Task<string> GenerateAndSaveRefreshToken(UserEntity user)
        {
            var resfreshToken = GenerateRefreshToken();
            user.RefreshToken = resfreshToken;
            user.ResfreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await repository.Save();
            return resfreshToken;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
