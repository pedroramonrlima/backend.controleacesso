using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleAcesso.Domain.Exceptions
{
    internal class GroupNotFoundException : DomainException
    {
        public GroupNotFoundException(int grupoId)
        : base($"Grupo com Id {grupoId} não foi encontrado.")
        {
        }
    }
}
