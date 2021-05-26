using System.Reflection;

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
        /// <param name="memberInfo">Validation subject member info.</param>
        public ValidationContext(string path, object? subject, MemberInfo? memberInfo)
        {
            Subject = subject;
            MemberInfo = memberInfo;
            Path = path;
        }

        /// <summary>
        ///     Validation subject.
        /// </summary>
        public object? Subject { get; }

        public MemberInfo? MemberInfo { get; }

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
        /// <param name="memberInfo">Validation subject member info.</param>
        public ValidationContext(string path, TModelType? subject, MemberInfo? memberInfo) : base(path, subject!,
            memberInfo)
        {
            Subject = subject;
        }

        /// <summary>
        ///     Validation subject.
        /// </summary>
        public new TModelType? Subject { get; }
    }
}