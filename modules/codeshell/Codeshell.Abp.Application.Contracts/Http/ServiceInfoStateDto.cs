using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codeshell.Abp.Http
{
    public class ServiceInfoStateDto
    {
        public string ServiceName { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
        public string Elapsed { get; set; }

        protected ServiceInfoStateDto() { }

        public ServiceInfoStateDto(ReRouteItem route)
        {
            ServiceName = route.Key;
            var hostAndPort = route.DownstreamHostAndPorts.FirstOrDefault();
            if (hostAndPort != null)
            {
                Url = $"{route.DownstreamScheme}://{hostAndPort.Host}:{hostAndPort.Port}";
            }
        }
    }
}
