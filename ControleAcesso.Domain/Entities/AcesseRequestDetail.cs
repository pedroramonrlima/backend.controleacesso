using ControleAcesso.Domain.Interfaces.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Domain.Entities
{
    public class AcesseRequestDetail : IEntity
    {
        [Key]
        public int Id { get; set; }

        public int RequesterEmployeeId { get; set; }
        public int ManagerApprovalId { get; set; }
        public int StatusRequestId { get; set; }
        public int AcesseRequestId { get; set; }

        [ForeignKey("RequesterEmployeeId")]
        public virtual Employee RequesterEmployee { get; set; }

        [ForeignKey("ManagerApprovalId")]
        public virtual Manager ManagerApproval { get; set; }

        [ForeignKey("StatusRequestId")]
        public virtual StatusRequest Status { get; set; }

        [ForeignKey("AcesseRequestId")]
        public virtual AcesseRequest AcesseRequest { get; set; }
    }
}
