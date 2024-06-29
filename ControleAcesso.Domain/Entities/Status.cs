using ControleAcesso.Domain.Interfaces.Entities;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Domain.Entities
{
    public class Status : IEntity
    {
        [Key]
        public int Id { get; set; }

        [StringLength(45)]
        public string Name { get; set; }

        public virtual ICollection<AcesseRequestDetail> AcesseRequestDetails { get; set; }
    }
}
