using laudosPiscinasProject.Api.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace laudosPiscinasProject.Api.Controllers
{
    public class RdoController : ApiController
    {
        [HttpPost]
        public PagedList CarregarLista(dynamic param)
        {
            return PagedList.create((int)param.page, 10, RdoModel.Lista(param));
        }

        [HttpPost]
        public HttpResponseMessage Salvar([FromBody] dynamic param)
        {
            try
            {
                RdoViewModel retorno = RdoModel.Salvar(param);
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPost]
        public bool Deletar([FromBody] dynamic param)
        {
            return RdoModel.Deletar(param);
        }

        [HttpPost]
        public RdoViewModel ObterRdo([FromBody] dynamic param)
        {
            return RdoModel.ObterRegistro(param);
        }

        [HttpPost]
        public byte[] GerarDocumentoRdo([FromBody] dynamic param)
        {
            int idRdo = param.idRdo ?? 0;
            bool gerarRelatorioFotografico = param.gerarRelatorioFotografico;
            return RdoModel.GerarDocumentoRdo(idRdo, gerarRelatorioFotografico);
        }

        [HttpPost]
        public bool Assinar([FromBody] dynamic param)
        {
            return RdoModel.Assinar(param);
        }
    }
}