using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Enumerations;
using ControleAcesso.Domain.Exceptions;
using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Infrastructure.Interfaces;

namespace ControleAcesso.Application.Services
{
    public class AcesseRequestDetailService : GenericService<AcesseRequestDetail>, IAcesseRequestDetailService
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        private readonly IAcesseRequestDetailRepository _acesseDetailRepository;
        private readonly ILdapService _ldapService;

        public AcesseRequestDetailService(
            IGenericRepository<AcesseRequestDetail> repository, 
            ILdapService ldapService, 
            IAcesseRequestDetailRepository acesseDetailRepository) : base(repository)
        {
            _acesseDetailRepository = acesseDetailRepository;
            _ldapService = ldapService;
        }

        public async Task<AcesseRequestDetail> ApproveManagerAsync(AcesseRequestDetail acesseRequestDetail, int idManager)
        {
            await ValidateManagerApprovalAsync(acesseRequestDetail, idManager);

            if (_errors.Any())
            {
                throw new DomainException(ResponseMessages.ErrorValidate, _errors);
            }

            if (!await ValidateAcesseRequestEspecialistAsync(acesseRequestDetail))
            {
                return await ProcessRequestAsync(acesseRequestDetail, EStatusRequest.Processando);
            }
            else
            {
                acesseRequestDetail.StatusRequestId = (int)EStatusRequest.AguardandoAprovacaEsspecialista;
                return await _acesseDetailRepository.UpdateAsync(acesseRequestDetail);
            }
        }

        public async Task<AcesseRequestDetail> ManagerRejectAsync(AcesseRequestDetail acesseRequestDetail, int employeeId)
        {
            await ValidateManagerApprovalAsync(acesseRequestDetail, employeeId);

            if (_errors.Any())
            {
                throw new DomainException(ResponseMessages.ErrorValidate, _errors);
            }

            acesseRequestDetail.StatusRequestId = (int)EStatusRequest.Reprovado;
            return await _acesseDetailRepository.UpdateAsync(acesseRequestDetail);
        }

        public async Task<AcesseRequestDetail> PriorApprovalAsync(AcesseRequestDetail acesseRequestDetail, int employeeId)
        {
            await ValidatePriorApprovalAsync(acesseRequestDetail, employeeId);

            if (_errors.Any())
            {
                throw new DomainException(ResponseMessages.ErrorValidate, _errors);
            }

            var requestDetail = await _acesseDetailRepository.GetPendingEspecialistByIdAsync(acesseRequestDetail.Id);

            var priorApproval = new PriorApproval
            {
                GroupApprovalId = requestDetail!.AcesseRequest.GroupAd.GroupApprovals.First().Id,
                AcesseRequestDetailId = requestDetail.Id,
                HasPriorApproval = true,
            };

            await _acesseDetailRepository.AddWithApprovalAsync(requestDetail, priorApproval);

            return await ProcessRequestAsync(requestDetail, EStatusRequest.Processando);
        }

        public async Task<AcesseRequestDetail> PriorRejectAsync(AcesseRequestDetail acesseRequestDetail, int employeeId)
        {
            await ValidatePriorApprovalAsync(acesseRequestDetail, employeeId);

            if (_errors.Any())
            {
                throw new DomainException(ResponseMessages.ErrorValidate, _errors);
            }

            var requestDetail = await _acesseDetailRepository.GetPendingEspecialistByIdAsync(acesseRequestDetail.Id);

            var priorApproval = new PriorApproval
            {
                GroupApprovalId = requestDetail!.AcesseRequest.GroupAd.GroupApprovals.First().Id,
                AcesseRequestDetailId = requestDetail.Id,
                HasPriorApproval = false,
            };

            requestDetail.StatusRequestId = (int)EStatusRequest.Reprovado;

            return await _acesseDetailRepository.AddWithApprovalAsync(requestDetail, priorApproval);
        }

        public async Task<IEnumerable<AcesseRequestDetail>> GetPendingManagerAsync(int idManager)
        {
            return await _acesseDetailRepository.GetPendingManagerAsync(idManager);
        }

        public async Task<IEnumerable<AcesseRequestDetail>> GetPendingEspecialistAsync(int employeeId)
        {
            return await _acesseDetailRepository.GetPendingEspecialistAsync(employeeId);
        }

        private async Task ValidateManagerApprovalAsync(AcesseRequestDetail acesseRequestDetail, int idManager)
        {
            var acesse = await _acesseDetailRepository.GetAsync(
                adr => adr.Id == acesseRequestDetail.Id &&
                       adr.ManagerApproval.EmployeeId == idManager,
                NavigationLevel.FirstLevel);

            if (acesse == null)
            {
                AddError(nameof(AcesseRequestDetail), ResponseMessages.DataNotFound);
            }
            else if (acesse.StatusRequestId != (int)EStatusRequest.AguardandoAprovacaManager)
            {
                AddError(nameof(AcesseRequestDetail),string.Format(ResponseMessages.ApprovalErrorMessage,acesse.Status.Name));
            }
        }

        private async Task ValidatePriorApprovalAsync(AcesseRequestDetail acesseRequestDetail, int employeeId)
        {
            var request = await _acesseDetailRepository.GetPendingEspecialistByIdAsync(acesseRequestDetail.Id, employeeId);

            if (request == null)
            {
                AddError(nameof(AcesseRequestDetail), ResponseMessages.DataNotFound);
            }
            else if (request.StatusRequestId != (int)EStatusRequest.AguardandoAprovacaEsspecialista)
            {
                AddError(nameof(AcesseRequestDetail), string.Format(ResponseMessages.ApprovalErrorMessage, request.Status.Name));
            }
        }

        private async Task<bool> ValidateAcesseRequestEspecialistAsync(AcesseRequestDetail acesseRequestDetail)
        {
            var acesse = await _acesseDetailRepository.GetAsync(
                ard => ard.Id == acesseRequestDetail.Id &&
                       ard.AcesseRequest.HasPriorApproval);

            return acesse != null;
        }

        private async Task<AcesseRequestDetail> ProcessRequestAsync(AcesseRequestDetail acesseRequestDetail, EStatusRequest initialStatus)
        {
            acesseRequestDetail.StatusRequestId = (int)initialStatus;
            acesseRequestDetail = await _acesseDetailRepository.UpdateAsync(acesseRequestDetail);

            try
            {
                _ldapService.AddUserToGroup(acesseRequestDetail.AcesseRequest.Employee.Login!, acesseRequestDetail.AcesseRequest.GroupAd.Dn!);
                acesseRequestDetail.StatusRequestId = (int)EStatusRequest.Aprovado;
            }
            catch (Exception ex)
            {
                acesseRequestDetail.StatusRequestId = (int)EStatusRequest.Error;
                AddError(nameof(ProcessRequestAsync), ex.Message);
            }

            return await _acesseDetailRepository.UpdateAsync(acesseRequestDetail);
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

        public async Task<IEnumerable<AcesseRequestDetail>> GetRequestByEmployeeIdAsync(int employeeId)
        {
            return await _acesseDetailRepository.GetRequestByEmployeeIdAsync(employeeId);
        }

        public Task<IEnumerable<AcesseRequestDetail>> GetPendingRequestsByEmployeeAndGroupAsync(int employeeId, int groupId)
        {
            return _acesseDetailRepository.GetAllAsync(ad =>
                (ad.AcesseRequest.EmployeeId == employeeId) &&
                (ad.StatusRequestId == 1 || ad.StatusRequestId == 2) &&
                (ad.AcesseRequest.GroupAdId == groupId), NavigationLevel.SecondLevel);
        }
    }
}
