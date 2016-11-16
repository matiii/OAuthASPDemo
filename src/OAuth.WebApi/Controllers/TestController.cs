using System.Web.Http;

namespace OAuth.WebApi.Controllers
{
    [RoutePrefix("api/test")]
    public class TestController: ApiController
    {
        [Authorize]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok();
        }

    }
}