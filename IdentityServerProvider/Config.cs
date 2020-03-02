// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServerProvider
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource( "roles", "Your role(s)", new List<string>(){"role"}),
                new IdentityResource( "country", "The country you're living in", new List<string>(){"country"}),
                new IdentityResource( "subscriptionlevel", "Your subscription level", new List<string>(){"subscriptionlevel"}),
            };


        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("api1", "My API"),
                new ApiResource("imagegalaryapi", "Role_Based Image Galary Api", 
                    new List<string>{"role"}
                ){
                    ApiSecrets = { new Secret("Sico007_".Sha256()) }
                },
                new ApiResource("policybasedapi","Policy_Based Weather Api",
                    new List<string>{ "role" }
                )
                {
                    ApiSecrets = { new Secret("Sico007_".Sha256()) }
                }
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                //My Client.Mvc Project Client
                new Client{
                    ClientName = "Client MVC",
                    ClientId = "ClientMVC",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RequireConsent = false,
                    //RequirePkce = true,

                    AccessTokenType = AccessTokenType.Reference,

                    // ////////////////////////////////   Tokens LifeTime     //////////////////////////////////////////
                    // IdentityTokenLifetime =300,
                    // AuthorizationCodeLifetime = 300,
                    AccessTokenLifetime = 120, // Defaults 3600

                    //AbsoluteRefreshTokenLifeTime but you can make it Sliding and give sliding token a time
                    // RefreshTokenExpiration = TokenExpiration.Sliding,
                    // SlidingRefreshTokenLifetime = ...

                    UpdateAccessTokenClaimsOnRefresh = true, // if claim is Updated the useraccesstoken update even if it's life time not expiered
                    AllowOfflineAccess = true, // To use the refresh token feature and connect the user even if disconnected from IDP


                    // ////////////////////////////////   Sending Claims     ////////////////////////////////
                    //AlwaysSendClientClaims = true,
                    //AlwaysIncludeUserClaimsInIdToken = true,
                    // where to redirect to after login
                    RedirectUris = new List<string>{ 
                        "https://localhost:44345/signin-oidc" 
                    },
                    // where to redirect to after logout
                    PostLogoutRedirectUris = {
                        "https://localhost:44345/signout-callback-oidc"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "imagegalaryapi",
                        "policybasedapi",
                        "country",
                        "subscriptionlevel"
                    },
                    ClientSecrets = {
                        new Secret("Sico007_".Sha256())
                    },
                    

                },
                // machine to machine client
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                },
                // interactive ASP.NET Core MVC client
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,
                
                    // where to redirect to after login
                    RedirectUris = { "http://localhost:5002/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                }
            };

        //public static List<TestUser> Users()
        //{
        //    return new List<TestUser> { 
            
        //        new TestUser{ SubjectId="ddd61595-85bf-4562-8185-91e7accf68a3", Username="alice", Password="Pass123$", IsActive=true, ProviderName="local", ProviderSubjectId="",
        //            Claims = new Claim[]{
        //                new Claim(JwtClaimTypes.Name, "Alice Smith"),
        //                new Claim(JwtClaimTypes.GivenName, "Alice"),
        //                new Claim(JwtClaimTypes.FamilyName, "Smith"),
        //                new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
        //                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
        //                new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
        //                new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
        //                new Claim(JwtClaimTypes.Role,"FreeUser"), 
        //            }
        //        },
        //        new TestUser{ SubjectId="b5ef4a33-0061-4121-8726-c91deb2debb1", Username="bob", Password="Pass123$", IsActive=true, ProviderName="local", ProviderSubjectId="",
        //            Claims = new Claim[]
        //            {
        //                new Claim(JwtClaimTypes.Name, "Bob Smith"),
        //                new Claim(JwtClaimTypes.GivenName, "Bob"),
        //                new Claim(JwtClaimTypes.FamilyName, "Smith"),
        //                new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
        //                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
        //                new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
        //                new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
        //                new Claim("location", "somewhere"),
        //                new Claim(JwtClaimTypes.Role,"PayingUser")
        //            }
        //        }
        //    };
        //}
    }
}