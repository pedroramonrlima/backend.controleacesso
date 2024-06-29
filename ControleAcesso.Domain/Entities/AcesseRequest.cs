using ControleAcesso.Domain.Interfaces.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Domain.Entities
{
    public class AcesseRequest : IEntity
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public int GroupAdId { get; set; }
        public int RequestTypeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("GroupAdId")]
        public virtual GroupAd GroupAd { get; set; }

        [ForeignKey("RequestTypeId")]
        public virtual RequestType RequestType { get; set; }
        public virtual ICollection<AcesseRequestDetail> AcesseRequestDetails { get; set; }
    }
}
