using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Exceptions;
using ControleAcesso.Domain.Interfaces.Services;
using ControleAcesso.Web.ModelView;
using ControleAcesso.Web.ModelView.Group;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace ControleAcesso.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _service;

        public GroupController(IGroupService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupAd>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GroupAd>> GetById(int id)
        {
            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpPost]
        public ActionResult<GroupCreateModelView> post([FromBody] GroupCreateModelView group) 
        {
            if (!IsValidCreate(group))
            {
                return BadRequest(FormatValidationErrors(ModelState, 400));
            }
            try
            {
                return Ok(_service.Add(group.ToEntity()));
            }
            catch (GroupValidateException ex)
            {
                return BadRequest(new {code=400 ,error = new { message = ex.Message,erros = ex.Errors } });
            }
        }

        [HttpPut]
        public async Task<ActionResult<GroupAd>> update([FromBody]GroupUpdateModelView group)
        {

            
            if (!IsValidUpdate(group))
            {
                return BadRequest(FormatValidationErrors(ModelState, 400));
            }

            try
            {
                return Ok( await _service.UpdateAsync(group.ToEntity()));

            }catch(GroupValidateException ex)
            {
                return BadRequest(new { code = 400, error = new { message = ex.Message, erros = ex.Errors } });
            }
        }

        [HttpDelete] 
        public async Task<ActionResult<GroupAd>> delete(int id) {

            return Ok(await _service.Delete(id));
        }

        private bool IsValidCreate(GroupCreateModelView group)
        {
            int expiresHour;
            if (!int.TryParse(group.ExpiresHour, out expiresHour) || expiresHour < 24 || expiresHour > 72)
            {
                ModelState.AddModelError("ExpiresHour", "O campo ExpiresHour deve ser um número inteiro entre 24 e 72.");
            }

            if (!ModelState.IsValid)
            {
                return false;
            }
            return true;
        }

        private bool IsValidUpdate(GroupUpdateModelView group)
        {
            int id;
            if (!int.TryParse(group.Id, out id) || id <= 0)
            {
                ModelState.AddModelError("Id", "O Id deve ser um número inteiro positivo.");
            }

            int expiresHour;
            if (!int.TryParse(group.ExpiresHour, out expiresHour) || expiresHour < 24 || expiresHour > 72)
            {
                ModelState.AddModelError("ExpiresHour", "O campo ExpiresHour deve ser um número inteiro entre 24 e 72.");
            }

            if (!ModelState.IsValid)
            {
                return false;
            }

            return true;
        }

        protected object FormatValidationErrors(ModelStateDictionary modelState, int statusCode)
        {
            var errors = modelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return new
            {
                errors = errors,
                title = "One or more validation errors occurred.",
                status = statusCode
            };
        }
    }
}
