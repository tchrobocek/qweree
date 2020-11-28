﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qweree.Validator
{
    /// <summary>
    ///     Validates given entities.
    /// </summary>
    public class Validator : IValidator
    {
        private readonly List<IObjectValidator> _validators = new List<IObjectValidator>();

        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="objectValidators">Collection of object validators.s</param>
        public Validator(IEnumerable<IObjectValidator> objectValidators)
        {
            _validators.AddRange(objectValidators);
        }

        public IObjectValidator[] ObjectValidators => _validators.ToArray();

        /// <summary>
        ///     Validates given subjects.
        /// </summary>
        /// <param name="subject">Subject.</param>
        public async Task<ValidationResult> ValidateAsync(object subject)
        {
            return await ValidateAsync("", subject);
        }

        /// <summary>
        ///     Validates given subjects.
        /// </summary>
        /// <param name="path">Path to subject.</param>
        /// <param name="subject">Subject.</param>
        public async Task<ValidationResult> ValidateAsync(string path, object subject)
        {
            var builder = new ValidationBuilder();

            await ValidateModelAsync(path, subject, builder)
                .ConfigureAwait(false);

            var result = builder.Build();
            return result;
        }

        private async Task ValidateModelAsync(string path, object subject, ValidationBuilder builder)
        {
            var type = subject.GetType();

            var matching = _validators.Where(v => v.Supports(type));

            foreach (var validator in matching)
            {
                await validator.ValidateAsync(new ValidationContext(path, subject, null), builder)
                    .ConfigureAwait(false);
            }
        }
    }
}