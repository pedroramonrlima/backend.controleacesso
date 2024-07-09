using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Domain.Models.Ldap;
using ControleAcesso.Infrastructure.Ldap.Enumerations;
using ControleAcesso.Infrastructure.Ldap.Interfaces;
using System.DirectoryServices.Protocols;
using System.Text;
using System.Text.RegularExpressions;

namespace ControleAcesso.Infrastructure.Ldap.Repositories
{
    public class LdapManagerRepository : ILdapManagerRepository
    {
        private readonly ILdapConnectionContext _ldapConnectionContext;
        private const string UsersContainer = "cn=Users";

        public LdapManagerRepository(ILdapConnectionContext ldapConnectionContext)
        {
            _ldapConnectionContext = ldapConnectionContext;
        }

        public void AddUser(LdapUser user)
        {
            using (var ldapConnection = _ldapConnectionContext.GetLdapConnection())
            {
                var newUserDN = $"cn={user.CN},{UsersContainer},{_ldapConnectionContext.BaseDN}";
                int userControl = (int)UserAccountControlFlags.PASSWORD_EXPIRED;
                var newEntry = new AddRequest(newUserDN, new DirectoryAttribute[]
                {
                    new DirectoryAttribute("objectClass", "user"),
                    new DirectoryAttribute("cn", user.CN),
                    new DirectoryAttribute("givenName", user.GiveName),
                    new DirectoryAttribute("displayName", user.CN),
                    new DirectoryAttribute("name", user.Name),
                    new DirectoryAttribute("sn", user.SN),
                    new DirectoryAttribute("SamAccountName", user.SamAccountName),
                    new DirectoryAttribute("Title", user.Title),
                    new DirectoryAttribute("userPrincipalName", user.UserPrincipalName),
                    new DirectoryAttribute("mail", user.Mail),
                    new DirectoryAttribute("initials", user.Initials),
                    new DirectoryAttribute("ipPhone", user.IPPhone),
                    new DirectoryAttribute("pager", user.Pager),
                    new DirectoryAttribute("Department", user.Department),
                    new DirectoryAttribute("userAccountControl", userControl.ToString()),
                    new DirectoryAttribute("unicodePwd", Encoding.Unicode.GetBytes($"\"{user.Password}\""))
                });

                try
                {
                    ldapConnection.SendRequest(newEntry);
                }
                catch (LdapException e)
                {
                    throw new Exception($"Erro ao adicionar novo usuário: {e.Message}");
                }
            }
        }

        public IEnumerable<LdapUser> GetLDAPUsers()
        {
            using (var ldapConnection = _ldapConnectionContext.GetLdapConnection())
            {
                var searchRequest = new SearchRequest(
                    _ldapConnectionContext.BaseDN,
                    "(&(objectClass=user)(!(objectClass=computer)))",
                    SearchScope.Subtree,
                    null
                );

                try
                {
                    var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);
                    return searchResponse.Entries.Cast<SearchResultEntry>().Select(MapUserModel).ToList();
                }
                catch (LdapException e)
                {
                    throw new Exception($"Erro ao obter usuários LDAP: {e.Message}", e);
                }
            }
        }

