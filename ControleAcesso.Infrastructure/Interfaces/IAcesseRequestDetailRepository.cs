using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Interfaces.Repositories;

namespace ControleAcesso.Infrastructure.Interfaces
{
    public interface IAcesseRequestDetailRepository : IGenericRepository<AcesseRequestDetail>
    {
        Task<AcesseRequestDetail> AddWithApprovalAsync(AcesseRequestDetail acesseRequestDetail, PriorApproval priorApproval);
        Task<IEnumerable<AcesseRequestDetail>> GetPendingEspecialistAsync(int employee);
        Task<AcesseRequestDetail?> GetPendingEspecialistByIdAsync(int id, int employeeId);
    }
}
