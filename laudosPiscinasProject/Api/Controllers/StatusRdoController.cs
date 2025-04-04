using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using static laudosPiscinasProject.Api.Models.StatusRdoModel;

namespace laudosPiscinasProject.Api.Controllers
{
    public class StatusRdoController : ApiController
    {
        public List<StatusRdoViewModel> Lista()
        {
            return StatusRdoModel.Lista();
        }
    }
}