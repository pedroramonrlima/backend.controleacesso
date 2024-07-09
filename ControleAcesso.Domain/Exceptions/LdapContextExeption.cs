namespace ControleAcesso.Domain.Exceptions
{
    public class LdapContextExeption : DomainException
    {
        public LdapContextExeption(string message, Dictionary<string, List<string>> errors):base(message,errors) { }
    }
}
