using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Web.ModelView.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ControleAcesso.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcessRequestController : AbstractController<AcesseRequest, AcesseRequestCreateModelView, AcesseRequestUpdateModelView>
    {
        public AcessRequestController(IGenericService<AcesseRequest> service) : base(service)
        {
        }
    }
}
