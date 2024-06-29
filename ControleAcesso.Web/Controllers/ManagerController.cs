using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Web.ModelView.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ControleAcesso.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : AbstractController<Manager, ManagerCreateModelView, ManagerUpdateModelView>
    {
        public ManagerController(IGenericService<Manager> service) : base(service)
        {
        }
    }
}
