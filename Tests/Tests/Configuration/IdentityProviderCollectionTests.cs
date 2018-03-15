﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Configuration;
using System;

namespace Sustainsys.Saml2.Tests.Configuration
{
    [TestClass]
    public class IdentityProviderCollectionTests
    {
        [TestMethod]
        public void IdentityProviderCollection_RegisterIdentityProvider_NullCheck()
        {
            var subject = new IdentityProviderCollection();

            Action a = () => subject.RegisterIdentityProviders(null);

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("options");
        }
    }
}
