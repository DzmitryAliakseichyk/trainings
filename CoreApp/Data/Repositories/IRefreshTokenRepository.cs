using System.Threading.Tasks;
using Data.Models;

namespace Data.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> Create(string refreshToken, string username);
        Task Remove(string refreshToken);
    }
}