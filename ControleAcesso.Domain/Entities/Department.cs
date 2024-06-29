using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ControleAcesso.Domain.Interfaces.Entities;

namespace ControleAcesso.Domain.Entities
{
    public class Department : IEntity
    {
        [Key]
        public int Id { get; set; }

        [StringLength(45)]
        public string Name { get; set; }

        public int? ManagerId { get; set; }

        [ForeignKey("ManagerId")]
        public virtual Manager Manager { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
