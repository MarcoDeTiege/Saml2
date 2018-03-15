using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using System;

namespace Sustainsys.Saml2.WebSso
{
    /// <summary>
    /// Represents the service provider metadata command behaviour.
    /// Instances of this class can be created directly or by using the factory method
    /// CommandFactory.GetCommand(CommandFactory.MetadataCommandName).
    /// </summary>
    public class MetadataCommand : ICommand
    {
        /// <summary>
        /// Run the command, creating and returning the service provider metadata.
        /// </summary>
        /// <param name="request">Request data.</param>
        /// <param name="options">Options</param>
        /// <returns>CommandResult</returns>
        public CommandResult Run(HttpRequestData request, IOptions options)
        {
            if(options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var urls = new Saml2Urls(request, options);

            var metadata = options.SPOptions.CreateMetadata(urls);
            options.Notifications.MetadataCreated(metadata, urls);

            var result = new CommandResult()
            {
                Content = metadata.ToXmlString(
                    options.SPOptions.SigningServiceCertificate,
                    options.SPOptions.OutboundSigningAlgorithm),
                ContentType = "application/samlmetadata+xml"
            };

            options.Notifications.MetadataCommandResultCreated(result);

            options.SPOptions.Logger.WriteInformation("Created metadata");

            return result;
        }
    }
}
