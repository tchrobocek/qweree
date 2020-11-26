namespace Qweree.Mongo.Exception
{
    public class InsertDocumentException : System.Exception
    {
        public InsertDocumentException()
        {
        }

        public InsertDocumentException(string? message) : base(message)
        {
        }

        public InsertDocumentException(string? message, System.Exception? innerException) : base(message, innerException)
        {
        }
    }
}