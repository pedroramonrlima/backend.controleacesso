using ControleAcesso.Domain.Entities;
using ControleAcesso.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleAcesso.Domain.Interfaces.Services
{
    public interface IGroupService : IGenericService<GroupAd>
    {
         GroupAd Add(GroupAd entity);

         Task<IEnumerable<GroupAd>> GetAllAsync(NavigationLevel navigationLevel = NavigationLevel.None);
    }
}
