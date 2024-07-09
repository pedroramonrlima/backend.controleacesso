using ControleAcesso.Infrastructure.Ldap.Interfaces;
using ControleAcesso.Infrastructure.Ldap.Models;
using Microsoft.Extensions.Options;
using System.DirectoryServices.Protocols;
using System.Net;

namespace ControleAcesso.Infrastructure.Ldap.Connection
{
    public class LdapConnectionContext : ILdapConnectionContext
    {
        private readonly LDAPSettings _ldapSettings;

        public LdapConnectionContext(IOptions<LDAPSettings> ldapSettings)
        {
            _ldapSettings = ldapSettings.Value;
        }
        public LdapConnection GetLdapConnection()
        {
            var ldapConnection = new LdapConnection(new LdapDirectoryIdentifier(_ldapSettings.Host, _ldapSettings.Port));
            ldapConnection.SessionOptions.SecureSocketLayer = true;
            ldapConnection.SessionOptions.VerifyServerCertificate = (conn, cert) => true; // Permitir qualquer certificado
            ldapConnection.SessionOptions.ProtocolVersion = 3; // LDAPv3
            ldapConnection.SessionOptions.SecureSocketLayer = false;
            ldapConnection.AuthType = AuthType.Basic;
            NetworkCredential credential = new NetworkCredential(_ldapSettings.User, _ldapSettings.Password);
            ldapConnection.Bind(credential);
            return ldapConnection;
        }

        public LdapConnection GetLdapConnection(string userDN, string password)
        {
            var ldapConnection = new LdapConnection(new LdapDirectoryIdentifier(_ldapSettings.Host, _ldapSettings.Port));
            ldapConnection.SessionOptions.SecureSocketLayer = true;
            ldapConnection.SessionOptions.VerifyServerCertificate = (conn, cert) => true; // Permitir qualquer certificado
            ldapConnection.SessionOptions.ProtocolVersion = 3; // LDAPv3
            ldapConnection.SessionOptions.SecureSocketLayer = false;
            ldapConnection.AuthType = AuthType.Basic;
            NetworkCredential credential = new NetworkCredential(userDN, password);
            ldapConnection.Bind(credential);
            return ldapConnection;

        }
        public string BaseDN => _ldapSettings.BaseDN;
    }
}
