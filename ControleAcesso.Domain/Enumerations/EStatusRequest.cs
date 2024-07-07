using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControleAcesso.Domain.Enumerations
{
    public enum EStatusRequest
    {
        AguardandoAprovacaManager = 1,
        AguardandoAprovacaEsspecialista = 2,
        Aprovado = 3,
        Reprovado = 4,
        Processando = 5,
        Error =6,
    }
}
