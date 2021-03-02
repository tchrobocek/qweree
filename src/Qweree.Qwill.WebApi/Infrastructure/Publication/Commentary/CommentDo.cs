using System;
using Qweree.Qwill.WebApi.Domain.Commentary;

namespace Qweree.Qwill.WebApi.Infrastructure.Publication.Commentary
{
    public class CommentDo
    {
        public Guid? Id { get; set; }
        public Guid? SubjectId { get; set; }
        public Guid? UserId { get; set; }
        public CommentSubjectType? SubjectType { get; set; }
        public string? Text { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
    }
}