using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Enumerations;
using ControleAcesso.Domain.Exceptions;
using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Domain.Models.Ldap;
using ControleAcesso.Domain.Models.Token;
using System.Text.RegularExpressions;

namespace ControleAcesso.Application.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IJwtRepository _jwtRepository;
        private readonly ILdapService _ldapService;
        private readonly IEmployeeService _employeeService;
        private readonly IGenericService<Manager> _managerService;
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public AuthenticateService(
            IGenericService<Manager> managerService,
            IEmployeeService employeeService,
            IJwtRepository jwtRepository, 
            ILdapService ldapService)
        {
            _jwtRepository = jwtRepository;
            _ldapService = ldapService;
            _employeeService = employeeService;
            _managerService = managerService;
        }

        public async Task<string> AuthenticateAsync(string login, string password)
        {
            UserPasswordValid(login,password);
            IsAdmin(login);

            if (_errors.Any())
            {
                throw new DomainException(ResponseMessages.ErrorValidate, _errors);
            }


            return _jwtRepository.GenerateToken(await GenerateObjectToken(login));
        }

        private async Task<Token> GenerateObjectToken(string login)
        {
            Employee employee = await _employeeService.GetEmployeeByLogin(login);
            Manager manager = await _managerService.GetByIdAsync((int)employee.Department.ManagerId,NavigationLevel.FirstLevel);
            Token token = new Token
            {
                EmployeeId = employee.Id,
                Login = employee.Login,
                Departament = employee.Department.Name,
                Name = employee.Name,
                Manager = manager.Employee.Name,
                Title = employee.Title.Name,
                IsAdmin = IsAdmin(login),
                IsSpecialist = await _employeeService.IsEmployeeGroupApproval(employee),
                IsManager = await _employeeService.IsEmployeeManager(employee)
            };

            return token;
        }
        private bool UserPasswordValid(string login, string password)
        {
            if (_ldapService.ValidUserPassowrd(login, password))
            {
                return true;
            }else
            {
                AddError(nameof(UserPasswordValid), "Usuário ou Senha incorretos");
                return false;
            }
        }

        private bool IsAdmin(string login)
        {
            LdapUser user = _ldapService.GetUserBySamAccountName(login);

            if(user == null) return false;

            // Expressão regular para extrair o texto "Domain Admin"
            string input = user.MemberOf;
            string pattern = @"CN=(" + _ldapService.GetGroupAdminApi() + ")";

            // Encontrar todas as correspondências
            MatchCollection matches = Regex.Matches(input, pattern);
            if (matches.Count() > 0 && _ldapService.GetGroupAdminApi() == matches[0].Groups[1].Value.ToString())
            {
                return true;
            }
            else
            {
                //AddError(nameof(IsAdmin), "Usuário não tem permissão para entrar no sistema");
                return false;
            }
        }

        private void AddError(string key, string message)
        {
            if (!_errors.ContainsKey(key))
            {
                _errors[key] = new List<string> { message };
            }
            else
            {
                _errors[key].Add(message);
            }
        }
    }
}
