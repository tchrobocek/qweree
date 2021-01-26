namespace Qweree.Mongo.Exception
{
    public class DocumentNotFoundException : System.Exception
    {
        public DocumentNotFoundException()
        {
        }

        public DocumentNotFoundException(string? message) : base(message)
        {
        }

        public DocumentNotFoundException(string? message, System.Exception? innerException) : base(message,
            innerException)
        {
        }
    }
}