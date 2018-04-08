using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;


namespace Siemplify.Server.WebApi.Authentication
{
    public class SiemplifyApiKeyAuthorizeAttribute : AuthorizeAttribute
    {
        private static HashSet<string> _keys; 
        static SiemplifyApiKeyAuthorizeAttribute()
        {
            _keys = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
             _keys.Add("89435f29-49ff-4426-9938-d808c3dd1313");

        }



        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            IEnumerable<string> values = new List<string>();
            var found = actionContext.Request.Headers.TryGetValues("ApiKey", out values);
            if (found == false)
                return false;
            if (!values.Any())
                return false;

            var key = values.FirstOrDefault();

            if (_keys.Contains(key))
            {
                return true;
            }

            return false;
        }
    }
}