using System.Linq;
using Xunit;

namespace Qweree.Validator.Test
{
    public class ValidationBuilderTest
    {
        [InlineData(ValidationStatus.Succeeded, new string[0], new string[0])]
        [InlineData(ValidationStatus.Succeeded, new[] {"a", "b"}, new string[0])]
        [InlineData(ValidationStatus.Failed, new string[0], new[] {"a", "b"})]
        [InlineData(ValidationStatus.Failed, new[] {"a", "b"}, new[] {"c", "d"})]
        [Theory]
        public void TestBuild(ValidationStatus expectedStatus, string[] warnings, string[] errors)
        {
            var builder = new ValidationBuilder();

            foreach (string error in errors)
                builder.AddError("", error);
            foreach (string warning in warnings)
                builder.AddWarning("", warning);

            var result = builder.Build();
            Assert.Equal(expectedStatus, result.Status);
            Assert.Equal(errors, result.Errors.Select(e => e.Message));
            Assert.Equal(warnings, result.Warnings.Select(e => e.Message));
        }
    }
}