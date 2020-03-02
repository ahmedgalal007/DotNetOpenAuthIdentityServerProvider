using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Galal.IDP.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace IdentityServerProvider.Controllers.UserRegistration
{
    public class UserRegisterationController : Controller
    {
        private readonly IIDPUserRepository _IDPUserRepository;
        private readonly IIdentityServerInteractionService _interaction;

        public UserRegisterationController(IIDPUserRepository iDPUserRepository,
            IIdentityServerInteractionService interaction)
        {
            _IDPUserRepository = iDPUserRepository;
            _interaction = interaction;
        }
        [HttpGet]
        public IActionResult RegisterUser(RegisterationInputModel registerationInputModel)
        {
            //var vm = new RegisterUserViewModel() { ReturnUrl = returnUrl };
            //return View(vm);

            var vm = new RegisterUserViewModel() {
                ReturnUrl = registerationInputModel.ReturnUrl,
                Provider = registerationInputModel.Provider,
                ProviderUserId = registerationInputModel.ProviderUserId
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // create user + claims
                var userToCreate = new Galal.IDP.Entities.User();
                userToCreate.SubjectId = Guid.NewGuid().ToString();
                userToCreate.Password = model.Password;
                userToCreate.Username = model.Username;
                userToCreate.IsActive = true;
                userToCreate.Claims.Add(new Galal.IDP.Entities.UserClaim("country", model.Country));
                userToCreate.Claims.Add(new Galal.IDP.Entities.UserClaim("address", model.Address));
                userToCreate.Claims.Add(new Galal.IDP.Entities.UserClaim("given_name", model.Firstname));
                userToCreate.Claims.Add(new Galal.IDP.Entities.UserClaim("family_name", model.Lastname));
                userToCreate.Claims.Add(new Galal.IDP.Entities.UserClaim("email", model.Email));
                userToCreate.Claims.Add(new Galal.IDP.Entities.UserClaim("subscriptionlevel", "FreeUser"));

                // if we're provisioning a user via external login, we must add the provider &
                // user id at the provider to this user's logins
                if (model.IsProvisioningFromExternal)
                {
                    userToCreate.Logins.Add(new Galal.IDP.Entities.UserLogin()
                    {
                        LoginProvider = model.Provider,
                        ProviderKey = model.ProviderUserId
                    });
                }

                // add it through the repository
                _IDPUserRepository.AddUser(userToCreate);

                if (!_IDPUserRepository.Save())
                {
                    throw new Exception($"Creating a user failed.");
                }

                if (!model.IsProvisioningFromExternal)
                {
                    // log the user in
                    await HttpContext.SignInAsync(userToCreate.SubjectId, userToCreate.Username);
                }


                // continue with the flow     
                if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return Redirect("~/");
            }

            // ModelState invalid, return the view with the passed-in model
            // so changes can be made
            return View(model);
        }
    }
}