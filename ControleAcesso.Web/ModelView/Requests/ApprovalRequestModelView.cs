using ControleAcesso.Domain.Entities;
using ControleAcesso.Web.Interfaces;

namespace ControleAcesso.Web.ModelView.Requests
{
    public class ApprovalRequestModelView : IObjectModelView<AcesseRequestDetail>
    {
        public int Id { get; set; }
        public int RequesterEmployeeId { get; set; }
        public int ManagerApprovalId { get; set; }
        public int StatusRequestId { get; set; }
        public int AcesseRequestId { get; set; }

        public AcesseRequestDetail ToEntity()
        {
            return new AcesseRequestDetail
            {
                Id = Id,
                RequesterEmployeeId = RequesterEmployeeId,
                ManagerApprovalId = ManagerApprovalId,
                StatusRequestId = StatusRequestId,
                AcesseRequestId = AcesseRequestId
            };
        }
    }
}
