using ControleAcesso.Domain.Models.Token;

namespace ControleAcesso.Domain.Interfaces.Repositories
{
    public interface IJwtRepository
    {
        string GenerateToken(string group, string login, string name);
        string GenerateToken(int id, string login, string name, string departament, string manager, string title);
        string GenerateToken(Token token);
    }
}
