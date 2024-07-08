using ControleAcesso.Domain.Models.Ldap;

namespace ControleAcesso.Domain.Interfaces.Repositories
{
    public interface ILdapManagerRepository
    {
        IEnumerable<LdapUser> GetLDAPUsers();
        LdapUser GetUserSamAccountName(string samAccountName);
        void AddUser(LdapUser user);
        bool SamAccountNameExists(string samAccountName);
        void UpdateUserPassword(string samAccountName, string newPassword);
        Task<IEnumerable<LdapUser>> GetLDAPUsersAsync();
    }
}
