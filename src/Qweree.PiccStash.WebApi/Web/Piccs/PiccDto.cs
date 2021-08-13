using System;
using System.Collections.Immutable;

namespace Qweree.PiccStash.WebApi.Web.Piccs
{
    public class PiccDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public ImmutableArray<string>? StorageSlug { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}