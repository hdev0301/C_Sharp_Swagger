using System.Web.Http;
using System.Web.Http.Cors;

namespace Siemplify.Server.WebApi.Controllers
{
    [RoutePrefix("api/Authentication")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class AuthenticationController : ApiController
    {

    }
}
