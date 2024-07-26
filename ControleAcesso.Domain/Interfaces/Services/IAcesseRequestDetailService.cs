using ControleAcesso.Domain.Entities;

namespace ControleAcesso.Domain.Interfaces.Services
{
    public interface IAcesseRequestDetailService : IGenericService<AcesseRequestDetail>
    {
        Task<AcesseRequestDetail> ApproveManagerAsync(AcesseRequestDetail acesseRequestDetail, int employeeId);
        Task<AcesseRequestDetail> ManagerRejectAsync(AcesseRequestDetail acesseRequestDetail, int employeeId);
        Task<AcesseRequestDetail> PriorApprovalAsync(AcesseRequestDetail acesse, int idEmployee);
        Task<AcesseRequestDetail> PriorRejectAsync(AcesseRequestDetail acesse, int idEmployee);
        Task<IEnumerable<AcesseRequestDetail>> GetPendingManagerAsync(int idManager);
        Task<IEnumerable<AcesseRequestDetail>> GetRequestByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<AcesseRequestDetail>> GetPendingEspecialistAsync(int employeeId);
        Task<IEnumerable<AcesseRequestDetail>> GetPendingRequestsByEmployeeAndGroupAsync(int employeeId, int groupId);
    }
}
