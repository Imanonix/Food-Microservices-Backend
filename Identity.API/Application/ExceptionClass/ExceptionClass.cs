namespace Application.ExceptionClass
{
    public class NotFoundException: IOException
    {
        public NotFoundException(string message) : base(message) { }
        
    }

    public class ValidationException: IOException
    {
        public ValidationException(string message) : base(message) { }
        
    }
    public class UnAuthorizeException : IOException
    {
        public UnAuthorizeException(string message): base(message) { }
        
    }

}
