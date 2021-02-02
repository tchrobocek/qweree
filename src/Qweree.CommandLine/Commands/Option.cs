namespace Qweree.CommandLine.Commands
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