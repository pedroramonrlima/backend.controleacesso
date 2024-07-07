using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Enumerations;
using ControleAcesso.Domain.Exceptions;
using ControleAcesso.Infrastructure.Interfaces;

namespace ControleAcesso.Application.Services
{
    public class AcesseRequestDetailService : IAcesseRequestDetailService
    {
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        private readonly IAcesseRequestDetailRepository _acesseDetailRepository;
        public AcesseRequestDetailService(IAcesseRequestDetailRepository acesseDetailRepository) 
        {
            _acesseDetailRepository = acesseDetailRepository;
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
                acesseRequestDetail.StatusRequestId = 3;
                return _acesseDetailRepository.Update(acesseRequestDetail);
            }
            else
            {
                acesseRequestDetail.StatusRequestId = 2;
                return _acesseDetailRepository.Update(acesseRequestDetail);

            }
        }

        public async Task<AcesseRequestDetail> ApproveEspecialista(AcesseRequestDetail acesse,int idEmployee)
        {
            acesse = await ValidApprovveEspecialist(acesse, idEmployee);

            if (_errors.Count > 0)
            {
                throw new DomainException("a", _errors);
            }

            PriorApproval priorApproval = new PriorApproval { 
                GroupApprovalId = acesse.AcesseRequest.GroupAd.GroupApprovals.First().Id,
                AcesseRequestDetailId = acesse.Id,
                HasPriorApproval = true,
            };

            acesse.StatusRequestId = (int) EStatusRequest.Aprovado;
            //acesse.Status = null;
            return await _acesseDetailRepository.AddWithApprovalAsync(acesse, priorApproval);
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

        private async Task<AcesseRequestDetail?> ValidApprovveEspecialist(AcesseRequestDetail acesse, int employeeId)
        {
            AcesseRequestDetail? requests = await _acesseDetailRepository.GetPendingEspecialistByIdAsync(acesse.Id,employeeId);
            if ( requests == null )
            {
                AddError(nameof(requests), "Não foi encontrado nenhuma requisição com os dados informados");
            }else if (requests.StatusRequestId != (int)EStatusRequest.AguardandoAprovacaEsspecialista)
            {
                AddError(nameof(requests), $"Não e possivel aprovar esta requisição devida a mesma ja está com o status {requests.Status.Name}");
            }

            return requests;
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
