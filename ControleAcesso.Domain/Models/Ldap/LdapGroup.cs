namespace ControleAcesso.Domain.Models.Ldap
{
    public class LdapGroup
    {
        public string Name { get; set; } = string.Empty;

        public string DN { get; set; } = string.Empty;

        public string CN { get; set; } = string.Empty;
    }
}
