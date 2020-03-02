using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel;
using Client.MVC.Services.Contracts;
using Microsoft.AspNetCore.DataProtection;

namespace Client.MVC.Services
{
    public class ApplicationHttpClient : IApplicationHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private HttpClient _httpClient = new HttpClient();
        private string _clientId, _clientSecret;
        public ApplicationHttpClient(IHttpContextAccessor httpContextAccessor )
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<HttpClient> GetClient(string BaseAddress, string clientId, string clientSecret)
        {
            string accessToken = string.Empty;
            var currentContext = _httpContextAccessor.HttpContext;
            _httpClient.BaseAddress = new Uri(BaseAddress);
            _clientId = clientId;
            _clientSecret = clientSecret;

            // should we renew access & refresh tokens?
            // get expires_at value
            var expires_at = await currentContext.GetTokenAsync("expires_at");

            // compare - make sure to use the exact date formats for comparison 
            // (UTC, in this case)
            if (string.IsNullOrWhiteSpace(expires_at)
                || ((DateTime.Parse(expires_at).AddSeconds(-60)).ToUniversalTime()
                < DateTime.UtcNow))
            {
                accessToken = await RenewTokens();
            }
            else
            {
                // get access token
                accessToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                // var accessTokenObj = await currentContext.AuthenticateAsync("oidc");
                // var result =  new { token = accessTokenObj.Properties.Items[".Token.id_token"] };
            }

            if (!string.IsNullOrEmpty(accessToken))
            {
                _httpClient.SetBearerToken(accessToken);
            }

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            return _httpClient;
        }

        private async Task<string> RenewTokens()
        {
            // get the current HttpContext to access the tokens
            var currentContext = _httpContextAccessor.HttpContext;
            
            var expat = _httpContextAccessor.HttpContext.GetTokenAsync("expires_at").Result;

            // var dataExp = DateTime.Parse(expat, null, DateTimeStyles.RoundtripKind);


            // get the saved refresh token
            var currentRefreshToken = await currentContext.GetTokenAsync("oidc",OpenIdConnectParameterNames.RefreshToken);

            //if ((dataExp - DateTime.Now).TotalMinutes < 10)
            //{

            // get the metadata
            var metaDataResponse = await _httpClient.GetDiscoveryDocumentAsync("https://localhost:44316/");

            // create Refresh Token Request
            RefreshTokenRequest refreshTokenRequest = new RefreshTokenRequest
            {
                Address = metaDataResponse.TokenEndpoint,

                ClientId = _clientId,
                ClientSecret = _clientSecret,
                RefreshToken = currentRefreshToken
            };

            // refresh the tokens
            var tokenResult = await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);

            if (!tokenResult.IsError)
            {
                // Save the tokens. 

                // get auth info
                var authenticateInfo = await currentContext.AuthenticateAsync("Cookies");

                // create a new value for expires_at, and save it
                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
                authenticateInfo.Properties.UpdateTokenValue("expires_at",
                    expiresAt.ToString("o", CultureInfo.InvariantCulture));

                authenticateInfo.Properties.UpdateTokenValue(
                    OpenIdConnectParameterNames.AccessToken,
                    tokenResult.AccessToken);
                authenticateInfo.Properties.UpdateTokenValue(
                    OpenIdConnectParameterNames.RefreshToken,
                    tokenResult.RefreshToken);

                // we're signing in again with the new values.  
                await currentContext.SignInAsync("Cookies", authenticateInfo.Principal, authenticateInfo.Properties);

                // return the new access token 
                return tokenResult.AccessToken;
            }
            else
            {
                currentContext.Response.Redirect("Authorization/AccessDenied");
                throw new Exception("Problem encountered while refreshing tokens.",
                    tokenResult.Exception);
            }
           // }
        }
    }
}