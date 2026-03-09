namespace Corvel.ToDo.Abstractions.Exceptions;

public class AuthenticationFailedException : Exception
{
    public AuthenticationFailedException(string message) : base(message) { }
}
