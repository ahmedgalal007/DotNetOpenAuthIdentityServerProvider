using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerProvider.Controllers.UserRegistration
{
    public class RegisterationInputModel
    {
        public string ReturnUrl { get; set; }
        public string Provider { get; set; }
        public string ProviderUserId { get; set; }
    }
}
