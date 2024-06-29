using ControleAcesso.Domain.Interfaces.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Domain.Entities
{
    [Serializable]
    public class GroupAd : IEntity
    {
    
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(255)]
        public string? Dn { get; set; }
        public int? ExpiresHour { get; set; }
        //public virtual ICollection<AcesseRequest> AcesseRequests { get; set; }
    }
}
