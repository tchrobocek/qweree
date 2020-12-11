namespace Qweree.Cdn.WebApi.Application.Explorer
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