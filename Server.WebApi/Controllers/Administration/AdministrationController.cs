//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Web.Http;
//using System.Web.Http.Cors;
//using Siemplify.Common;
//using Siemplify.Common.IoC;
//using Siemplify.DataModel.User;
//using Siemplify.Server.DataAccessors.Admin;
//using Siemplify.Server.DataAccessors.Users;

//namespace Siemplify.Server.WebApi.Controllers.Administration
//{
//    [Authorize]
//    [EnableCors(origins: "*", headers: "*", methods: "*")]
//    [RoutePrefix("api/Admin")]
//    public class AdministrationController : ApiController, IAdministrationService
//    {
//        private readonly IAdminAccessor _adminAccessor;
//        private readonly Logger _logger;
        
//        public AdministrationController()
//        {
//            _adminAccessor = new AdminAccessor();
//            _logger = Logger.Instance;
//        }
        
//        // POST: api/Simulator/ClearAll
//        [Route("ClearAll")]
//        [HttpGet]
//        [AllowAnonymous]
//        public IHttpActionResult ClearAll()
//        {
//            _adminAccessor.ClearDb();
//            return Ok();
//        }

//        // POST: api/Simulator/ClearAll
//        [Route("ClearDbWithoutUsers")]
//        [HttpGet]
//        [AllowAnonymous]
//        public IHttpActionResult ClearDbWithoutUsers()
//        {
//            _adminAccessor.ClearDbWithoutUsers();
//            return Ok();
//        }

//        // GET: api/Admin/GetAllUsers
//        [Route("GetAllUsers")]
//        [HttpGet]
//        [AllowAnonymous]
//        public List<UserProfile> GetAllUsers()
//        {
//            var usersAccessor = new UsersAccessor();
//            return usersAccessor.UsersProfiles.ToList();
//        }

//        // GET: api/Admin/GetAllUserLogInDetails
//        [Route("GetAllUserLogInDetails")]
//        [HttpGet]
//        [AllowAnonymous]
//        public List<UserLogInDetails> GetAllUserLogInDetails()
//        {
//            var adminAccessor = new AdminAccessor();
//            return adminAccessor.UserLogInDetails.ToList();
//        }

//        [Route("AddOrUpdateLogInDetails")]
//        [HttpPost]
//        [AllowAnonymous]
//        public IHttpActionResult AddOrUpdateLogInDetails(UserLogInDetails userLogInDetails)
//        {
//            var adminAccessor = new AdminAccessor();
//            adminAccessor.AddOrUpdateLogInDetails(userLogInDetails);
//            return Ok();
//        }


    
//    }
//}