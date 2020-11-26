namespace Qweree.Validator
{
    /// <summary>
    ///     Wrapper for validation message and path of validated subject.
    /// </summary>
    public class ValidationMessage
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="path">Validation subject path.</param>
        /// <param name="message">Validation message.</param>
        public ValidationMessage(string path, string message)
        {
            Message = message;
            Path = path;
        }

        /// <summary>
        ///     Validation subject path.
        /// </summary>
        public string Path { get; }

        /// <summary>
        ///     Validation message.
        /// </summary>
        public string Message { get; }
    }
}