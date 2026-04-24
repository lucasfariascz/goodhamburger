namespace GoodHamburger.Domain.Exceptions;

public class DuplicateItemException : Exception
{
    public DuplicateItemException(string Message) : base(Message) {}
}