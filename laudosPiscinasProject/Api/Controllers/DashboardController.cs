using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace laudosPiscinasProject.Api.Controllers
{
    public class DashboardController : ApiController
    {
        [HttpPost]
        public object CarregarDashboards(dynamic param)
        {
            return DashboardModel.CarregarDashboards(param);
        }


    }
}