using System;

namespace Qweree.Qwill.WebApi.Domain.Publishers
{
    public class Author
    {
        public Author(Guid id, string name, Guid userId, DateTime creationDate, DateTime lastModificationDate)
        {
            Id = id;
            Name = name;
            UserId = userId;
            CreationDate = creationDate;
            LastModificationDate = lastModificationDate;
        }

        public Guid Id { get; }
        public string Name { get; }
        public Guid UserId { get; }
        public DateTime CreationDate { get; }
        public DateTime LastModificationDate { get; }
    }
}