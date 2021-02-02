namespace Qweree.CommandLine.CommandRouter
{
    public class Argument
    {
        public Argument(string name, int order)
        {
            Name = name;
            Order = order;
        }

        public string Name { get; }
        public int Order { get; }
    }
}