        public LdapUser GetUserSamAccountName(string samAccountName)
        {
            using (var ldapConnection = _ldapConnectionContext.GetLdapConnection())
            {
                var searchRequest = new SearchRequest(
                    _ldapConnectionContext.BaseDN,
                    $"(&(objectClass=user)(samAccountName={samAccountName}))",
                    SearchScope.Subtree,
                    null
                );

                try
                {
                    var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);
                    if (searchResponse.Entries.Count > 0)
                    {
                        return MapUserModel(searchResponse.Entries[0]);
                    }
                    return null;
                }
                catch (LdapException e)
                {
                    throw new Exception($"Erro ao obter usuário pelo samAccountName: {e.Message}", e);
                }
            }
        }

        public bool SamAccountNameExists(string samAccountName)
        {
            using (var ldapConnection = _ldapConnectionContext.GetLdapConnection())
            {
                var searchRequest = new SearchRequest(
                    _ldapConnectionContext.BaseDN,
                    $"(&(objectClass=user)(samAccountName={samAccountName}))",
                    SearchScope.Subtree,
                    null
                );

                try
                {
                    var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);
                    return searchResponse.Entries.Count > 0;
                }
                catch (LdapException e)
                {
                    throw new Exception($"Erro ao verificar existência do samAccountName: {e.Message}", e);
                }
            }
        }

        public void UpdateUserPassword(string samAccountName, string newPassword)
        {
            using (var ldapConnection = _ldapConnectionContext.GetLdapConnection())
            {
                var user = GetUserSamAccountName(samAccountName);

                if (user == null)
                {
                    throw new Exception($"Usuário com o samAccountName '{samAccountName}' não encontrado.");
                }

                var modifyRequest = new ModifyRequest(
                    user.DN,
                    DirectoryAttributeOperation.Replace,
                    "unicodePwd",
                    Encoding.Unicode.GetBytes($"\"{newPassword}\"")
                );

                try
                {
                    ldapConnection.SendRequest(modifyRequest);
                }
                catch (LdapException e)
                {
                    throw new Exception($"Erro ao atualizar senha do usuário: {e.Message}", e);
                }
            }
        }

        public bool ValidUserPassowrd(string dn, string password)
        {
            bool isValid = false;
            using (var ldapConnection = _ldapConnectionContext.GetLdapConnection(dn, password))
            {
                isValid = true;
            }
            return isValid;
        }

        public void AddUserToGroup(LdapUser user, LdapGroup group)
        {
            using (var ldapConnection = _ldapConnectionContext.GetLdapConnection())
            {
                // Verifique se o usuário já é membro do grupo
                var searchRequest = new SearchRequest(group.DN, $"(member={user.DN})", SearchScope.Base, null);
                var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);

                if (searchResponse.Entries.Count == 0)
                {
                    // O usuário não é membro do grupo, então adicione-o
                    var modifyRequest = new ModifyRequest(group.DN, DirectoryAttributeOperation.Add, "member", user.DN);
                    ldapConnection.SendRequest(modifyRequest);
                }
                else
                {
                    // O usuário já é membro do grupo
                    Console.WriteLine("O usuário já é membro do grupo.");
                }
            }
        }

        public IEnumerable<LdapGroup> GetLdapGroups()
        {
            using (var ldapConnection = _ldapConnectionContext.GetLdapConnection())
            {
                var searchRequest = new SearchRequest(
                    _ldapConnectionContext.BaseDN,
                    "(&(objectClass=group))",
                    SearchScope.Subtree,
                    null
                );

                try
                {
                    var searchResponse = (SearchResponse)ldapConnection.SendRequest(searchRequest);
                    return searchResponse.Entries.Cast<SearchResultEntry>().Select(MapGroupModel).ToList();
                }
                catch (LdapException e)
                {
                    throw new Exception($"Erro ao obter grupos LDAP: {e.Message}", e);
                }
            }
        }
        private LdapGroup MapGroupModel(SearchResultEntry entry)
        {
            return new LdapGroup
            {
                DN = entry.DistinguishedName,
                CN = GetAttributeValue(entry, "cn"),
                Description = GetAttributeValue(entry, "description")
                //Members = GetAttributeValues(entry, "member")
            };
        }

        private LdapUser MapUserModel(SearchResultEntry entry)
        {
            return new LdapUser
            {
                DN = entry.DistinguishedName,
                Name = GetAttributeValue(entry, "name"),
                Mail = GetAttributeValue(entry, "mail"),
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
        }

        private string GetAttributeValue(SearchResultEntry entry, string attributeName)
        {
            if (entry.Attributes.Contains(attributeName))
            {
                var attribute = entry.Attributes[attributeName];
                return attribute.Count > 0 ? attribute[0].ToString() : string.Empty;
            }
            return string.Empty;
        }

        private string GetManagerFromDn(string dn)
        {
            var match = Regex.Match(dn, @"CN=([^,]+)");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}
