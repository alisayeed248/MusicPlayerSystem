using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;

using MusicPlayerWebApp.Models;

namespace MusicPlayerWebApp.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AmazonCognitoIdentityProviderClient _cognitoClient;
        private readonly string _clientId;
        private readonly string _userPoolId;
        public AuthenticationService(IConfiguration configuration) {
            _clientId = configuration["Cognito:ClientId"];
            _userPoolId = configuration["Cognito:UserPoolId"];
            var awsCredentials = new StoredProfileAWSCredentials();
            _cognitoClient = new AmazonCognitoIdentityProviderClient(awsCredentials, RegionEndpoint.USEast1);
        }

        public async Task<UserSession> SignInAsync(string username, string password)
        {
            var request = new AdminInitiateAuthRequest
            {
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH,
                ClientId = _clientId,
                UserPoolId = _userPoolId,
                AuthParameters = new Dictionary<string, string>
            {
                {"USERNAME", username},
                {"PASSWORD", password}
            }
            };

            try
            {
                var response = await _cognitoClient.AdminInitiateAuthAsync(request);
                return new UserSession
                {
                    IdToken = response.AuthenticationResult.IdToken,
                    AccessToken = response.AuthenticationResult.AccessToken,
                    RefreshToken = response.AuthenticationResult.RefreshToken
                };
            }
            catch (NotAuthorizedException ex)
            {
                throw new UnauthorizedAccessException("Invalid username or password.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eror during sign-in: {ex.Message}");
                throw new ApplicationException("An error occurred during sign-in.", ex);
            }
        }
    }
}
