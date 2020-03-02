using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityModel.Client;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Client.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Client.MVC.Controllers
{
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authorize( Roles = "PayingUser")]
        public async Task<IActionResult> OrderFrame()
        {
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken); 
            // Add IdentityModel NuGet Package
            var discoveryEndpoint = new HttpClient(); //DiscoveryEndpoint("https://localhost:44316/", "");
            var metaDataResponse = await discoveryEndpoint.GetDiscoveryDocumentAsync("https://localhost:44316/");

            var request = new UserInfoRequest() { 
                Address = metaDataResponse.UserInfoEndpoint,
                Method = HttpMethod.Get,
                //AuthorizationHeaderStyle = BasicAuthenticationHeaderStyle.Rfc2617,
                Token = accessToken
            };

            var response = await discoveryEndpoint.GetUserInfoAsync(request);
            if (response.IsError)
            {
                throw new Exception("Problem accessing the UseInfo endpoint.", response.Exception);
            }
            var address = response.Claims.SingleOrDefault(x => x.Type == "address")?.Value;
            return View( new OrderFrameViewModel(address));
        }
    }
}