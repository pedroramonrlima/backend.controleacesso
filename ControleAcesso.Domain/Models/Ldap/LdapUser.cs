namespace ControleAcesso.Domain.Models.Ldap
{
    public class LdapUser
    {
        public string Name { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string CN { get; set; } = string.Empty;
        public string GiveName { get; set; } = string.Empty;
        public string SN { get; set; } = string.Empty;
        public string SamAccountName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string UserPrincipalName { get; set; } = string.Empty;
        public string IPPhone { get; set; } = string.Empty;
        public string Pager { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty;
        public string DN { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string PhysicalDeliveryOfficeName { get; set; } = string.Empty;
        public string MemberOf { get; set; } = string.Empty;
    }
}
