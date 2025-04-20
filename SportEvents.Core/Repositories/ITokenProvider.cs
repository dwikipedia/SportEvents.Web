namespace SportEvents.Domain.Repositories
{
    public interface ITokenProvider
    {
        string GetToken();
        void SetToken(string token);
        bool IsAuthenticated();
    }
}
