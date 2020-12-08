using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Qweree.Validator.Extensions;
using Xunit;

namespace Qweree.Validator.Test.Extensions
{
    public class ValidatorExtensionsTest
    {
        [Fact]
        public async Task TestValidate_Objects()
        {
            var subjects = new[]
            {
                "hello",
                1,
                new object()
            };

            const string warning = "warning";
            const string error = "error";

            var validatorMock = new Mock<IValidator>();
            var validator = validatorMock.Object;

            validatorMock.Setup(m => m.ValidateAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns<string, object>((path, subject) =>
                {
                    var result = new ValidationResult(ValidationStatus.Failed,
                        new[] {new ValidationMessage(path, warning)}, new[] {new ValidationMessage(path, error)});
                    return Task.FromResult(result);
                });

            var actualResult = await validator.ValidateManyAsync(subjects);

            validatorMock.Verify(m => m.ValidateAsync("0", subjects[0], It.IsAny<CancellationToken>()), Times.Once());
            validatorMock.Verify(m => m.ValidateAsync("1", subjects[1], It.IsAny<CancellationToken>()), Times.Once());
            validatorMock.Verify(m => m.ValidateAsync("2", subjects[2], It.IsAny<CancellationToken>()), Times.Once());


            Assert.Equal(new[] {warning, warning, warning}, actualResult.Warnings.Select(w => w.Message).ToArray());
            Assert.Equal(new[] {"0", "1", "2"}, actualResult.Warnings.Select(w => w.Path).ToArray());
            Assert.Equal(new[] {error, error, error}, actualResult.Errors.Select(e => e.Message).ToArray());
            Assert.Equal(new[] {"0", "1", "2"}, actualResult.Errors.Select(e => e.Path).ToArray());
        }
        [Fact]
        public async Task TestValidate_KeyValuePairs()
        {
            var subjects = new[]
            {
                new KeyValuePair<string, object>("a", "hello"),
                new KeyValuePair<string, object>("b", 1),
                new KeyValuePair<string, object>("c", new object())
            };

            const string warning = "warning";
            const string error = "error";

            var validatorMock = new Mock<IValidator>();
            var validator = validatorMock.Object;

            validatorMock.Setup(m => m.ValidateAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns<string, object>((path, subject) =>
                {
                    var result = new ValidationResult(ValidationStatus.Failed,
                        new[] {new ValidationMessage(path, warning)}, new[] {new ValidationMessage(path, error)});
                    return Task.FromResult(result);
                });

            var actualResult = await validator.ValidateManyAsync(subjects);

            validatorMock.Verify(m => m.ValidateAsync("a", subjects[0].Value, It.IsAny<CancellationToken>()), Times.Once());
            validatorMock.Verify(m => m.ValidateAsync("b", subjects[1].Value, It.IsAny<CancellationToken>()), Times.Once());
            validatorMock.Verify(m => m.ValidateAsync("c", subjects[2].Value, It.IsAny<CancellationToken>()), Times.Once());

            Assert.Equal(new[] {warning, warning, warning}, actualResult.Warnings.Select(w => w.Message).ToArray());
            Assert.Equal(new[] {"a", "b", "c"}, actualResult.Warnings.Select(w => w.Path).ToArray());
            Assert.Equal(new[] {error, error, error}, actualResult.Errors.Select(e => e.Message).ToArray());
            Assert.Equal(new[] {"a", "b", "c"}, actualResult.Errors.Select(e => e.Path).ToArray());
        }
    }
}