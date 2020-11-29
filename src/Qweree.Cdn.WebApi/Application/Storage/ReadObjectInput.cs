namespace Qweree.Cdn.WebApi.Application.Storage
{
    public class ReadObjectInput
    {
        public ReadObjectInput(string slug)
        {
            Slug = slug;
        }

        public string Slug { get; }
    }
}