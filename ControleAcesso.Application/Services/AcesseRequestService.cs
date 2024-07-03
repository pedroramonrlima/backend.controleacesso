using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Enumerations;
using ControleAcesso.Domain.Exceptions;
using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Domain.Interfaces.Services;

namespace ControleAcesso.Application.Services
{
    public class AcesseRequestService : GenericService<AcesseRequest>, IAcesseRequestService
    {
        private Dictionary<string, List<string>> Errors = new Dictionary<string, List<string>>();
        private readonly IGenericService<Employee> _employeeService;
        private readonly IGenericRepository<AcesseRequestDetail> _acesseRequestDetailRepository;
        private readonly IAcesseRequestRepository _acesseRequestRepository;
        public AcesseRequestService(
            IGenericRepository<AcesseRequestDetail> acesseRequestDetailRepository,
            IGenericService<Employee> employeeService, 
            IGenericRepository<AcesseRequest> repository, 
            IAcesseRequestRepository acesseRequestRepository) : base(repository)
        {
            _employeeService = employeeService;
            _acesseRequestRepository = acesseRequestRepository;
            _acesseRequestDetailRepository = acesseRequestDetailRepository;
        }

        public override async Task<AcesseRequest> AddAsync(AcesseRequest entity)
        {
            await Validate(entity);
            

            AcesseRequestDetail acesseRequestDetail = new AcesseRequestDetail
            {
                RequesterEmployeeId = entity.EmployeeId,
                StatusRequestId = 1
            };

            entity = await _acesseRequestRepository.AddWithDetailsAsync(entity,acesseRequestDetail);
            
            return entity;
        }

        private async Task<bool> Validate(AcesseRequest entity)
        {
            await IsDepartamentManagerAsync(entity);
            await IsRquisicaoGroupExistAsync(entity);

            if (Errors.Count() > 0)
            {
                throw new DomainException("Ouve um ou mais erros ao tentar processar sua solicitação", Errors);
            }
            return true;
        }

        private async Task<bool> IsDepartamentManagerAsync(AcesseRequest entity)
        {
            var employee = await _employeeService.GetByIdAsync(entity.EmployeeId,NavigationLevel.FirstLevel);
            if (employee.Department.ManagerId == null)
            {
                Errors["Departament"] = ["Departamento está sem gestor associado"];
            }
            return true;
        }

        private async Task<bool> IsRquisicaoGroupExistAsync(AcesseRequest entity)
        {
            IEnumerable<AcesseRequestDetail> requests = await _acesseRequestDetailRepository.GetAllAsync(ad =>
                (ad.AcesseRequest.EmployeeId == entity.EmployeeId) &&
                (ad.StatusRequestId == 1 || ad.StatusRequestId == 2) &&
                (ad.AcesseRequest.GroupAdId == entity.GroupAdId),NavigationLevel.SecondLevel);
  

            if (requests.Count() > 0)
            {
                AcesseRequestDetail? firstRequest = requests.FirstOrDefault();
                string status = firstRequest!.Status.Name;
                int id = firstRequest.Id;
                string group = firstRequest!.AcesseRequest.GroupAd.Name;

                Errors["Group"] = [
                    string.Format(
                        ResponseMessages.AcesseRequestIsExists, id, group, status)
                    ];
            }


            return true;
        }
    }
}
