//using System.Collections.Generic;
//using System.ServiceModel;
//using System.ServiceModel.Web;
//using System.Web.Http;
//using Siemplify.DataModel.User;
//using Siemplify.Server.DataAccessors.Admin;

//namespace Siemplify.Server.WebApi.Controllers.Administration
//{
//    [ServiceContract]
//    public interface IAdministrationService
//    {

//        [OperationContract]
//        IHttpActionResult ClearAll();

//        [OperationContract]
//        IHttpActionResult ClearDbWithoutUsers();

//        [OperationContract]
//        [WebGet]
//        List<UserProfile> GetAllUsers();

//        [OperationContract]
//        List<UserLogInDetails> GetAllUserLogInDetails();

//        [OperationContract]
//        IHttpActionResult AddOrUpdateLogInDetails(UserLogInDetails userLogInDetails);


//    }
//}
