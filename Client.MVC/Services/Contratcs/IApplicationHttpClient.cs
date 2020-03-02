using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client.MVC.Services.Contracts
{
    public interface IApplicationHttpClient
    {
        Task<HttpClient> GetClient(string BaseAddress, string clientId, string clientSecret);
    }
}
