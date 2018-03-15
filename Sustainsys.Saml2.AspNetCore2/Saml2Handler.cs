﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sustainsys.Saml2.WebSso;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.AspNetCore2
{
    /// <summary>
    /// Authentication handler for Saml2
    /// </summary>
    public class Saml2Handler : IAuthenticationRequestHandler, IAuthenticationSignOutHandler
    {
        private readonly IOptionsMonitorCache<Saml2Options> optionsCache;

        // Internal to be visible to tests.
        internal Saml2Options options;
        HttpContext context;
        private readonly IDataProtector dataProtector;
        private readonly IOptionsFactory<Saml2Options> optionsFactory;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="optionsCache">Options</param>
        /// <param name="dataProtectorProvider">Data Protector Provider</param>
        /// <param name="optionsFactory">Factory for options</param>
        public Saml2Handler(
            IOptionsMonitorCache<Saml2Options> optionsCache,
            IDataProtectionProvider dataProtectorProvider,
            IOptionsFactory<Saml2Options> optionsFactory)
        {
            if (dataProtectorProvider == null)
            {
                throw new ArgumentNullException(nameof(dataProtectorProvider));
            }

            dataProtector = dataProtectorProvider.CreateProtector(GetType().FullName);

            this.optionsFactory = optionsFactory;
            this.optionsCache = optionsCache;
        }

        /// <InheritDoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "scheme")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "context")]
        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            this.context = context;
            options = optionsCache.GetOrAdd(scheme.Name, () => optionsFactory.Create(scheme.Name));

            return Task.CompletedTask;
        }

        /// <InheritDoc />
        [ExcludeFromCodeCoverage]
        public Task<AuthenticateResult> AuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        /// <InheritDoc />
        public async Task ChallengeAsync(AuthenticationProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            // Don't serialize the return url twice, move it to our location.
            var redirectUri = properties.RedirectUri;
            properties.RedirectUri = null;

            var requestData = context.ToHttpRequestData(null);

            var result = SignInCommand.Run(
                null,
                redirectUri,
                requestData,
                options,
                properties.Items);

            await result.Apply(context, dataProtector, null, null);
        }

        /// <InheritDoc />
        [ExcludeFromCodeCoverage]
        public Task ForbidAsync(AuthenticationProperties properties)
        {
            throw new NotImplementedException();
        }

        /// <InheritDoc />
        public async Task<bool> HandleRequestAsync()
        {
            if(context.Request.Path.StartsWithSegments(options.SPOptions.ModulePath, StringComparison.Ordinal))
            {
                var commandName = context.Request.Path.Value.Substring(
                    options.SPOptions.ModulePath.Length).TrimStart('/');

                var commandResult = CommandFactory.GetCommand(commandName).Run(
                    context.ToHttpRequestData(dataProtector.Unprotect), options);

                await commandResult.Apply(context, dataProtector, options.SignInScheme, options.SignOutScheme);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Initiate a federated sign out if supported (Idp supports it and sp has a configured
        /// signing certificate)
        /// </summary>
        /// <param name="properties">Authentication props, containing a return url.</param>
        /// <returns>Task</returns>
        public async Task SignOutAsync(AuthenticationProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            await LogoutCommand.InitiateLogout(
                context.ToHttpRequestData(dataProtector.Unprotect),
                new Uri(properties.RedirectUri, UriKind.RelativeOrAbsolute),
                options,
                // In the Asp.Net Core2 model, it's the caller's responsibility to terminate the
                // local session on an SP-initiated logout.
                terminateLocalSession: false)
                .Apply(context, dataProtector, null, null);
        }
    }
}
