using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ControleAcesso.Domain.Interfaces.Entities;

namespace ControleAcesso.Domain.Entities
{
    public class Employee : IEntity
    {
        [Key]
        public int Id { get; set; }

        [StringLength(45)]
        public string Cpf { get; set; }
        
        [StringLength(45)]
        public string? Login { get; set; }

        [StringLength(45)]
        public string Registration { get; set; }

        [StringLength(45)]
        public string Name { get; set; }

        public DateTime BomDate { get; set; }

        public DateTime ContractDate { get; set; }

        public int OfficeId { get; set; }
        public int DepartmentId { get; set; }
        public int EmployeeStatusId { get; set; }

        [ForeignKey("OfficeId")]
        public virtual Company Office { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        [ForeignKey("EmployeeStatusId")]
        public virtual EmployeeStatus EmployeeStatus { get; set; }

        //public virtual ICollection<AcesseRequest> AcesseRequests { get; set; }
        //public virtual ICollection<AcesseRequestDetail> AcesseRequestDetails { get; set; }
    }
}
