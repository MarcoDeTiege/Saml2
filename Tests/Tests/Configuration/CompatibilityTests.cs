using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Configuration;
using System;

namespace Sustainsys.Saml2.Tests.Configuration
{
    [TestClass]
    public class CompatibilityTests
    {
        [TestMethod]
        public void Compatibility_Ctor_Nullcheck()
        {
            Action a = () => new Compatibility(null);

            a.ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("configElement");
        }
    }
}
