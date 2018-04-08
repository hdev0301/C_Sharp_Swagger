using System.Collections.Generic;
using System.Web.Http;
using Model;

namespace Siemplify.Server.WebApi.Controllers
{
    public class UserController : ApiController
    {
        private readonly List<User> _users;
        public UserController()
        {
            _users = new List<User>();
            _users.Add(new User { Name = "User-1" });
            _users.Add(new User { Name = "User-2" });
            _users.Add(new User { Name = "User-3" });
        }
        [HttpPost]
        [Route("api/v1/adduser")]
        public IHttpActionResult AddBook(User user)
        {
            _users.Add(user);
            return Ok(string.Format("User {0} was added.", user.Name));
        }

        [HttpGet]
        [Route("api/v1/users")]
        public List<User> GetUsers()
        {
            return _users;
        }
    }
}
