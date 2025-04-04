using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using static laudosPiscinasProject.Api.Models.UnidadeMedidaModel;

namespace laudosPiscinasProject.Api.Controllers
{
    public class UnidadeMedidaController : ApiController
    {
        public List<UnidadeMedidaViewModel> Lista()
        {
            return UnidadeMedidaModel.Lista();
        }
    }
}