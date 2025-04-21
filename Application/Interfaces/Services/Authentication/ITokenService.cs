using Domain.Entities.Users;

namespace Application.Interfaces.Services.Authentication
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        Task<User> GetUserFromToken(string token);
    }
}
