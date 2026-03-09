namespace Corvel.ToDo.Abstractions.Exceptions;

public class DuplicateEmailException : Exception
{
    public DuplicateEmailException(string message) : base(message) { }
}
