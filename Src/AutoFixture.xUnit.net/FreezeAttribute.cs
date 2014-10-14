using System;
using System.Reflection;
using Ploeh.AutoFixture.Kernel;

namespace Ploeh.AutoFixture.Xunit
{
    /// <summary>
    /// An attribute that can be applied to parameters in an <see cref="AutoDataAttribute"/>-driven
    /// Theory to indicate that the parameter value should be frozen and used to satisfy any requests
    /// made through the same <see cref="IFixture"/> and that match a set of criteria.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class FreezeAttribute : CustomizeAttribute
    {
        private IRequestSpecification matcher;
        private Type targetType;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezeAttribute"/> class.
        /// </summary>
        public FreezeAttribute()
        {
            this.By = Matching.ExactType;
        }

        /// <summary>
        /// Gets or sets the <see cref="Matching"/> criteria used to determine
        /// which requests will be satisfied by the frozen parameter value.
        /// </summary>
        /// <remarks>
        /// If not specified, requests will be matched by exact type.
        /// </remarks>
        public Matching By { get; set; }

        /// <summary>
        /// Gets or sets the identifier used to determine which requests
        /// for a class member will be satisfied by the frozen parameter value.
        /// </summary>
        public string TargetName { get; set; }

        /// <summary>
        /// Gets a <see cref="FreezeOnMatchCustomization"/> configured
        /// to match requests based on the <see cref="Type"/> of the parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter for which the customization is requested.
        /// </param>
        /// <returns>
        /// A <see cref="FreezeOnMatchCustomization"/> configured
        /// to match requests based on the <see cref="Type"/> of the parameter.
        /// </returns>
        public override ICustomization GetCustomization(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            this.targetType = parameter.ParameterType;

            MatchByType();
            MatchByName();
            return FreezeCustomizationForTargetType();
        }

        private void MatchByType()
        {
            AlwaysMatchByExactType();
            MatchByBaseType();
            MatchByImplementedInterfaces();
        }

        private void MatchByName()
        {
            MatchByPropertyName();
            MatchByParameterName();
            MatchByFieldName();
        }

        private void AlwaysMatchByExactType()
        {
            MatchBy(
                new OrRequestSpecification(
                    new ExactTypeSpecification(targetType),
                    new SeedRequestSpecification(targetType)));
        }

        private void MatchByBaseType()
        {
            if (ShouldMatchBy(Matching.DirectBaseType))
            {
                MatchBy(new DirectBaseTypeSpecification(targetType));
            }
        }

        private void MatchByImplementedInterfaces()
        {
            if (ShouldMatchBy(Matching.ImplementedInterfaces))
            {
                MatchBy(new ImplementedInterfaceSpecification(targetType));
            }
        }

        private void MatchByParameterName()
        {
            if (ShouldMatchBy(Matching.ParameterName))
            {
                MatchBy(new ParameterSpecification(targetType, TargetName));
            }
        }

        private void MatchByPropertyName()
        {
            if (ShouldMatchBy(Matching.PropertyName))
            {
                MatchBy(new PropertySpecification(targetType, TargetName));
            }
        }

        private void MatchByFieldName()
        {
            if (ShouldMatchBy(Matching.FieldName))
            {
                MatchBy(new FieldSpecification(targetType, TargetName));
            }
        }

        private bool ShouldMatchBy(Matching criteria)
        {
            return By.HasFlag(criteria);
        }

        private void MatchBy(IRequestSpecification criteria)
        {
            this.matcher = this.matcher == null
                ? criteria
                : new OrRequestSpecification(this.matcher, criteria);
        }

        private ICustomization FreezeCustomizationForTargetType()
        {
            return new FreezeOnMatchCustomization(targetType, matcher);
        }
    }
}
