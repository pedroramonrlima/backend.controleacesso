using ControleAcesso.Domain.Entities;

namespace ControleAcesso.Domain.Interfaces.Services
{
    public interface IAcesseRequestDetailService
    {
        Task<AcesseRequestDetail> ApproveManagerAsync(AcesseRequestDetail acesseRequestDetail, int idManager);

        Task<AcesseRequestDetail> PriorApprovalAsync(AcesseRequestDetail acesse, int idEmployee);
        Task<IEnumerable<AcesseRequestDetail>> GetPedentManager(int idManager);
        Task<IEnumerable<AcesseRequestDetail>> GetPedentEspecialist(int employeeId);
    }
}
