using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Configuration;
using System;

namespace Sustainsys.Saml2.Tests.Configuration
{
    [TestClass]
    public class FederationCollectionTests
    {
        [TestMethod]
        public void FederationCollection_RegisterFederations_NullCheck()
        {
            var subject = new FederationCollection();

            Action a = () => subject.RegisterFederations(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }
    }
}
