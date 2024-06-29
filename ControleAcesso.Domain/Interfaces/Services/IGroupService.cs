using ControleAcesso.Domain.Entities;
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
    }
}
