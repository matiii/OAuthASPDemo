using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using OAuth.WebApi.Entities;
using OAuth.WebApi.Extensions;

namespace OAuth.WebApi.Providers
{
    internal class AuthorizationServerProvider: OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId, clientSecret;
            int id = 0;
            Client client = null;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
                context.TryGetFormCredentials(out clientId, out clientSecret);

            if (String.IsNullOrEmpty(clientId) || String.IsNullOrEmpty(clientSecret) || !int.TryParse(clientId, out id))
            { context.SetError("ClientId and ClientSecret should be provided."); return;}

            using (var db = new OAuthEntities())
            {
                client = await db.Client.AsNoTracking().Include(x => x.AllowedOrigins).FirstOrDefaultAsync(x => x.Id == id);

                if (client?.Secret != clientSecret.ToHash())
                {
                    context.SetError("ClientId wasn't recognized or hash is not valid.");
                    return;
                }
            }

            context.OwinContext.Set(ProviderKeys.AllowedOrigins, client.AllowedOrigins.Select(x => x.Origin).ToArray());
            context.OwinContext.Set(ProviderKeys.TokenTimeSpan, client.TokenTimeSpan);

            context.Validated(clientId);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            string[] allowedOrigins = context.OwinContext.Get<string[]>(ProviderKeys.AllowedOrigins);

            context.OwinContext.Response.Headers.Add(ProviderKeys.OriginHeader, allowedOrigins);

            using (var db = new OAuthEntities())
            {
                var user = await db.User.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == context.UserName);

                if (user == null || user.Password != context.Password.ToHash())
                {
                    context.SetError("Invalid login or password");
                    return;
                }
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "user"));

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                        "as:client_id", context.ClientId ?? string.Empty
                    },
                    {
                        "userName", context.UserName
                    }
                });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            var currentClient = context.ClientId;

            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);

            var newClaim = newIdentity.Claims.FirstOrDefault(c => c.Type == "newClaim");
            if (newClaim != null)
            {
                newIdentity.RemoveClaim(newClaim);
            }
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);

            return Task.FromResult<object>(null);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}