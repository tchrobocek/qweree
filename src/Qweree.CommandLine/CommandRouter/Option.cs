namespace Qweree.CommandLine.CommandRouter
{
    public class Option
    {
        public Option(string name, char? shortName)
        {
            Name = name;
            ShortName = shortName;
        }

        public string Name { get; }
        public char? ShortName { get; }
    }
}