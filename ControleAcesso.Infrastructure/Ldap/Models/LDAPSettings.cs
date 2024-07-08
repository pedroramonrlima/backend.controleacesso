namespace ControleAcesso.Infrastructure.Ldap.Models
{
    public class LDAPSettings
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string BaseDN { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string GroupAdmin { get; set; }
    }
}
