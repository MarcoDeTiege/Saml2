﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sustainsys.Saml2.Tests
{
    [TestClass]
    public class ManagedSHA256SignatureDescriptionTests
    {
        [TestMethod]
        public void ManagedSha256SignatureDescription_CreateDeformatter_NullCheck()
        {
            new ManagedSHA256SignatureDescription()
                .Invoking(d => d.CreateDeformatter(null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("key");
        }

        [TestMethod]
        public void ManagedSha256SignatureDescription_CreateFormatter_NullCheck()
        {
            new ManagedSHA256SignatureDescription()
                .Invoking(d => d.CreateFormatter(null))
                .ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("key");
        }
    }
}
