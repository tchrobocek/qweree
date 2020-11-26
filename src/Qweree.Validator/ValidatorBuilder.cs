using System.Collections.Generic;
using Qweree.Validator.ModelValidation;

namespace Qweree.Validator
{
    public class ValidatorBuilder
    {
        private readonly List<IObjectValidator> _objectValidators = new List<IObjectValidator>();
        private readonly List<IConstraintValidator> _constraintValidators = new List<IConstraintValidator>();
        private readonly List<ModelSettings> _modelSettings = new List<ModelSettings>();

        private bool _withModelValidator;

        public void WithObjectValidator(IObjectValidator objectValidator)
        {
            _objectValidators.Add(objectValidator);
        }

        public void WithConstraintValidator(IConstraintValidator constraintValidator)
        {
            _constraintValidators.Add(constraintValidator);
        }

        public void WithModelSettings(IEnumerable<ModelSettings> modelSettings)
        {
            _modelSettings.AddRange(modelSettings);
        }

        public void WithModelValidator()
        {
            _withModelValidator = true;
        }

        public Validator Build()
        {
            if (_withModelValidator)
                _objectValidators.Add(SetupModelValidator());

            return new Validator(_objectValidators);
        }

        private ModelValidator SetupModelValidator()
        {
            var constraintValidators = _constraintValidators.ToArray();
            return new ModelValidator(_modelSettings, constraintValidators);
        }
    }
}