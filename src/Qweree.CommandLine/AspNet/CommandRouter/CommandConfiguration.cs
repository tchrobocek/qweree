namespace Qweree.CommandLine.AspNet.CommandRouter
{
    public class CommandConfiguration
    {
        public CommandConfiguration(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }
        public string Description { get; }
    }
}