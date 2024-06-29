namespace ControleAcesso.Web.Response
{
    public class ErrorResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<ErrorDetail> Errors { get; set; } = new List<ErrorDetail>();
    }
}
