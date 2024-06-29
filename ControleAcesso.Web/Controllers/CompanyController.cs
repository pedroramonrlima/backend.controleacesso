using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Web.ModelView.Companies;
using Microsoft.AspNetCore.Mvc;

namespace ControleAcesso.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : AbstractController<Company, CompanyCreateModelView, CompanyUpdateModelView>
    {
        public CompanyController(IGenericService<Company> service) : base(service)
        {
        }
    }
}
