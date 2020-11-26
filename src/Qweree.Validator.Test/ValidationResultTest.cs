using Xunit;

namespace Qweree.Validator.Test
{
    public class ValidationResultTest
    {
        [Theory]
        [InlineData(ValidationStatus.Failed, false)]
        [InlineData(ValidationStatus.Succeeded, true)]
        public void TestHasSucceeded(ValidationStatus validationStatus, bool expectedValue)
        {
            var result = new ValidationResult(validationStatus, new ValidationMessage[0], new ValidationMessage[0]);
            Assert.Equal(expectedValue, result.HasSucceeded);
        }
        [Theory]
        [InlineData(ValidationStatus.Failed, true)]
        [InlineData(ValidationStatus.Succeeded, false)]
        public void TestHasFailed(ValidationStatus validationStatus, bool expectedValue)
        {
            var result = new ValidationResult(validationStatus, new ValidationMessage[0], new ValidationMessage[0]);
            Assert.Equal(expectedValue, result.HasFailed);
        }
    }
}