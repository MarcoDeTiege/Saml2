﻿using Sustainsys.Saml2.Internal;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace Sustainsys.Saml2.Saml2P
{
    /// <summary>
    /// Saml2 Soap binding implementation.
    /// </summary>
    /// <remarks>
    /// This class does not follow the pattern of the other three bindings
    /// (Redirect, POST and Artifact) because it does not use the front channel
    /// with messages being passed over the user's browser.
    /// </remarks>
    public static class Saml2SoapBinding
    {
        const string soapFormatString = "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\"><SOAP-ENV:Body>{0}</SOAP-ENV:Body></SOAP-ENV:Envelope>";

        /// <summary>
        /// Create a SOAP body around a specified payload.
        /// </summary>
        /// <param name="payload">Payload of the message.</param>
        /// <returns></returns>
        public static string CreateSoapBody(string payload)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                soapFormatString,
                payload
            );
        }

        /// <summary>
        /// Extract the body of a SOAP message.
        /// </summary>
        /// <param name="xml">xml data</param>
        /// <returns>Parsed data.</returns>
        public static XmlElement ExtractBody(string xml)
        {
            var xmlDoc = XmlHelpers.XmlDocumentFromString(xml);

            return xmlDoc.DocumentElement["Body", Saml2Namespaces.SoapEnvelopeName]
                .ChildNodes.OfType<XmlElement>().Single();
        }

        /// <summary>
        /// Send a SOAP request to the specified endpoint and return the result.
        /// </summary>
        /// <param name="payload">Message payload</param>
        /// <param name="destination">Destination endpoint</param>
        /// <param name="signingServiceCertificate"></param>
        /// <param name="resolver"></param>
        /// <returns>Response.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Tls",
            Justification = "TLS is a well known abbreviation for Transport Layer Security")]
        public static XmlElement SendSoapRequest(string payload, Uri destination,
            X509Certificate2 signingServiceCertificate, Func<Uri, string, string> resolver)
        {
            AssertDestinationIsValid(destination);

            if (resolver == null)
            {
                resolver = DefaultArtifactResolver;
            }

            var message = BuildSoapMesssage(payload, signingServiceCertificate);

            var response = resolver(destination, message);

            return ExtractBody(response);
        }

        private static string DefaultArtifactResolver(Uri destination, string message)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("SOAPAction", "http://www.oasis-open.org/committees/security");

                var response = client.UploadString(destination, message);

                return response;
            }
        }

        private static void AssertDestinationIsValid(Uri destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            switch (destination.Scheme)
            {
                case "http":
                case "https":
                    break;
                default:
                    throw new ArgumentException("The Uri scheme " + destination.Scheme +
                                                " is not allowed for outbound SOAP messages. Only http or https URLs are allowed.");
            }
        }

        private static string BuildSoapMesssage(string payload, X509Certificate2 clientCertificate)
        {
            if (clientCertificate != null)
            {
                var xmlDoc = new XmlDocument
                {
                    PreserveWhitespace = true
                };

                xmlDoc.LoadXml(payload);
                xmlDoc.Sign(clientCertificate, true);
                payload = xmlDoc.OuterXml;
            }

            var message = CreateSoapBody(payload);

            return message;
        }
    }
}
