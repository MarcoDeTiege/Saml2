using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.TestHelpers;
using System;
using System.IdentityModel.Metadata;

namespace Sustainsys.Saml2.Tests.Configuration
{
    [TestClass]
    public class IdentityProviderDictionaryTests
    {
        [TestMethod]
        public void IdentityProviderDictionary_Indexer_Nullcheck()
        {
            var subject = new IdentityProviderDictionary();

            Action a = () => { var i = subject[null]; };

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("entityId");
        }

        [TestMethod]
        public void IdentityProviderDictionary_Add()
        {
            var subject = new IdentityProviderDictionary();

            var entityId = new EntityId("http://idp.example.com");
            var idp = new IdentityProvider(entityId, StubFactory.CreateSPOptions());

            subject.Add(idp);

            subject[entityId].Should().BeSameAs(idp);
        }

        [TestMethod]
        public void IdentityProviderDictionary_Add_Nullcheck()
        {
            var subject = new IdentityProviderDictionary();

            Action a = () => subject.Add(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("idp");
        }
    }
}
