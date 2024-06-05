using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using MusicPlayerWebApp.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerWebApp.Services
{
    public class CognitoAuthenticationService : ICognitoAuthenticationService
    {
        private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _userPoolId;
        private readonly string _password;

        public CognitoAuthenticationService(IConfiguration configuration) {
            _clientId = configuration["Cognito:ClientId"];
            _userPoolId = configuration["Cognito:UserPoolId"];
            _clientSecret = configuration["Cognito:ClientSecret"];

            var awsCredentials = new StoredProfileAWSCredentials();
            _cognitoClient = new AmazonCognitoIdentityProviderClient(awsCredentials, RegionEndpoint.USEast1);
        }

        public async Task<UserSession> SignInAsync(string username, string password)
        {
            var key = Encoding.UTF8.GetBytes(_clientSecret);
            var message = Encoding.UTF8.GetBytes(username + _clientId);
            
                using (var hmac = new HMACSHA256(key))
            {
                var hashBytes = hmac.ComputeHash(message);
                var secretHash = Convert.ToBase64String(hashBytes);

                var request = new AdminInitiateAuthRequest
                {
                    AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH,
                    ClientId = _clientId,
                    UserPoolId = _userPoolId,
                    AuthParameters = new Dictionary<string, string>
                    {
                        {"USERNAME", username},
                        {"PASSWORD", password},
                        {"SECRET_HASH", secretHash }
                    }
                };
                Console.WriteLine($"Request details: AuthFlow={request.AuthFlow}, ClientId={request.ClientId}, Params={string.Join(", ", request.AuthParameters.Select(kv => kv.Key + "=" + kv.Value))}");
                try
                {
                    if (_cognitoClient == null)
                    {
                        throw new InvalidOperationException("Cognito client is not initialized.");
                    }

                    Console.WriteLine($"Requesting authentication for user: {username}");
                    var response = await _cognitoClient.AdminInitiateAuthAsync(request);
                    Console.WriteLine("Response received:");
                    Console.WriteLine($"ChallengeName: {response.ChallengeName}");
                    Console.WriteLine($"Session: {response.Session}");
                    if (response.AuthenticationResult == null)
                    {
                        Console.WriteLine("AuthenticationResult is null.");
                        throw new ApplicationException("AuthenticationResult is null.");
                    }
                    Console.WriteLine($"IdToken: {response.AuthenticationResult.IdToken}");
                    Console.WriteLine($"AccessToken: {response.AuthenticationResult.AccessToken}");
                    Console.WriteLine($"RefreshToken: {response.AuthenticationResult.RefreshToken}");
                    Console.WriteLine($"Test: {response.AuthenticationResult.RefreshToken}");
                    return new UserSession
                    {
                        IdToken = response.AuthenticationResult.IdToken,
                        AccessToken = response.AuthenticationResult.AccessToken,
                        RefreshToken = response.AuthenticationResult.RefreshToken
                    };
                }
                catch (AmazonCognitoIdentityProviderException e) 
                {
                        Console.WriteLine($"Cognito provider exception: {e.Message}");
                        throw;
                 }
                //catch (NotAuthorizedException ex)
                //{
                //    throw new UnauthorizedAccessException("Invalid username or password.", ex);
                //}
                catch (Exception ex)
                {
                    //Console.WriteLine($"Error during sign-in: {ex.Message}");
                    //throw new ApplicationException("An error occurred during sign-in.", ex);
                    Console.WriteLine($"Exception: {ex.ToString()}");
                    throw;
                }
            }
        }
    }
}
