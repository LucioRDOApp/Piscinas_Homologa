using laudosPiscinasProject.Api.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace laudosPiscinasProject.Api.Controllers
{
    public class MedicaoController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage GerarRelatorio(RelatorioMedicaoViewModel filter)
        {
            try
            {
                byte[] retorno = RelatorioModel.Medicao(filter);
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }           
        }
    }
}
