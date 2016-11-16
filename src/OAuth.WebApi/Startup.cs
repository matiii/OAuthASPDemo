using Microsoft.Owin;
using OAuth.WebApi.Entities;
using OAuth.WebApi.Extensions;
using Owin;

[assembly: OwinStartup(typeof(OAuth.WebApi.Startup))]
namespace OAuth.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            App_Start.OAuth.Configure(app);
            App_Start.WebApi.Configure(app);

            //TestData();

        }

        private void TestData()
        {
            using (var db = new OAuthEntities())
            {
                db.Client.Add(new Client
                {
                    Name = "test",
                    Secret = "test".ToHash(),
                    TokenLifeTime = 15
                });

                db.User.Add(new User
                {
                    UserName = "user",
                    Password = "pass".ToHash()
                });

                db.SaveChanges();
            }
        }
    }
}