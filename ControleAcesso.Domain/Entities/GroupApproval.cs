using ControleAcesso.Domain.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleAcesso.Domain.Entities
{
    public class GroupApproval : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int GroupAdId {  get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("GroupAdId")]
        public virtual GroupAd GroupAd { get; set; }
    }
}
