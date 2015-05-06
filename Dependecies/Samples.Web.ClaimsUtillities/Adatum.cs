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
    public static class Adatum
    {
        public static string OrganizationName
        {
            get
            {
                return "Adatum";
            }
        }

        public static class Groups
        {
            public static readonly string MarketingManagers = "Marketing Managers";
            public static readonly string DomainUsers = "Domain Users";
        }
    }
}