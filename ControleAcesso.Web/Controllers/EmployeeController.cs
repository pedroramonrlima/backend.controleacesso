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

        [HttpGet("{cpf}")]
        public async Task<ActionResult<Employee>> GetEmployeeCpf(string cpf)
        {

            return Ok(new Employee
            {
                Name="Ramon",
                Cpf="111111111111"
            });
        }
    }
}
