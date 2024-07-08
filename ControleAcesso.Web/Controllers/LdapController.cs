using ControleAcesso.Domain.Interfaces.Repositories;
using ControleAcesso.Domain.Models.Ldap;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ControleAcesso.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LdapController : ControllerBase
    {
        private readonly ILdapManagerRepository _ldapManagerRepository;

        public LdapController(ILdapManagerRepository ldapManagerRepository)
        {
            _ldapManagerRepository = ldapManagerRepository;
        }

        [HttpGet]
        public ActionResult<LdapUser> get()
        {
            return Ok(_ldapManagerRepository.GetLDAPUsers());
        }
    }
}
