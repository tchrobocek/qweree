namespace Qweree.CommandLine.AspNet.CommandRouter
{
    public class ConfigurationBuilder
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public CommandConfiguration Build()
        {
            return new(Name, Description);
        }
    }
}