using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Web.ModelView.Departament;
using Microsoft.AspNetCore.Mvc;

namespace ControleAcesso.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartamentController : AbstractController<Department, DepartamentCreateModelView, DepartamentUpdateModelView>
    {
        public DepartamentController(IGenericService<Department> service) : base(service)
        {
        }
    }
}
