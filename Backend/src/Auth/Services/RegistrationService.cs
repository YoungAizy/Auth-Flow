using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Auth.Data;
using Auth.Data.DTOs;
using Auth.Infrastructure.Aws;
using Auth.Infrastructure.Mappers;
using Auth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Npgsql;

namespace Auth.Services
{
    public class RegistrationService
    {
        //private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
        //private readonly string _userPoolId = "YOUR_USER_POOL_ID";
        //private readonly string _clientId = "YOUR_APP_CLIENT_ID";

        private readonly AppDBContext _context;
        private readonly UserMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
        private readonly string _userPoolId;
        private readonly string _clientId;

        public RegistrationService(
            AppDBContext context,
            UserMapper mapper,
            IMemoryCache cache,
            CognitoClientFactory cognitoFactory,
            IConfiguration configuration)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            if (cognitoFactory is null) throw new ArgumentNullException(nameof(cognitoFactory));
            _cognitoClient = cognitoFactory.CreateClient() ?? throw new InvalidOperationException("Cognito client factory returned null.");

            // Read optional Cognito configuration from appsettings (fallback to placeholders)
            _userPoolId = configuration?["AWS:Cognito:UserPoolId"] ?? "YOUR_USER_POOL_ID";
            _clientId = configuration?["AWS:Cognito:ClientId"] ?? "YOUR_APP_CLIENT_ID";
        }
        //public RegistrationService()
        //{
        //    // Configure the AWS Region, e.g., RegionEndpoint.USEast1
        //    _cognitoClient = new AmazonCognitoIdentityProviderClient(RegionEndpoint.GetBySystemName("YOUR_AWS_REGION"));
        //}

        public async Task<SignUpResponse> SignUpUserAsync(RegistrationRequest request)
        {
            var signUpRequest = new SignUpRequest
            {
                ClientId = _clientId,
                Username = request.Email,
                Password = request.Password,
                UserAttributes = new List<AttributeType>
                {
                    new AttributeType { Name = "email", Value = request.Email }
                }
            };

            try
            {
                SignUpResponse response = await _cognitoClient.SignUpAsync(signUpRequest);
                return response;
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                throw;
            }
        }

        // After successful sign up, the user needs to be confirmed.
        public async Task<ConfirmSignUpResponse> ConfirmUserSignUpAsync(string username, string confirmationCode)
        {
            var confirmSignUpRequest = new ConfirmSignUpRequest
            {
                ClientId = _clientId,
                Username = username,
                ConfirmationCode = confirmationCode
            };

            try
            {
                ConfirmSignUpResponse response = await _cognitoClient.ConfirmSignUpAsync(confirmSignUpRequest);
                Console.WriteLine($"User {username} confirmed successfully.");
                return response;
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                Console.WriteLine($"Confirmation failed: {ex.Message}");
                throw;
            }
        }
        public void Registration() 
        {

        }

        public async Task<ServiceResult> SaveProfile(
            RegistrationRequest userForm, CancellationToken ct) 
        {
            User user = _mapper.ToEntity(userForm);

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync(ct);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set($"user_{user.Id}", user, cacheOptions);

            }
            catch(DbUpdateException ex) when(ex.InnerException is PostgresException { SqlState: "23505" })
            {
                return new(false,"Email is already in use.");
            }
            catch (NpgsqlException ex)
            {
                // 2. Handle DATABASE INFRASTRUCTURE error (Abrupt close, connection refused)
                // Log this error internally!
                return new(false,"The database is currently unavailable. Please try again later.", ex.Message);
            }
            catch (Exception)
            {
                // 3. Handle UNEXPECTED system error (Bug, out of memory, etc.)
                // This is usually caught by Global Exception Handling, but can be caught here for specific cleanup
                throw;
            }

            return new(true);
        }
    }
}
