using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace laudosPiscinasProject.Api.Controllers
{
    public class EfetivoController : ApiController
    {
        public PagedList CarregarLista(dynamic param)
        {
            return PagedList.create((int)param.page, 100, EfetivoModel.Lista(param));
        }
        public ICollection<EfetivoStatusViewModel> CarregarListaEfetivoStatus()
        {
            return EfetivoModel.CarregarListaEfetivoStatus();
        }
        [HttpGet]
        public HttpResponseMessage UltimaDataLancada(string oco_id_obra)
        {
            try
            {
                DateTime retorno = EfetivoModel.UltimaDataLancada(oco_id_obra);
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }

        }
        [HttpPost]
        public EfetivoViewModel ChangeEfetivoStatus([FromBody] EfetivoViewModel param)
        {
            EfetivoViewModel resultado = EfetivoModel.ChangeEfetivoStatus(param);
            return resultado;
        }
        [HttpPost]
        public HttpResponseMessage GerarRelatorio(RelatorioEfetivoViewModel filter)
        {
            try
            {
                byte[] retorno = RelatorioEfetivoModel.Efetivo(filter);
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        [HttpPost]
        public bool CopiarEfetivo(dynamic param)
        {
            var retorno = EfetivoModel.CopiarEfetivo(param);
            return retorno;
        }
    }
}