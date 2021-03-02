using System;
using Qweree.Qwill.WebApi.Domain.Publishers;

namespace Qweree.Qwill.WebApi.Domain.Commentary
{
    public class SubjectComment
    {
        public SubjectComment(Guid id, Author author, string text, DateTime creationTime)
        {
            Id = id;
            Author = author;
            Text = text;
            CreationTime = creationTime;
        }

        public Guid Id { get; }
        public Author Author { get; }
        public string Text { get; }
        public DateTime CreationTime { get; }
    }
}