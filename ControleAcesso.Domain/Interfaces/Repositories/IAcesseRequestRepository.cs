using ControleAcesso.Domain.Entities;
using System.Linq.Expressions;

namespace ControleAcesso.Domain.Interfaces.Repositories
{
    public interface IAcesseRequestRepository : IGenericRepository<AcesseRequest>
    {
        Task<AcesseRequest> AddWithDetailsAsync(AcesseRequest acesseRequest, AcesseRequestDetail acesseRequestDetail);
        Task<IEnumerable<AcesseRequest>> GetAllAcesseRequestAsync();

        Task<AcesseRequest?> GetAcesseRequestAsync(Expression<Func<AcesseRequest, bool>> predicate);
    }
}
