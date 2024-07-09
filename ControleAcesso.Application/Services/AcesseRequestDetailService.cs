using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Enumerations;
using ControleAcesso.Domain.Exceptions;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Infrastructure.Interfaces;

namespace ControleAcesso.Application.Services
{
    public class AcesseRequestDetailService : IAcesseRequestDetailService
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        private readonly IAcesseRequestDetailRepository _acesseDetailRepository;
        private readonly ILdapService _ldapService;
        public AcesseRequestDetailService(ILdapService ldapService, IAcesseRequestDetailRepository acesseDetailRepository) 
        {
            _acesseDetailRepository = acesseDetailRepository;
            _ldapService = ldapService;
        }
        public async Task<AcesseRequestDetail> ApproveManager(AcesseRequestDetail acesseRequestDetail ,int idManager)
        {
            await ValidIsManager(acesseRequestDetail,idManager);

            if (_errors.Count > 0)
            {
                throw new DomainException("a", _errors);
            }

            if(!await ValidAcesseRequestEspecialist(acesseRequestDetail))
            {
                acesseRequestDetail.StatusRequestId = (int)EStatusRequest.Processando;
                acesseRequestDetail = await _acesseDetailRepository.UpdateAsync(acesseRequestDetail);
                try
                                         {
                    _ldapService.AddUserToGroup(acesseRequestDetail.AcesseRequest.Employee.Login, acesseRequestDetail.AcesseRequest.GroupAd.Dn);
                    acesseRequestDetail.StatusRequestId = (int)EStatusRequest.Aprovado;
                    return await _acesseDetailRepository.UpdateAsync(acesseRequestDetail);
                }
                catch
                {
                    acesseRequestDetail.StatusRequestId = (int)EStatusRequest.Error;
                    return await _acesseDetailRepository.UpdateAsync(acesseRequestDetail);

                }
            }
            else
            {
                acesseRequestDetail.StatusRequestId = 2;
                return await _acesseDetailRepository.UpdateAsync(acesseRequestDetail);

            }
        }

        public async Task<AcesseRequestDetail> PriorApproval(AcesseRequestDetail acesse,int idEmployee)
        {
            await ValidPriorApproval(acesse, idEmployee);

            if (_errors.Count > 0)
            {
                throw new DomainException("a", _errors);
            }
            AcesseRequestDetail requestDetail = await _acesseDetailRepository.GetPendingEspecialistByIdAsync(acesse.Id);

            PriorApproval priorApproval = new PriorApproval { 
                GroupApprovalId = requestDetail.AcesseRequest.GroupAd.GroupApprovals.First().Id,
                AcesseRequestDetailId = requestDetail.Id,
                HasPriorApproval = true,
            };

            requestDetail.StatusRequestId = (int) EStatusRequest.Processando;
            await _acesseDetailRepository.AddWithApprovalAsync(requestDetail, priorApproval);

            try
            {
                _ldapService.AddUserToGroup(requestDetail.AcesseRequest.Employee.Login, requestDetail.AcesseRequest.GroupAd.Dn);
                requestDetail.StatusRequestId = (int)EStatusRequest.Aprovado;
                
                // Atualiza a entidade com o novo status
                requestDetail = await _acesseDetailRepository.UpdateAsync(requestDetail);


                return requestDetail;

            }
            catch
            {
                requestDetail.StatusRequestId = (int)EStatusRequest.Error;
                return await _acesseDetailRepository.UpdateAsync(requestDetail);
            }
        }

        public async Task<IEnumerable<AcesseRequestDetail>> GetPedentManager(int idManager)
        {
            IEnumerable<AcesseRequestDetail> requestsDetail = await _acesseDetailRepository.GetAllAsync(ard =>
            ard.StatusRequestId == 1 &&
            ard.ManagerApproval.EmployeeId == idManager);
            return requestsDetail;
        }

        public async Task<IEnumerable<AcesseRequestDetail>> GetPedentEspecialist(int employeeId)
        {
            return await _acesseDetailRepository.GetPendingEspecialistAsync(employeeId);
        }

        private async Task ValidIsManager(AcesseRequestDetail acesseRequestDetail, int idManager) 
        {
            AcesseRequestDetail? acesse = await _acesseDetailRepository.GetAsync(
                adr => adr.Id == acesseRequestDetail.Id &&
                adr.ManagerApprovalId == idManager
                ,NavigationLevel.FirstLevel);

            if ( acesse == null )
            {
                AddError(nameof(AcesseRequestDetail), "Não foi encontrado nenhuma requisição com os dados informados");
            }else if(acesse.StatusRequestId != 1)
            {
                AddError(nameof(AcesseRequestDetail), $"Não e possivel aprovar esta requisição devida a mesma ja está com o status {acesse.Status.Name}");
            }
        }

        private async Task<bool> ValidAcesseRequestEspecialist(AcesseRequestDetail acesseRequestDetail)
        {
            AcesseRequestDetail? acesse = await _acesseDetailRepository.GetAsync(
                ard => ard.Id == acesseRequestDetail.Id &&
                ard.AcesseRequest.HasPriorApproval == true
                );
            if ( acesse == null )
            {
                return false;
            }
            return true;
        }

        private async Task<bool?> ValidPriorApproval(AcesseRequestDetail acesse, int employeeId)
        {
            AcesseRequestDetail? requests = await _acesseDetailRepository.GetPendingEspecialistByIdAsync(acesse.Id,employeeId);
            if ( requests == null )
            {
                AddError(nameof(requests), "Não foi encontrado nenhuma requisição com os dados informados");
            }else if (requests.StatusRequestId != (int)EStatusRequest.AguardandoAprovacaEsspecialista)
            {
                AddError(nameof(requests), $"Não e possivel aprovar esta requisição devida a mesma ja está com o status {requests.Status.Name}");
            }

            return true;
        }

        private void AddError(string key, string message)
        {
            if (!_errors.ContainsKey(key))
            {
                _errors[key] = [message];
            }
            else
            {
                _errors[key].Add(message);
            }
        }
    }
}
