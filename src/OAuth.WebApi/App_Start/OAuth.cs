using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using OAuth.WebApi.Providers;
using Owin;

// ReSharper disable once CheckNamespace
namespace OAuth.WebApi.App_Start
{
    internal static class OAuth
    {
        public static void Configure(IAppBuilder app)
        {
            var bearerOptions = new OAuthBearerAuthenticationOptions();
            var serverOptions = new OAuthAuthorizationServerOptions
            {
#if DEBUG
                AllowInsecureHttp = true,
#endif
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new AuthorizationServerProvider(),
                RefreshTokenProvider = new RefreshTokenProvider()
            };

            app.UseOAuthAuthorizationServer(serverOptions);
            app.UseOAuthBearerAuthentication(bearerOptions);
        }
    }
}