using System.Collections.Generic;

namespace laudosPiscinasProject.Api.Models
{
    public class RouteModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        //public string[] Permissions { get; set; }
        public List<string> Permissions { get; set; }
    }
}
