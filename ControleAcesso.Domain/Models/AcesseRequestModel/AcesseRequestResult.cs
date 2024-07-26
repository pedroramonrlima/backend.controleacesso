using ControleAcesso.Domain.Entities;
namespace ControleAcesso.Domain.Models.AcesseRequestModel
{
    public class AcesseRequestResult
    {
        public bool Success { get; set; }
        public bool PartialSucess { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<AcesseRequest> AcesseRequests { get; set; } = new List<AcesseRequest>();

        public Dictionary<string, List<string>> ValidationErrors { get; set; } = new Dictionary<string, List<string>>();
    }
}
