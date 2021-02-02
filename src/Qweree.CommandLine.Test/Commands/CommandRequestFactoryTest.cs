using Qweree.CommandLine.Commands;
using Xunit;

namespace Qweree.CommandLine.Test.Commands
{
    public class CommandRequestFactoryTest
    {
        [Fact]
        public void TestFromArgs()
        {
            var arguments = new[]
            {
                new Argument("arg1", 1),
                new Argument("arg0", 0)
            };
            var options = new[]
            {
                new Option("option0", 'o'),
                new Option("option1", 'p'),
                new Option("option2", null)
            };

            {
                var args = new[] {"a", "-op", "a", "b", "--option2", "c", "d", "--option1", "e"};
                var request = CommandRequestFactory.FromArgs(args, arguments, options);

                Assert.Equal(new[] {"b", "d"}, request.RemainingArgs);
                Assert.True(request.Arguments.ContainsKey("arg0"));
                Assert.False(request.Arguments.ContainsKey("arg1"));
                Assert.Equal("a", request.Arguments["arg0"]);
                Assert.True(request.Options.ContainsKey("option0"));
                Assert.True(request.Options.ContainsKey("option1"));
                Assert.True(request.Options.ContainsKey("option2"));
                Assert.True(request.Options.ContainsKey("o"));
                Assert.True(request.Options.ContainsKey("p"));
                Assert.Equal(new string[] {}, request.Options["option0"]);
                Assert.Equal(new string[] {}, request.Options["o"]);
                Assert.Equal(new[] {"a", "e"}, request.Options["option1"]);
                Assert.Equal(new[] {"a", "e"}, request.Options["p"]);
                Assert.Equal(new[] {"c"}, request.Options["option2"]);
                Assert.Equal(args, request.Args);
            }
        }
    }
}