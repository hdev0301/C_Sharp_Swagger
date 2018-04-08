using System;
using System.Collections.Generic;
using System.Web.Http.Dispatcher;

namespace Siemplify.Server.WebApi.Infrastructure
{
    public class SiemplifyControllerResolver : IHttpControllerTypeResolver
    {
        private readonly Type[] _types;

        public SiemplifyControllerResolver(Type[] types)
        {
            _types = types;
        }

        public ICollection<Type> GetControllerTypes(IAssembliesResolver assembliesResolver)
        {
            return _types;
        }
    }
}