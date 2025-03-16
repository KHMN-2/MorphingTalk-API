using Domain.Entities.Users;

namespace Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
    }
}
