﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Sustainsys.Saml2.WebSso;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Security;

namespace Sustainsys.Saml2.HttpModule.Tests
{
    [TestClass]
    public class HttpRequestBaseExtensionsTests
    {
        [TestMethod]
        public void HttpRequestBaseExtensions_ToHttpRequestData_ShouldThrowOnNull()
        {
            HttpRequestBase request = null;
            Action a = () => request.ToHttpRequestData();

            a.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("requestBase");
        }

        [TestMethod]
        public void HttpRequestBaseExtensions_ToHttpRequestData()
        {
            var url = new Uri("http://example.com:42/ApplicationPath/Path?RelayState=SomeState");
            string appPath = "/ApplicationPath";

            var request = Substitute.For<HttpRequestBase>();
            request.HttpMethod.Returns("GET");
            request.Url.Returns(url);
            request.Form.Returns(new NameValueCollection { { "Key", "Value" } });
            request.ApplicationPath.Returns(appPath);

            var cookieValue = HttpRequestData.ConvertBinaryData(
                MachineKey.Protect(
                    new StoredRequestState(null, new Uri("urn:someUri"), null, null).Serialize(),
                    HttpRequestBaseExtensions.ProtectionPurpose));
            request.Cookies.Returns(new HttpCookieCollection());
            request.Cookies.Add(new HttpCookie(StoredRequestState.CookieNameBase + "SomeState", cookieValue));

            var actual = request.ToHttpRequestData();

            var expected = new HttpRequestData(
                "GET",
                url,
                appPath,
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("Key", new string[] { "Value" })
                },
                Enumerable.Empty<KeyValuePair<string, string>>(),
                null, 
                ClaimsPrincipal.Current);

            actual.ShouldBeEquivalentTo(expected, opt => opt.Excluding(s => s.StoredRequestState));
            actual.StoredRequestState.ReturnUrl.AbsoluteUri.Should().Be("urn:someUri");
        }

        [TestMethod]
        public void HttpRequestBaseExtensions_ToHttpRequestData_IgnoreCookieFlag()
        {
            var url = new Uri("http://example.com:42/ApplicationPath/Path?RelayState=SomeState");
            string appPath = "/ApplicationPath";

            var request = Substitute.For<HttpRequestBase>();
            request.HttpMethod.Returns("GET");
            request.Url.Returns(url);
            request.Form.Returns(new NameValueCollection { { "Key", "Value" } });
            request.ApplicationPath.Returns(appPath);

            var cookieValue = "SomethingThatCannotBeDecrypted";
            request.Cookies.Returns(new HttpCookieCollection());
            request.Cookies.Add(new HttpCookie("Sustainsys.SomeState", cookieValue));

            var subject = request.ToHttpRequestData(true);

            var expected = new HttpRequestData(
                "GET",
                url,
                appPath,
                new KeyValuePair<string, IEnumerable<string>>[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("Key", new string[] { "Value" })
                },
                Enumerable.Empty<KeyValuePair<string, string>>(),
                null,
                ClaimsPrincipal.Current);

            subject.ShouldBeEquivalentTo(expected);
        }
    }
}
