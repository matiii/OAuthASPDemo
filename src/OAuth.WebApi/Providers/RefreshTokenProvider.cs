using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Infrastructure;
using OAuth.WebApi.Entities;
using OAuth.WebApi.Extensions;

namespace OAuth.WebApi.Providers
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];

            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");

            using (var db = new OAuthEntities())
            {
                var refreshTokenLifeTime = context.OwinContext.Get<TimeSpan>(ProviderKeys.TokenTimeSpan);

                var token = new RefreshToken
                {
                    Id = refreshTokenId.ToHash(),
                    ClientId = int.Parse(clientid),
                    Subject = context.Ticket.Identity.Name,
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(refreshTokenLifeTime.TotalMinutes)
                };

                context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
                context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

                token.ProtectedTicket = context.SerializeTicket();

                db.RefreshToken.Add(token);
                await db.SaveChangesAsync();
                context.SetToken(refreshTokenId);
            }
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {

            var allowedOrigin = context.OwinContext.Get<string[]>(ProviderKeys.AllowedOrigins);
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", allowedOrigin );

            string hashedTokenId = context.Token.ToHash();

            using (var db = new OAuthEntities())
            {
                var refreshToken = await db.RefreshToken.FirstOrDefaultAsync(x => x.Id == hashedTokenId);

                if (refreshToken != null)
                {
                    //Get protectedTicket from refreshToken class
                    context.DeserializeTicket(refreshToken.ProtectedTicket);
                    db.RefreshToken.Remove(refreshToken);
                }
            }
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}