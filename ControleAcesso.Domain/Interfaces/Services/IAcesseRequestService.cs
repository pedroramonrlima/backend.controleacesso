using ControleAcesso.Domain.Entities;

namespace ControleAcesso.Domain.Interfaces.Services
{
    public interface IAcesseRequestService : IGenericService<AcesseRequest>
    {
        AcesseRequest Add(AcesseRequest entity);

        Task<AcesseRequest> AddAsync(AcesseRequest entity);

        Task<AcesseRequest> UpdateAsync(AcesseRequest entity);
    }
}
