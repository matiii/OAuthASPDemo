using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json.Serialization;
using Owin;

// ReSharper disable once CheckNamespace
namespace OAuth.WebApi.App_Start
{
    internal static class WebApi
    {
        public static void Configure(IAppBuilder app)
        {
            //config web api
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            app.UseWebApi(config);
        }
    }
}