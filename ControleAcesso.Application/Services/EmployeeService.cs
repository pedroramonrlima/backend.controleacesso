using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Enumerations;
using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Domain.Interfaces.Services;

namespace ControleAcesso.Application.Services
{
    public class EmployeeService : GenericService<Employee>, IEmployeeService
    {
        private IGenericRepository<GroupApproval> _groupApprovalRepository;
        private IGenericRepository<Manager> _managerRepository;
        public EmployeeService(
            IGenericRepository<Manager> managerRepository,
            IGenericRepository<GroupApproval> groupApprovalRepository,
            IGenericRepository<Employee> repository) : base(repository)
        {
            _groupApprovalRepository = groupApprovalRepository;
            _managerRepository = managerRepository;
        }

        public Task<Employee?> GetEmployeeByLogin(string login)
        {
            return _repository.GetAsync(ep => ep.Login == login,NavigationLevel.FirstLevel);
        }

        public async Task<bool> IsEmployeeGroupApproval(Employee employee) 
        {
            var groupApproval = await _groupApprovalRepository.GetAllAsync(ga => ga.EmployeeId == employee.Id);

            if (groupApproval.Count() > 0) 
            {
                return true;
            }else
            {
                return false;
            }
        }

        public async Task<bool> IsEmployeeManager(Employee employee)
        {
            var manager = await _managerRepository.GetAllAsync(ga => ga.EmployeeId == employee.Id);

            if (manager.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
