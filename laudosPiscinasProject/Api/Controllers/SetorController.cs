using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;
using static laudosPiscinasProject.Api.Models.SetorModel;

namespace laudosPiscinasProject.Api.Controllers
{
    public class SetorController : ApiController
    {
        [HttpGet]
        public List<SetorViewModel> Lista()
        {
            return SetorModel.Lista();
        }

        [HttpPost]
        public bool Atualizar([FromBody] dynamic param)
        {
            return SetorModel.Atualizar(param); 
        }

        [HttpPost]
        public bool Remover([FromBody] dynamic param)
        {
            return SetorModel.Remove(param);
        }
    }
}