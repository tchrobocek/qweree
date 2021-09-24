namespace Qweree.Cdn.WebApi.Infrastructure.Explorer
{
    public class ExplorerFilter
    {
        public ExplorerFilter(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}