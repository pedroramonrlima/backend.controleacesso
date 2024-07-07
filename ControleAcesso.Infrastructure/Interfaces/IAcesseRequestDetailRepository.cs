using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Interfaces.Repositories;

namespace ControleAcesso.Infrastructure.Interfaces
{
    public interface IAcesseRequestDetailRepository : IGenericRepository<AcesseRequestDetail>
    {
        Task<IEnumerable<AcesseRequestDetail>> GetPendingEspecialistAsync(int employee);
    }
}
