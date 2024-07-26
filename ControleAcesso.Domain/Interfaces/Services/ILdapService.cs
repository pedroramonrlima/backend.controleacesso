using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Models.Ldap;

namespace ControleAcesso.Domain.Interfaces.Services
{
    public interface ILdapService
    {
        void AddUser(Employee employee);
        IEnumerable<LdapUser> GetLDAPUsers();
        LdapUser GetUserBySamAccountName(string samAccountName);
        bool SamAccountNameExists(string samAccountName);
        void UpdateUserPassword(string samAccountName, string newPassword);
        void UpdateUser(LdapUser user);
        void AddUserToGroup(string dnUser, string dnGroup);
        bool ValidUserPassowrd(string login, string password);
        IEnumerable<LdapGroup> GetLdapGroups();
        string GetGroupAdminApi();
    }
}
