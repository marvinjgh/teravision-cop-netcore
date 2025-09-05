using Backend.Service.DataTransferObjects;
using Backend.Service.Models;

namespace Backend.Service.Contracts;

public interface IAuthServices
{
    Task<UserEntity?> ResgisterAsync(UserDTO request);
    Task<TokenResponseDTO?> LoginAsync(UserDTO request);
    Task<TokenResponseDTO?> RefreshTokenAsync(RefreshTokenDTO request);
}
