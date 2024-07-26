namespace ControleAcesso.Domain.Interfaces.Services
{
    public interface IAuthenticateService
    {
        Task<string> AuthenticateAsync(string login, string password);
    }
}
