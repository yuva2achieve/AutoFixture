using System;
using System.Linq;
using System.Reflection;
using Ploeh.AutoFixture.Kernel;
using Ploeh.TestTypeFoundation;
using Xunit;

namespace Ploeh.AutoFixture.Xunit.UnitTest
{
    public class FreezeAttributeTest
    {
        [Fact]
        public void SutIsCustomizeAttribute()
        {
            // Fixture setup
            // Exercise system
            var sut = new FreezeAttribute();
            // Verify outcome
            Assert.IsAssignableFrom<CustomizeAttribute>(sut);
            // Teardown
        }

        [Fact]
        public void InitializeShouldSetDefaultMatchingStrategy()
        {
            // Fixture setup
            // Exercise system
            var sut = new FreezeAttribute();
            // Verify outcome
            Assert.Equal(Matching.ExactType, sut.By);
            // Teardown
        }

        [Fact]
        public void InitializeShouldSetDefaultTargetName()
        {
            // Fixture setup
            // Exercise system
            var sut = new FreezeAttribute();
            // Verify outcome
            Assert.Null(sut.TargetName);
            // Teardown
        }

        [Fact]
        public void GetCustomizationShouldReturnFreezeOnMatchCustomization()
        {
            // Fixture setup
            var sut = new FreezeAttribute();
            // Exercise system
            var customization = sut.GetCustomization(AParameter<ConcreteType>());
            // Verify outcome
            Assert.IsAssignableFrom<FreezeOnMatchCustomization>(customization);
            // Teardown
        }

        [Fact]
        public void GetCustomizationShouldMatchByExactParameterType()
        {
            // Fixture setup
            var parameter = AParameter<object>();
            var sut = new FreezeAttribute();
            // Exercise system
            var customization = (FreezeOnMatchCustomization)sut.GetCustomization(parameter);
            // Verify outcome
            var matcher = Assert.IsType<OrRequestSpecification>(customization.Matcher);
            var exactTypeMatcher = matcher.Specifications.OfType<ExactTypeSpecification>().SingleOrDefault();
            Assert.NotNull(exactTypeMatcher);
            Assert.Equal(parameter.ParameterType, exactTypeMatcher.TargetType);
            // Teardown
        }

        [Fact]
        public void GetCustomizationShouldMatchBySeedRequestForParameterType()
        {
            // Fixture setup
            var parameter = AParameter<object>();
            var sut = new FreezeAttribute();
            // Exercise system
            var customization = (FreezeOnMatchCustomization)sut.GetCustomization(parameter);
            // Verify outcome
            var matcher = Assert.IsType<OrRequestSpecification>(customization.Matcher);
            var seedRequestMatcher = matcher.Specifications.OfType<SeedRequestSpecification>().SingleOrDefault();
            Assert.NotNull(seedRequestMatcher);
            Assert.Equal(parameter.ParameterType, seedRequestMatcher.TargetType);
            // Teardown
        }

        [Fact]
        public void GetCustomizationWithMatchingByDirectBaseTypeShouldMatchByBaseType()
        {
            // Fixture setup
            var sut = new FreezeAttribute { By = Matching.DirectBaseType };
            // Exercise system
            var customization = (FreezeOnMatchCustomization)sut.GetCustomization(AParameter<object>());
            // Verify outcome
            var matcher = Assert.IsType<OrRequestSpecification>(customization.Matcher);
            Assert.NotEmpty(matcher.Specifications.OfType<DirectBaseTypeSpecification>());
            // Teardown
        }

        [Fact]
        public void GetCustomizationWithMatchingByImplementedInterfacesShouldMatchByImplementedInterfaces()
        {
            // Fixture setup
            var sut = new FreezeAttribute { By = Matching.ImplementedInterfaces };
            // Exercise system
            var customization = (FreezeOnMatchCustomization)sut.GetCustomization(AParameter<object>());
            // Verify outcome
            var matcher = Assert.IsType<OrRequestSpecification>(customization.Matcher);
            Assert.NotEmpty(matcher.Specifications.OfType<ImplementedInterfaceSpecification>());
            // Teardown
        }

        [Fact]
        public void GetCustomizationWithMatchingByParameterNameShouldMatchByParameter()
        {
            // Fixture setup
            var sut = new FreezeAttribute { By = Matching.ParameterName, TargetName = "parameter" };
            // Exercise system
            var customization = (FreezeOnMatchCustomization)sut.GetCustomization(AParameter<object>());
            // Verify outcome
            var matcher = Assert.IsType<OrRequestSpecification>(customization.Matcher);
            Assert.NotEmpty(matcher.Specifications.OfType<ParameterSpecification>());
            // Teardown
        }

        [Fact]
        public void GetCustomizationWithMatchingByPropertyNameShouldMatchByProperty()
        {
            // Fixture setup
            var sut = new FreezeAttribute { By = Matching.PropertyName, TargetName = "Property" };
            // Exercise system
            var customization = (FreezeOnMatchCustomization)sut.GetCustomization(AParameter<object>());
            // Verify outcome
            var matcher = Assert.IsType<OrRequestSpecification>(customization.Matcher);
            Assert.NotEmpty(matcher.Specifications.OfType<PropertySpecification>());
            // Teardown
        }

        [Fact]
        public void GetCustomizationWithMatchingByFieldNameShouldMatchByField()
        {
            // Fixture setup
            var sut = new FreezeAttribute { By = Matching.FieldName, TargetName = "Field" };
            // Exercise system
            var customization = (FreezeOnMatchCustomization)sut.GetCustomization(AParameter<object>());
            // Verify outcome
            var matcher = Assert.IsType<OrRequestSpecification>(customization.Matcher);
            Assert.NotEmpty(matcher.Specifications.OfType<FieldSpecification>());
            // Teardown
        }

        [Fact]
        public void GetCustomizationWithNullShouldThrowArgumentNullException()
        {
            // Fixture setup
            var sut = new FreezeAttribute();
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() => sut.GetCustomization(null));
        }

        private static ParameterInfo AParameter<T>()
        {
            return typeof(SingleParameterType<T>)
                .GetConstructor(new[] { typeof(T) })
                .GetParameters()
                .Single(p => p.Name == "parameter");
        }
    }
}
