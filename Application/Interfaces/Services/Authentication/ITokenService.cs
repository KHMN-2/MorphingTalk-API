using Domain.Entities.Users;

namespace Application.Interfaces.Services.Authentication
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(User user);
        Task<string> GenerateRefreshToken();
        Task<User> GetUserFromToken(string token);
    }
}
