using ControleAcesso.Domain.Constants;
using ControleAcesso.Domain.Entities;
using ControleAcesso.Web.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ControleAcesso.Web.ModelView.Requests
{
    public class AcesseRequestCreateModelView : IObjectModelView<AcesseRequest>
    {
        public IEnumerable<GroupAd> GroupAds { get; set; }
        public bool HasPriorApproval { get; set; } = false;

        public AcesseRequest ToEntity()
        {
            return new AcesseRequest
            {
                HasPriorApproval = HasPriorApproval,
            };
        }
    }
}
