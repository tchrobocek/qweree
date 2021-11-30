using System;

namespace Qweree.WebApplication.Infrastructure.Notes
{
    public class NoteCollectionDto
    {
        public string? Version { get; set; } = "1.0";
        public NoteDto[]? Notes { get; set; }
    }

    public class NoteDto
    {
        public Guid? Id { get; set; }
        public string? Text { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}