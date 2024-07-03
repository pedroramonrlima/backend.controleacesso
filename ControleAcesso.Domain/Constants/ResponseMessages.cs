namespace ControleAcesso.Domain.Constants
{
    public class ResponseMessages
    {
        //Generic
        public const string NotProcessingRequest = "Não foi possível processar sua solicitação.";
        public const string ErrorValidate = "Erro de Validação";


        //Validations ModelState
        public const string RequiredField = "Campo é obrigatório";
        public const string OnlyTextAllowed = "O campo deve conter somente letras";
        public const string OnlyNumbersAllowed = "O Campo deve conter somente numeros";
        public const string NoPastDateAllowed = "A data não pode ser no passado.";
        public const string NoFutureDateAllowed = "A data não pode ser no futuro.";

        //Validations Service
        public const string DataNotFound = "Não foi encontrado nenhum registro com os dados informados.";
        public const string ProblemUpdateDatabase = "Ocorreu um conflito ao tentar atualizar o registro. Por favor, verifique se os dados estão corretos e tente novamente.";
        public const string ProblemConsultDatabase = "Não foi possivel realizar a consulta no banco de dados:";
        public const string ProblemDeleteDatabase = "Ocorreu um conflito ao tentar deletar o registro. Por favor, verifique se os dados estão corretos e tente novamente.";
        public const string ProblemInsertDatabase = "Ocorreu um conflito ao tentar fazer o cadastro do registro. Por favor, verifique se os dados estão corretos e tente novamente.";

        //Validation AcesseRequest
        public const string AcesseRequestIsExists = "Já existe uma Requisição de acesso sobre o numero {0} para o item ({1}) com o status {2}";



        public static string MaxCharacters(int value)
        {
            return $"O Campo deve conter no maximo {value} caractere";
        }

    }    
}
