namespace Qweree.Validator
{
    /// <summary>
    ///     Wrapper for subject path and subject of validation
    /// </summary>
    public class ValidationContext
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="path">Validation subject path.</param>
        /// <param name="subject">Validation subject.</param>
        public ValidationContext(string path, object subject)
        {
            Subject = subject;
            Path = path;
        }

        /// <summary>
        ///     Validation subject.
        /// </summary>
        public object Subject { get; }

        /// <summary>
        ///     Subject path.
        /// </summary>
        public string Path { get; }
    }

    /// <summary>
    ///     Wrapper for subject path and subject of validation
    /// </summary>
    /// <typeparam name="TModelType">Model type.</typeparam>
    public class ValidationContext<TModelType> : ValidationContext
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="path">Validation subject path.</param>
        /// <param name="subject">Validation subject.</param>
        public ValidationContext(string path, TModelType subject) : base(path, subject!)
        {
            Subject = subject;
        }

        /// <summary>
        ///     Validation subject.
        /// </summary>
        public new TModelType Subject { get; }
    }
}