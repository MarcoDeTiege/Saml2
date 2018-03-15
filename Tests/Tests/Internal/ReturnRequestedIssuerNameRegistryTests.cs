﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sustainsys.Saml2.Internal;
using System;

namespace Sustainsys.Saml2.Tests.Internal
{
    [TestClass]
    public class ReturnRequestedIssuerNameRegistryTests
    {
        [TestMethod]
        public void ReturnRequestedIssuerNameRegistry_GetIssuerNameRequested()
        {
            var name = "SomeName";

            new ReturnRequestedIssuerNameRegistry().GetIssuerName(
                null, name).Should().Be(name);
        }

        [TestMethod]
        public void ReturnRequestedIssuerNameRegistry_GetIssuerName()
        {
            Action a = () => new ReturnRequestedIssuerNameRegistry().GetIssuerName(null);

            a.ShouldThrow<NotImplementedException>();
        }
    }
}
