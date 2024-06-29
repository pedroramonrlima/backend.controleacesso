using ControleAcesso.Domain.Interfaces.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Domain.Entities
{
    public class RequestType : IEntity
    {
        [Key]
        public int Id { get; set; }

        public bool HasPriorApproval { get; set; }
    }
}
