using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Exceptions;
using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Domain.Enumerations;
using ControleAcesso.Domain.Models.AcesseRequestModel;

namespace ControleAcesso.Application.Services
{
    public class AcesseRequestService : GenericService<AcesseRequest>, IAcesseRequestService
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        private readonly IGenericService<Employee> _employeeService;
        private readonly IGroupService _groupService;
        private readonly IAcesseRequestDetailService _acesseRequestDetailService;
        private readonly IAcesseRequestRepository _acesseRequestRepository;

        public AcesseRequestService(
            IGroupService groupService,
            IAcesseRequestDetailService acesseRequestDetailService,
            IGenericService<Employee> employeeService,
            IGenericRepository<AcesseRequest> repository,
            IAcesseRequestRepository acesseRequestRepository) : base(repository)
        {
            _employeeService = employeeService;
            _acesseRequestRepository = acesseRequestRepository;
            _groupService = groupService;
            _acesseRequestDetailService = acesseRequestDetailService;
        }

        public override async Task<AcesseRequest> AddAsync(AcesseRequest entity)
        {
            await ValidateCreate(entity);

            AcesseRequestDetail acesseRequestDetail = new AcesseRequestDetail
            {
                RequesterEmployeeId = entity.EmployeeId,
                StatusRequestId = 1
            };

            entity.HasPriorApproval = await ValidGroupApproval(entity);

            entity = await _acesseRequestRepository.AddWithDetailsAsync(entity, acesseRequestDetail);

            return entity;
        }

        public override async Task<AcesseRequest> UpdateAsync(AcesseRequest entity)
        {
            await ValidateUpdate(entity);
            return await base.UpdateAsync(entity);
        }

        public Task<IEnumerable<AcesseRequestDetail>> GetAllAcesseRequestDetailAsync(int employeeId)
        {
            return _acesseRequestDetailService.GetRequestByEmployeeIdAsync(employeeId);
        }

        private async Task ValidateUpdate(AcesseRequest entity)
        {
            await IsDepartamentManagerAsync(entity);
            await IsGroupExistsAsync(entity);

            if (_errors.Any())
            {
                var validationErrors = _errors.SelectMany(
                    kvp => kvp.Value.Select(message => new ValidationError { Key = kvp.Key, Message = message })
                ).ToList();

                throw new DomainException("Houve um ou mais erros ao tentar processar sua solicitação", validationErrors);
            }
        }

        private async Task ValidateCreate(AcesseRequest entity)
        {
            await IsDepartamentManagerAsync(entity);
            await IsRquisicaoGroupExistAsync(entity);
            await IsGroupExistsAsync(entity);

            if (_errors.Any())
            {
                var validationErrors = _errors.SelectMany(
                    kvp => kvp.Value.Select(message => new ValidationError { Key = kvp.Key, Message = message })
                ).ToList();
                _errors.Clear();
                throw new DomainException("Houve um ou mais erros ao tentar processar sua solicitação", validationErrors);
            }
        }

        private async Task IsDepartamentManagerAsync(AcesseRequest entity)
        {
            try
            {
                var employee = await _employeeService.GetByIdAsync(entity.EmployeeId, NavigationLevel.FirstLevel);
                if (employee.Department.ManagerId == null)
                {
                    AddError(nameof(employee.Department), string.Format(ResponseMessages.DepartamentNotManager, employee.Department.Name));
                }
            }
            catch (DomainException)
            {
                AddError(nameof(entity.EmployeeId), ResponseMessages.DataNotFound);
            }
        }

        private async Task<bool> IsRquisicaoGroupExistAsync(AcesseRequest entity)
        {
            IEnumerable<AcesseRequestDetail> requests = await _acesseRequestDetailService.GetPendingRequestsByEmployeeAndGroupAsync(entity.EmployeeId, entity.GroupAdId);

            if (requests.Any())
            {
                AcesseRequestDetail firstRequest = requests.First();
                string status = firstRequest.Status.Name;
                int id = firstRequest.Id;
                string group = firstRequest.AcesseRequest.GroupAd.Name;

                AddError(nameof(entity.GroupAd), string.Format(ResponseMessages.AcesseRequestIsExists, id, group, status));
            }

            return true;
        }

        private async Task IsGroupExistsAsync(AcesseRequest entity)
        {
            try
            {
                await _groupService.GetByIdAsync(entity.GroupAdId);
            }
            catch (DomainException)
            {
                AddError(nameof(entity.GroupAd), ResponseMessages.DataNotFound);
            }
        }

        private async Task<bool> ValidGroupApproval(AcesseRequest entity)
        {
            return await _groupService.IsGroupApproval(entity.GroupAdId);
        }

        private void AddError(string key, string message)
        {
            if (!_errors.ContainsKey(key))
            {
                _errors[key] = new List<string>();
            }
            _errors[key].Add(message);
        }

        public async Task<AcesseRequestResult> AddAsync(IEnumerable<GroupAd> entities, int employeeId)
        {
            var result = new AcesseRequestResult();

            foreach (var groupAd in entities)
            {
                var acesseRequest = new AcesseRequest
                {
                    EmployeeId = employeeId,
                    GroupAdId = groupAd.Id
                };

                try
                {
                    var addedRequest = await AddAsync(acesseRequest);
                    result.AcesseRequests.Add(addedRequest);
                }
                catch (DomainException dex)
                {
                    foreach (var error in dex.Errors2)
                    {
                        result.Errors.Add(error.Message);
                        if (!result.ValidationErrors.ContainsKey(error.Key))
                        {
                            result.ValidationErrors[error.Key] = new List<string>();
                        }
                        result.ValidationErrors[error.Key].Add(error.Message);
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Failed to add request for GroupAdId: {groupAd.Id}. Error: {ex.Message}");
                }
            }

            if (result.AcesseRequests.Any() && result.Errors.Any())
            {
                result.PartialSucess = true;
            }
            else if (result.Errors.Any())
            {
                result.Success = false;
            }
            else
            {
                result.Success = true;
            }

            return result;
        }
    }
}

