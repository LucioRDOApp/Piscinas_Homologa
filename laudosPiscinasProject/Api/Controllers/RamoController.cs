using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using static laudosPiscinasProject.Api.Models.RamoModel;

namespace laudosPiscinasProject.Api.Controllers
{
    public class RamoController : ApiController
    {
        [HttpGet]
        public List<RamoViewModel> Lista()
        {
            return RamoModel.Lista();
        }
        [HttpPost]
        public bool Atualizar([FromBody] dynamic param)
        {
            return RamoModel.Atualizar(param);
        }
        [HttpPost]
        public bool Remover([FromBody] dynamic param)
        {
            return RamoModel.Remove(param);
        }
    }
}