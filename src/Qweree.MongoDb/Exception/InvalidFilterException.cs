namespace Qweree.Mongo.Exception;

public class InvalidFilterException : System.Exception
{
    public InvalidFilterException()
    {
    }

    public InvalidFilterException(string? message) : base(message)
    {
    }

    public InvalidFilterException(string? message, System.Exception? innerException) : base(message, innerException)
    {
    }
}