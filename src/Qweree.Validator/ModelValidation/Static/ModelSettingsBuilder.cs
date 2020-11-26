using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Qweree.Validator.ModelValidation.Static
{
    /// <summary>
    ///     Model settings builder.
    /// </summary>
    public class ModelSettingsBuilder
    {
        private readonly List<PropertySettingsBuilder> _properties = new List<PropertySettingsBuilder>();

        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="subjectType">Settings subject type.</param>
        public ModelSettingsBuilder(Type subjectType)
        {
            SubjectType = subjectType;
        }

        /// <summary>
        ///     Settings subject type.
        /// </summary>
        public Type SubjectType { get; }

        /// <summary>
        ///     Adds property settings
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <returns>Property settings builder.</returns>
        /// <exception cref="ArgumentException">Thrown when property is already mapped.</exception>
        public PropertySettingsBuilder AddProperty(string name)
        {
            if (_properties.Any(p => p.PropertyName == name))
                throw new ArgumentException("Property was already added.");

            var builder = new PropertySettingsBuilder(name);
            _properties.Add(builder);

            return builder;
        }

        /// <summary>
        ///     Builds settings.
        /// </summary>
        /// <returns>Built settings.</returns>
        public ModelSettings Build()
        {
            return new ModelSettings(SubjectType, _properties.Select(p => p.Build()));
        }
    }

    /// <summary>
    ///     Generically typed model settings builder.
    /// </summary>
    /// <typeparam name="TModelType"></typeparam>
    public class ModelSettingsBuilder<TModelType> : ModelSettingsBuilder
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        public ModelSettingsBuilder() : base(typeof(TModelType))
        {
        }

        /// <summary>
        ///     Adds property settings by expression.
        /// </summary>
        /// <param name="propertySettingsExpression">Property settings expression.</param>
        /// <returns>Property settings builder.</returns>
        /// <exception cref="ArgumentException">Thrown when property is already mapped.</exception>
        public PropertySettingsBuilder AddProperty(Expression<Func<TModelType, object>> propertySettingsExpression)
        {
            PropertyInfo propertyInfo;

            switch (propertySettingsExpression.Body)
            {
                case UnaryExpression unaryExp when unaryExp.Operand is MemberExpression memberExp:
                    propertyInfo = (PropertyInfo) memberExp.Member;
                    break;
                case MemberExpression memberExp:
                    propertyInfo = (PropertyInfo) memberExp.Member;
                    break;
                default:
                    throw new ArgumentException(
                        $"The expression doesn't indicate a valid property. [ {propertySettingsExpression} ]");
            }

            return AddProperty(propertyInfo.Name);
        }
    }
}