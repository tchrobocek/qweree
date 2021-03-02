using System;
using Qweree.Qwill.WebApi.Web.Publication.Publishers;

namespace Qweree.Qwill.WebApi.Web.Publication.Commentary
{
    public class SubjectCommentDto
    {
        public Guid? Id { get; set; }
        public AuthorDto? Author { get; set; }
        public string? Text { get; set; }
        public DateTime CreationTime { get; set; }
    }
}