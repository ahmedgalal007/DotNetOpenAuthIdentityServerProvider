using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Client.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Client.MVC.Services.Contracts;

namespace Client.MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private HttpClient _httpClient_Api, _httpClient_Api_policy;
        private readonly IApplicationHttpClient _aplicationHttpClient;
        public HomeController(IApplicationHttpClient applicationHttpClient)
        {
            _aplicationHttpClient = applicationHttpClient;

        }
        public IActionResult Index()
        {
            return View();
        }


        //public async Task<IActionResult> PrivacyAsync()
        //{
        //    await WriteOutIdentityIntoConfiguration();

        //    return View(Content("Privacy Content"));
        //}

        [Authorize(Roles = "PayingUser")]
        public async Task<IActionResult> WeatherApi()
        {
            //if(_httpClient_Api == null)
            _httpClient_Api = await _aplicationHttpClient.GetClient("https://localhost:44315/", "ClientMVC", "Sico007_");

            var response = await _httpClient_Api.GetAsync("WeatherForecast").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var json = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), typeof(List<WeatherForecast>));
                return View(json);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Authorization");
            }

            throw new Exception($"A problem happened while calling the Api:{response.ReasonPhrase}");
        }

        // Install Microsoft.AspNetCore.Authorization Package 
        [Authorize(Policy = "PolicyApi")]
        public async Task<IActionResult> PolicyBasedApi()
        {

            // call the API
            //if(_httpClient_Api_policy == null)
            _httpClient_Api_policy = await _aplicationHttpClient.GetClient("https://localhost:44338/", "ClientMVC", "Sico007_");

            var response = await _httpClient_Api_policy.GetAsync("WeatherForecast").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var json = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync(), typeof(List<WeatherForecast>));
                return View(json);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                  response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Authorization");
            }

            throw new Exception($"A problem happened while calling the Api:{response.ReasonPhrase}");
        }

        public IActionResult Privacy()
        {
            return View(User.Claims);
        }

        public async Task logout()
        {
            var revokeClient = new HttpClient();
            revokeClient.BaseAddress = new Uri("https://localhost:44316/");
            var metaDataResponse = await revokeClient.GetDiscoveryDocumentAsync("https://localhost:44316/");
            // revoke the access token
            var _accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (!string.IsNullOrWhiteSpace(_accessToken))
            {
                var revoke = await revokeClient.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Address = metaDataResponse.RevocationEndpoint,
                    ClientId = "ClientMVC",
                    ClientSecret = "Sico007_",
                    Token = _accessToken//,
                    //TokenTypeHint = OpenIdConnectParameterNames.AccessToken
                });
                if (revoke.IsError)
                {
                    throw new Exception("Problem encountered while revoking the access token.",
                    revoke.Exception);
                }
            }

            // revoke the refresh token
            var _refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            if (!string.IsNullOrWhiteSpace(_refreshToken))
            {
                var revoke = await revokeClient.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Address = metaDataResponse.RevocationEndpoint,
                    ClientId = "ClientMVC",
                    ClientSecret = "Sico007_",
                    Token = _refreshToken //,
                    // TokenTypeHint = OpenIdConnectParameterNames.RefreshToken
                });
                if (revoke.IsError)
                {
                    throw new Exception("Problem encountered while revoking the refresh token.",
                    revoke.Exception);
                }
            }

            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task WriteOutIdentityIntoConfiguration()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            Debug.WriteLine($"Identity Token: {identityToken}");
            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim Type : {claim.Type} - Claim Value : {claim.Value}");
            }
        }
    }


    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
