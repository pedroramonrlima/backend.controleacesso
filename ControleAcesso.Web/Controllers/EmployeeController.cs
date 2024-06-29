using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Web.ModelView.Employees;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ControleAcesso.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : AbstractController<Employee, EmployeeCreateModelView, EmployeeUpdateModelView>
    {
        public EmployeeController(IGenericService<Employee> service) : base(service)
        {
        }
    }
}
