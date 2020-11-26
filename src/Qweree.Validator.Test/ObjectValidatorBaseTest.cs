using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Qweree.Validator.Test
{
    public class ObjectValidatorBaseTest
    {
        [Fact]
        public async Task TestObjectValidatorBase()
        {
            const int subject = 0;
            var subjectType = subject.GetType();
            var mock = new Mock<ObjectValidatorBase<int>>();

            var validator = mock.Object;
            Assert.True(validator.Supports(subjectType));
            await validator.ValidateAsync(new ValidationContext("", subject), new ValidationBuilder());
        }

        [Fact]
        public async Task TestObjectValidatorBase_WrongSubject()
        {
            const string subject = "subject";
            var subjectType = subject.GetType();
            var mock = new Mock<ObjectValidatorBase<int>>();

            var validator = mock.Object;
            Assert.False(validator.Supports(subjectType));

            await Assert.ThrowsAsync<InvalidCastException>(async () =>
                await validator.ValidateAsync(new ValidationContext("", subject), new ValidationBuilder()));
        }
    }
}