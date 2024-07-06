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
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        private readonly IGenericService<Employee> _employeeService;
        private readonly IGenericService<GroupAd> _groupService;
        private readonly IGenericRepository<AcesseRequestDetail> _acesseRequestDetailRepository;
        private readonly IAcesseRequestRepository _acesseRequestRepository;
        public AcesseRequestService(
            IGenericService<GroupAd> groupService,
            IGenericRepository<AcesseRequestDetail> acesseRequestDetailRepository,
            IGenericService<Employee> employeeService, 
            IGenericRepository<AcesseRequest> repository, 
            IAcesseRequestRepository acesseRequestRepository) : base(repository)
        {
            _employeeService = employeeService;
            _acesseRequestRepository = acesseRequestRepository;
            _acesseRequestDetailRepository = acesseRequestDetailRepository;
            _groupService = groupService;
        }

        public override async Task<AcesseRequest> AddAsync(AcesseRequest entity)
        {
            await ValidateCreate(entity);
            

            AcesseRequestDetail acesseRequestDetail = new AcesseRequestDetail
            {
                RequesterEmployeeId = entity.EmployeeId,
                StatusRequestId = 1
            };

            entity = await _acesseRequestRepository.AddWithDetailsAsync(entity,acesseRequestDetail);
            
            return entity;
        }

        public override async Task<AcesseRequest> UpdateAsync(AcesseRequest entity)
        {
            await ValidateUpdate(entity);

            return await base.UpdateAsync(entity);
        }

        private async Task ValidateUpdate(AcesseRequest entity)
        {
            await IsDepartamentManagerAsync(entity);
            await IsGroupExistsAsync(entity);

            if (_errors.Count() > 0)
            {
                throw new DomainException("Ouve um ou mais erros ao tentar processar sua solicitação", _errors);
            }
        }

        private async Task ValidateCreate(AcesseRequest entity)
        {
            await IsDepartamentManagerAsync(entity);
            await IsRquisicaoGroupExistAsync(entity);
            await IsGroupExistsAsync(entity);

            if (_errors.Count() > 0)
            {
                throw new DomainException("Ouve um ou mais erros ao tentar processar sua solicitação", _errors);
            }
        }

        private async Task IsDepartamentManagerAsync(AcesseRequest entity)
        {
            try
            {
                var employee = await _employeeService.GetByIdAsync(entity.EmployeeId, NavigationLevel.FirstLevel);
                if (employee.Department.ManagerId == null)
                {
                    AddError(nameof(employee.Department), string.Format(ResponseMessages.DepartamentNotManager,employee.Department.Name));
                }
            }catch (DomainException ex)
            {
                AddError(nameof(entity.EmployeeId), ResponseMessages.DataNotFound);
            }
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

                AddError(nameof(entity.GroupAd), string.Format(
                        ResponseMessages.AcesseRequestIsExists, id, group, status));
            }


            return true;
        }

        private async Task IsGroupExistsAsync(AcesseRequest entity)
        {
            try
            {
                await _groupService.GetByIdAsync(entity.GroupAdId);
            }
            catch (DomainException ex)
            {
                AddError(nameof(entity.GroupAd), ResponseMessages.DataNotFound);
            }
        }

        private void AddError(string key, string message)
        {
            if (!_errors.ContainsKey(key))
            {
                _errors[key] = [message];
            }else
            {
                _errors[key].Add(message);
            }

            
        }
    }
}
