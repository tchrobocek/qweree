using System;
using Qweree.Validator;

namespace Qweree.TestUtils.Qweree.Validator
{
    public class EmptyValidator : global::Qweree.Validator.Validator
    {
        public EmptyValidator() : base(Array.Empty<IObjectValidator>())
        {
        }
    }
}