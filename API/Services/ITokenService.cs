using API.Entities;

namespace API.Services
{
    public interface ITokenService
    {
        Task<string> CreateToken(Member user);
    }
}
