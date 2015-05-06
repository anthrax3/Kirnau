//===============================================================================
// Microsoft patterns & practices
// Windows Azure Architecture Guide
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// This code released under the terms of the 
// Microsoft patterns & practices license (http://wag.codeplex.com/license)
//===============================================================================


namespace Samples.Web.ClaimsUtillities
{
    public static class AllOrganizations
    {
        public static class ClaimTypes
        {
            public static readonly string Group = "http://schemas.xmlsoap.org/claims/group";
        }

        public static class Users
        {
            public const string Administrator = "Admin";
        }
    }
}