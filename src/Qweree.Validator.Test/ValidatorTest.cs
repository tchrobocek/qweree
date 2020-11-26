using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Qweree.Validator.Test
{
    public class ValidatorTest
    {
        [Fact]
        public async Task TestValidator_Objects()
        {
            const string text = "string";

            var validatorMock1 = new Mock<IObjectValidator>();
            var validatorMock2 = new Mock<IObjectValidator>();

            var validator = new Validator(new[] {validatorMock1.Object, validatorMock2.Object});

            await validator.ValidateAsync(text);

            validatorMock1.Setup(m => m.Supports(typeof(string)))
                .Returns(true);
            validatorMock1.Setup(m => m.ValidateAsync(It.IsAny<ValidationContext>(), It.IsAny<ValidationBuilder>()))
                .Callback<ValidationContext, ValidationBuilder>((e, b) => b.AddError("", "1"));

            validatorMock2.Setup(m => m.Supports(typeof(string)))
                .Returns(true);
            validatorMock2.Setup(m => m.ValidateAsync(It.IsAny<ValidationContext>(), It.IsAny<ValidationBuilder>()))
                .Callback<ValidationContext, ValidationBuilder>((e, b) => b.AddError("", "2"));

            var result = await validator.ValidateAsync(text);

            Assert.Equal(ValidationStatus.Failed, result.Status);
            Assert.Equal(new[] {"1", "2"}, result.Errors.Select(e => e.Message));
            Assert.Empty(result.Warnings);
        }

        [Fact]
        public async Task TestValidator_ValidationSubjects()
        {
            const string text = "string";
            const string path = "path";

            var validatorMock1 = new Mock<IObjectValidator>();
            var validatorMock2 = new Mock<IObjectValidator>();

            var validator = new Validator(new[] {validatorMock1.Object, validatorMock2.Object});

            {
                var result = await validator.ValidateAsync(path, text);
                Assert.Equal(ValidationStatus.Succeeded, result.Status);
                Assert.Empty(result.Warnings);
                Assert.Empty(result.Errors);
            }

            validatorMock1.Setup(m => m.Supports(typeof(string)))
                .Returns(true);
            validatorMock1.Setup(m => m.ValidateAsync(It.IsAny<ValidationContext>(), It.IsAny<ValidationBuilder>()))
                .Callback<ValidationContext, ValidationBuilder>((e, b) => b.AddError(e.Path, "1"));

            validatorMock2.Setup(m => m.Supports(typeof(string)))
                .Returns(true);
            validatorMock2.Setup(m => m.ValidateAsync(It.IsAny<ValidationContext>(), It.IsAny<ValidationBuilder>()))
                .Callback<ValidationContext, ValidationBuilder>((e, b) => b.AddError(e.Path, "2"));

            {
                var result = await validator.ValidateAsync(path, text);

                Assert.Equal(ValidationStatus.Failed, result.Status);
                Assert.Equal(new[] {"1", "2"}, result.Errors.Select(e => e.Message));
                Assert.Equal(new[] {path, path}, result.Errors.Select(e => e.Path));
                Assert.Empty(result.Warnings);
            }
        }
    }
}