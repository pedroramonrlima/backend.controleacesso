using ControleAcesso.Domain.Interfaces.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Domain.Entities
{
    public class PriorApproval : IEntity
    {
        [Key]
        public int Id { get; set; }

        public bool HasPriorApproval { get; set; }

        public int GroupApprovalId { get; set; }
        public int AcesseRequestDetailId { get; set; }

        [ForeignKey("GroupApprovalId")]
        public virtual GroupApproval GroupApproval { get; set; }

        [ForeignKey("AcesseRequestDetailId")]
        public virtual AcesseRequestDetail AcesseRequestDetail { get; set; }
    }
}
