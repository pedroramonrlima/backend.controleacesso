using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Web.ModelView.Employees;
using Microsoft.AspNetCore.Mvc;

namespace ControleAcesso.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeStatusController : AbstractController<EmployeeStatus, EmployeeStatusCreateModelView, EmployeeStatusUpdateModelView>
    {
        public EmployeStatusController(IGenericService<EmployeeStatus> service) : base(service)
        {
        }
    }
}
