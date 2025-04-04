using laudosPiscinasProject.Api.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace laudosPiscinasProject.Api.Controllers
{
    public class StatusTarefaController : ApiController
    {
        public List<StatusTarefaViewModel> Lista()
        {
            return StatusTarefaModel.Lista();
        }

        public List<StatusTarefaViewModel> ListaStatusTarefaPermitidos(dynamic param)
        {
            return StatusTarefaModel.ListaStatusTarefaPermitidos(param);
        }
    }
}