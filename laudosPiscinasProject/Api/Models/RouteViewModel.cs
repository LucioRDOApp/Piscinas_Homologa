using System.Collections.Generic;

namespace laudosPiscinasProject.Api.Models
{
    public class RouteViewModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<string> Permissions { get; set; }
    }
}
