using ControleAcesso.Domain.Entities;

namespace ControleAcesso.Domain.Interfaces.Services
{
    public interface IEmployeeService : IGenericService<Employee>
    {
        Task<Employee?> GetEmployeeByLogin(string login);
        Task<bool> IsEmployeeGroupApproval(Employee employee);
        Task<bool> IsEmployeeManager(Employee employee);
    }
}
