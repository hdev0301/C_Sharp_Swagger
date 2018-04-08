using System;

namespace Siemplify.Server.WebApi.Infrastructure
{
    public class SelfHostingParameters
    {
        public SelfHostingParameters(string baseAddress)
        {
            BaseAddress = baseAddress;
            UseDataContractSerializer = true;
        }

        public string BaseAddress { get; set; }
        public bool UseDataContractSerializer { get; set; }
        public Type[] ControllerTypes { get; set; }
    }
}