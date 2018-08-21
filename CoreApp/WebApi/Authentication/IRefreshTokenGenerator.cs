namespace WebApi.Authentication
{
    public interface IRefreshTokenGenerator
    {
        string Generate();
    }
}