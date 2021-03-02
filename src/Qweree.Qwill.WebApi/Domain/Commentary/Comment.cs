using System;

namespace Qweree.Qwill.WebApi.Domain.Commentary
{
    public enum CommentSubjectType
    {
        None,
        Story,
        Channel
    }

    public class Comment
    {
        public Comment(Guid id, Guid subjectId, Guid userId, CommentSubjectType subjectType, string text, DateTime creationDate,
            DateTime modificationDate)
        {
            Id = id;
            SubjectId = subjectId;
            UserId = userId;
            SubjectType = subjectType;
            Text = text;
            CreationDate = creationDate;
            ModificationDate = modificationDate;
        }

        public Guid Id { get; }
        public Guid SubjectId { get; }
        public Guid UserId { get; }
        public CommentSubjectType SubjectType { get; }
        public string Text { get; }
        public DateTime CreationDate { get; }
        public DateTime ModificationDate { get; }
    }
}