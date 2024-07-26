using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Web.ModelView.Employees;
using ControleAcesso.Web.ModelView.Titles;
using Microsoft.AspNetCore.Mvc;

namespace ControleAcesso.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitleController : AbstractController<Title, TitleCreateModelView, TitleUpdateModelView>
    {
        public TitleController(IGenericService<Title> service) : base(service)
        {
        }
    }
}
