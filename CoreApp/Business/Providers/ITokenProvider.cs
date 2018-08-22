using System.Threading.Tasks;

namespace Business.Providers
{
    public interface ITokenProvider
    {
        Task Save(string refreshToken, string accessTokenSignature, string userName);
        Task Delete(string refreshToken, string accessToken);
        Task Revoke(string userName);
    }
}