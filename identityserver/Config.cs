﻿using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [];

    public static IEnumerable<Client> Clients =>
        [
            new Client
            {
                ClientId = "web-par",
                ClientSecrets = { new Secret("ddedF4f289k$3eDa23ed0iTk4Raq&tttk23d08nhzd".Sha256()) },

                RequirePushedAuthorization = true,

                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,

                RedirectUris = { "https://localhost:5007/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:5007/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:5007/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile" }
            }
        ];
}
