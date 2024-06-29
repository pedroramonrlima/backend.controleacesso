using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleAcesso.Domain.Exceptions
{
    public class GroupValidateException : DomainException
    {
        public GroupValidateException(string message)
       : base(message)
        {
        }

        public GroupValidateException(String message,List<string> errors) :base(message, errors) { }
    }
}
