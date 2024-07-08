using System.DirectoryServices.Protocols;

namespace ControleAcesso.Infrastructure.Ldap.Interfaces
{
    public interface ILdapConnectionContext
    {
        LdapConnection GetLdapConnection();

        LdapConnection GetLdapConnection(string userDN, string password);

        string BaseDN { get; }
    }
}
