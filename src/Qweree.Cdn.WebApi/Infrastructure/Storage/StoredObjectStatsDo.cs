namespace Qweree.Cdn.WebApi.Infrastructure.Storage
{
    public class StoredObjectStatsDo
    {
        public string? Id { get; set; }
        public long? Count { get; set; }
        public long? Used { get; set; }
    }
}