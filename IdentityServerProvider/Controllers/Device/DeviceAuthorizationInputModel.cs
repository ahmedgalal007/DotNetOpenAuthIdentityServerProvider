// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServerProvider.Controllers.Consent;

namespace IdentityServerProvider.Controllers.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string UserCode { get; set; }
    }
}