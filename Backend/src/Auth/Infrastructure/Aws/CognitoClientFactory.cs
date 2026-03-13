using System;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Microsoft.Extensions.Configuration;

namespace Auth.Infrastructure.Aws
{
    /// <summary>
    /// Factory that creates configured AmazonCognitoIdentityProviderClient instances.
    /// The factory itself is lightweight; the actual client is created when requested
    /// so the DI container can manage the client's lifecycle (dispose at shutdown).
    /// </summary>
    public class CognitoClientFactory
    {
        private readonly string _region;

        public CognitoClientFactory(IConfiguration configuration)
        {
            _region = configuration["AWS:Region"] ?? throw new InvalidOperationException("AWS:Region not configured.");
        }

        public AmazonCognitoIdentityProviderClient CreateClient()
        {
            return new AmazonCognitoIdentityProviderClient(RegionEndpoint.GetBySystemName(_region));
        }
    }
}