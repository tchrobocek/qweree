using System;
using Qweree.Mongo;
using Qweree.Qwill.WebApi.Domain.Commentary;
using Qweree.Qwill.WebApi.Domain.Persistence;

namespace Qweree.Qwill.WebApi.Infrastructure.Publication.Commentary
{
    public class CommentRepository : MongoRepositoryBase<Comment, CommentDo>, ICommentRepository
    {
        public CommentRepository(string collectionName, MongoContext context) : base(collectionName, context)
        {
        }

        protected override Func<Comment, CommentDo> ToDocument => comment => new CommentDo
        {
            Id = comment.Id,
            Text = comment.Text,
            CreationDate = comment.CreationDate,
            ModificationDate = comment.ModificationDate,
            SubjectId = comment.SubjectId,
            UserId = comment.UserId,
            SubjectType = comment.SubjectType
        };

        protected override Func<CommentDo, Comment> FromDocument => document => new Comment(
            document.Id ?? Guid.Empty,
            document.SubjectId ?? Guid.Empty,
            document.UserId ?? Guid.Empty,
            document.SubjectType ?? CommentSubjectType.None,
            document.Text ?? "",
            document.CreationDate ?? DateTime.MinValue,
            document.ModificationDate ?? DateTime.MinValue);
    }
}