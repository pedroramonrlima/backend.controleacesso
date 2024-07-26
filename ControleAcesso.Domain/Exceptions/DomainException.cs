namespace ControleAcesso.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public List<string> Errors { get; private set; } = new List<string>();
        public List<ValidationError> Errors2 { get; } = new List<ValidationError>();
        public Dictionary<string, List<string>> Properties { get; private set; } = new Dictionary<string, List<string>>();
        public DomainException() { }

        public DomainException(string message) : base(message) {
            
            Errors.Add(message);
            Properties.Add("DomainExeption",Errors);
        }

        public DomainException(string message, Exception innerException) : base(message, innerException) { }

        public DomainException(string message,List<string> errors)
            : base(message)
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
            Properties.Add("DomainExeption",Errors);
        }

        public DomainException(string message, Dictionary<string,List<string>> errors)
            : base(message)
        {
            Properties = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        public DomainException(string message, List<ValidationError> erros) : base(message) 
        { 
            Errors2 = erros ?? new List<ValidationError> ();
        }
    }

    public class ValidationError
    { 
        public string Key { get; set; }
        public string Message {get; set;}
    }

}
