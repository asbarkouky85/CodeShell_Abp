using System.Collections.Generic;

namespace Codeshell.Abp.Http
{
    public class ReRouteItem
    {
        public string DownstreamPathTemplate { get; set; }
        public string DownstreamScheme { get; set; }
        public IEnumerable<DownstreamHostAndPortItem> DownstreamHostAndPorts { get; set; }
        public string Key { get; set; }
    }
}
