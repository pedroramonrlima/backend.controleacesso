using ControleAcesso.Domain.Entities;

namespace ControleAcesso.Domain.Exceptions
{
    public interface IAcesseRequestDetailService
    {
        Task<AcesseRequestDetail> ApproveManager(AcesseRequestDetail acesseRequestDetail, int idManager);

        Task<IEnumerable<AcesseRequestDetail>> ApproveEspecialista(int idManager);
        Task<IEnumerable<AcesseRequestDetail>> GetPedentManager(int idManager);
        Task<IEnumerable<AcesseRequestDetail>> GetPedentEspecialist(int employeeId);
    }
}
