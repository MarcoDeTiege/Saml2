using System;
using System.IdentityModel.Metadata;

namespace Sustainsys.Saml2.Metadata
{
    /// <summary>
    /// Extended entitiesdescriptor for SAML2 metadata, adding more attributes.
    /// </summary>
    public class ExtendedEntitiesDescriptor : EntitiesDescriptor, ICachedMetadata
    {
        /// <summary>
        /// Permitted cache duration for the metadata.
        /// </summary>
        public TimeSpan? CacheDuration { get; set; }

        /// <summary>
        /// Valid until
        /// </summary>
        public DateTime? ValidUntil { get; set; }
    }
}
