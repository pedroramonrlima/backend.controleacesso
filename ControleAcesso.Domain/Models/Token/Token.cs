namespace ControleAcesso.Domain.Models.Token
{
    public class Token
    {
        public int EmployeeId { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Departament { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public bool IsManager { get; set; }
        public bool IsSpecialist { get; set; }
    }
}
