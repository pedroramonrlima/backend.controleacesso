using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Enumerations;
using ControleAcesso.Domain.Models.AcesseRequestModel;

namespace ControleAcesso.Domain.Interfaces.Services
{
    public interface IAcesseRequestService : IGenericService<AcesseRequest>
    {
        AcesseRequest Add(AcesseRequest entity);

        Task<AcesseRequest> AddAsync(AcesseRequest entity);
        Task<AcesseRequestResult> AddAsync(IEnumerable<GroupAd> entities, int employeeId);

        Task<AcesseRequest> UpdateAsync(AcesseRequest entity);

        Task<IEnumerable<AcesseRequestDetail>> GetAllAcesseRequestDetailAsync(int employeeId);
    }
}
