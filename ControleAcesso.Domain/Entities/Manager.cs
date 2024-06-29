using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ControleAcesso.Domain.Interfaces.Entities;
namespace ControleAcesso.Domain.Entities
{
    public class Manager : IEntity
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
    }
}
