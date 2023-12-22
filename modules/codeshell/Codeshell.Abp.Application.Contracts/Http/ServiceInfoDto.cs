using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Http
{
    public class ServiceInfoDto
    {
        public string ServiceName { get; set; }
        public string Version { get; set; }
        public string RemoteServiceUrl { get; set; }
        public string RemoteServiceState { get; set; }
    }
}
