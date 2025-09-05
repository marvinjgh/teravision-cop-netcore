namespace Backend.Service.DataTransferObjects;

public class RefreshTokenDTO
{
    public long UserId { get; set; }
    public required string RefreshToken { get; set; }
}
