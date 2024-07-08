using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Domain.Models.Ldap;
using ControleAcesso.Infrastructure.Ldap.Interfaces;
using System.DirectoryServices.Protocols;
using System.Text.RegularExpressions;

namespace ControleAcesso.Infrastructure.Ldap.Repositories
{
    public class LdapManagerRepository : ILdapManagerRepository
    {
        private readonly ILdapConnectionContext _ldapConnectionContext;

        public LdapManagerRepository(ILdapConnectionContext ldapConnectionContext)
        {
            _ldapConnectionContext = ldapConnectionContext;
        }

        public void AddUser(LdapUser user)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LdapUser> GetLDAPUsers()
        {
            List<LdapUser> users = new List<LdapUser>();
            try
            {
                using (var ldapConnection = _ldapConnectionContext.GetLdapConnection())
                {
                    SearchRequest searchRequest = new SearchRequest(
                        _ldapConnectionContext.BaseDN,
                        "(&(objectClass=user)(!(objectClass=computer)))",
                        SearchScope.Subtree,
                        null

                    );

                    SearchResponse searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);

                    foreach (SearchResultEntry entry in searchResponse.Entries)
                    {
                        users.Add(MapUserModel(entry));
                    }
                }

                return users;
            }
            catch (LdapException e)
            {
                Console.WriteLine($"Erro ao conectar ao servidor LDAP: {e.Message}");
                throw new ArgumentException($"erro ao tentar se conectar ao servidor LDAP: {e.Message}");
            }
        }

        public Task<IEnumerable<LdapUser>> GetLDAPUsersAsync()
        {
            throw new NotImplementedException();
        }

        public LdapUser GetUserSamAccountName(string samAccountName)
        {
            throw new NotImplementedException();
        }

        public bool SamAccountNameExists(string samAccountName)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserPassword(string samAccountName, string newPassword)
        {
            throw new NotImplementedException();
        }

        private LdapUser MapUserModel(SearchResultEntry entry)
        {
            LdapUser user = new LdapUser
            {
                DN = entry.DistinguishedName,
                Name = GetAttributeValue(entry, "name"),
                Mail = GetAttributeValue(entry, "UserPrincipalName"),
                SamAccountName = GetAttributeValue(entry, "SamAccountName"),
                Manager = GetManagerFromDn(GetAttributeValue(entry, "Manager")),
                Company = GetAttributeValue(entry, "Company"),
                Title = GetAttributeValue(entry, "Title"),
                PhysicalDeliveryOfficeName = GetAttributeValue(entry, "physicalDeliveryOfficeName"),
                IPPhone = GetAttributeValue(entry, "ipPhone"),
                Pager = GetAttributeValue(entry, "pager"),
                Department = GetAttributeValue(entry, "Department"),
                Initials = GetAttributeValue(entry, "initials"),
                UserPrincipalName = GetAttributeValue(entry, "UserPrincipalName")
            };

            return user;

        }

        private string GetAttributeValue(SearchResultEntry entry, string attributeName)
        {
            DirectoryAttribute attribute = entry.Attributes[attributeName];

            if (attribute != null && attribute.Count > 0)
            {
                return attribute[0]?.ToString() ?? "";
            }

            return "";
        }

        private string GetManagerFromDn(string dn)
        {
            string pattern = @"CN=([^,]+)";
            Match match = Regex.Match(dn, pattern);
            return match.Success ? match.Groups[1].Value : "";
        }
    }
}
