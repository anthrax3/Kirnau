﻿//===============================================================================
// Microsoft patterns & practices
// Windows Azure Architecture Guide
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// This code released under the terms of the 
// Microsoft patterns & practices license (http://wag.codeplex.com/license)
//===============================================================================


namespace Samples.Web.ClaimsUtillities
{
    using System;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading;
    using Microsoft.IdentityModel.Claims;

    public static class ClaimHelper
    {
        public static Claim GetCurrentUserClaim(string claimType)
        {
            return GetClaimsFromPrincipal(Thread.CurrentPrincipal, claimType);
        }

        public static Claim GetClaimsFromPrincipal(IPrincipal principal, string claimType)
        {
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            IClaimsPrincipal claimsPrincipal = principal as IClaimsPrincipal;

            if (claimsPrincipal == null)
            {
                throw new ArgumentException("Cannot convert principal to IClaimsPrincipal.", "principal");
            }

            return GetClaimFromIdentity(claimsPrincipal.Identities[0], claimType);
        }

        public static Claim GetClaimFromIdentity(IIdentity identity, string claimType)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            IClaimsIdentity claimsIdentity = identity as IClaimsIdentity;

            if (claimsIdentity == null)
            {
                throw new ArgumentException("Cannot convert identity to IClaimsIdentity", "identity");
            }

            return claimsIdentity.Claims.Single(c => c.ClaimType == claimType);
        }
    }
}
