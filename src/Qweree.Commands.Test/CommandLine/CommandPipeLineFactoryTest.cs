using System.Collections.Generic;
using System.Collections.Immutable;
using DeepEqual.Syntax;
using Qweree.Commands.CommandLine;
using Qweree.TestUtils.DeepEqual;
using Xunit;

namespace Qweree.Commands.Test.CommandLine
{
    public class CommandPipeLineFactoryTest
    {
        [Fact]
        public void TestCommandPipeline()
        {
            var expectedPipeLine = new List<CommandCall>
            {
                new("command1 command2", new[]{new CommandCallOption("-f", new[] {"a", "b"}.ToImmutableArray()),
                    new CommandCallOption("--file", new[] {"b"}.ToImmutableArray())}.ToImmutableArray()),
                new("command3", new[]{new CommandCallOption("-f", ImmutableArray<string>.Empty)}.ToImmutableArray()),
                new("command4", new[]{new CommandCallOption("-a", ImmutableArray<string>.Empty), new CommandCallOption("-s", ImmutableArray<string>.Empty), new CommandCallOption("-d", ImmutableArray<string>.Empty), new CommandCallOption("-f", ImmutableArray<string>.Empty)}.ToImmutableArray())
            };

            var args = new[] {"command1", "command2", "-f", "a", "--file", "b", "-f", "b", "command3", "-f"};

            var actualPipeline = CommandPipelineFactory.CreatePipeline(args);
            actualPipeline.ShouldDeepEqual(expectedPipeLine, new ImmutableArrayComparison(new ImmutableArrayComparison(new ImmutableArrayComparison())));
        }
    }
}