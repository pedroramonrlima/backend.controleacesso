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
