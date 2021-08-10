namespace Qweree.ConsoleApplication.Infrastructure.RunContext
{
    public class Context
    {
        public Context(string rootDirectory)
        {
            RootDirectory = rootDirectory;
        }

        public string RootDirectory { get; }
    }
}