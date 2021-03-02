using Qweree.Qwill.WebApi.Domain.Commentary;
using Qweree.Qwill.WebApi.Web.Publication.Publishers;

namespace Qweree.Qwill.WebApi.Web.Publication.Commentary
{
    public static class SubjectCommentMapper
    {
        public static SubjectCommentDto ToDto(SubjectComment subjectComment)
        {
            return new()
            {
                Author = AuthorMapper.ToDto(subjectComment.Author),
                Id = subjectComment.Id,
                Text = subjectComment.Text,
                CreationTime = subjectComment.CreationTime
            };
        }
    }
}