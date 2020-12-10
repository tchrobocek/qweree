namespace Qweree.Cdn.WebApi.Application.Storage
{
    public class ReadObjectInput
    {
        public ReadObjectInput(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}