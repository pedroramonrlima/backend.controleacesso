using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Domain.Models.Ldap;

namespace ControleAcesso.Application.Services
{
    public class LdapService : ILdapService
    {
        private readonly ILdapManagerRepository _ldapManagerRepository;

        public LdapService(ILdapManagerRepository ldapManagerRepository)
        {
            _ldapManagerRepository = ldapManagerRepository;
        }

        public void AddUser(Employee employee)
        {            
            throw new NotImplementedException();
        }

        public void AddUserToGroup(string samAccountName, string dnGroup)
        {
            LdapUser user = _ldapManagerRepository.GetUserSamAccountName(samAccountName);

            LdapGroup group = new LdapGroup
            {
                DN = dnGroup,
            };

            _ldapManagerRepository.AddUserToGroup(user,group);
        }

        public string GetGroupAdminApi()
        {
            return _ldapManagerRepository.GroupAdmin;
        }

        public IEnumerable<LdapGroup> GetLdapGroups()
        {
            return _ldapManagerRepository.GetLdapGroups();
        }

        public IEnumerable<LdapUser> GetLDAPUsers()
        {
            return _ldapManagerRepository.GetLDAPUsers();
        }

        public LdapUser GetUserBySamAccountName(string samAccountName)
        {
            return _ldapManagerRepository.GetUserSamAccountName(samAccountName);
        }

        public bool SamAccountNameExists(string samAccountName)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(LdapUser user)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserPassword(string samAccountName, string newPassword)
        {
            throw new NotImplementedException();
        }

        public bool ValidUserPassowrd(string login, string password)
        {
            LdapUser user = _ldapManagerRepository.GetUserSamAccountName(login);
            if(user != null)
            {
                return _ldapManagerRepository.ValidUserPassowrd(user.DN, password);
            }
            return false;
        }
    }
}
