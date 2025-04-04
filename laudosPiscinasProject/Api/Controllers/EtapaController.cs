using laudosPiscinasProject.Api.Models;
using System;
using System.Web.Http;

namespace laudosPiscinasProject.Api.Controllers
{
    public class EtapaController : ApiController
    {
        public IHttpActionResult Post(EtapaViewModel param)
        {
            var result = EtapaModel.Create(param);
            return Ok(result);
        }

        public IHttpActionResult Get(int? id = null, string titulo = "", int? idobra = null, string descricao = "",
            string dataInicial = null, string dataFinalPlanejada = null, string dataInicialExecutada = null,
            string dataFinalExecutada = null, int statusTarefa = 0)
        {
            var filter = new EtapaViewModel
            {
                Id = id ?? 0,
                Titulo = titulo,
                IdObra = idobra ?? 0,
                descricao = descricao,
                dataInicial = string.IsNullOrEmpty(dataInicial) ? DateTime.MinValue : Convert.ToDateTime(dataInicial),
                dataFinalPlanejada = string.IsNullOrEmpty(dataFinalPlanejada) ? default(DateTime) : Convert.ToDateTime(dataFinalPlanejada),
                dataInicialExecutada = string.IsNullOrEmpty(dataInicialExecutada) ? default(DateTime) : Convert.ToDateTime(dataInicialExecutada),
                dataFinalExecutada = string.IsNullOrEmpty(dataFinalExecutada) ? default(DateTime) : Convert.ToDateTime(dataFinalExecutada),
                idStatus = statusTarefa
            };
            var result = EtapaModel.Retrieve(filter);
            return Ok(result);
        }

        [HttpPost]
        [Route("api/Etapa/CarregarLista")]
        public PagedList CarregarLista(dynamic param)
        {
            return PagedList.create((int)param.page, 10, EtapaModel.Lista(param));
        }

        public IHttpActionResult Put(int id, EtapaViewModel param)
        {
            param.Id = id;
            var result = EtapaModel.Update(param);
            return Ok(result);
        }

        public IHttpActionResult Delete(int id)
        {
            var result = EtapaModel.Delete(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("api/Etapa/excluir")]
        public int excluir([FromBody] int id)
        {
            var result = EtapaModel.Delete(id);
            return result;
        }
        
        [HttpPost]
        [Route("api/Etapa/atualizarEtapa")]
        public int atualizarEtapa([FromBody] EtapaViewModel etapa)
        {
            var result = EtapaModel.AtualizarEtapa(etapa);
            return result;
        }

        [HttpPost]
        [Route("api/Etapa/obterEtapa")]
        public EtapaViewModel obterEtapa([FromBody] int id)
        {
            var result = EtapaModel.ObterEtapa(id);
            return result;
        }

        [HttpPost]
        [Route("api/Etapa/AdicionaRecupera")]
        public IHttpActionResult AdicionaRecupera(EtapaViewModel param)
        {
            var result = EtapaModel.CreateRetrieve(param);
            return Ok(result);
        }

        [HttpPost]
        [Route("api/Etapa/ObterTarefasParaRDO")]
        public IHttpActionResult ObterTarefasParaRDO(EtapaViewModel param)
        {
            //var filter = new EtapaViewModel { DataMedicao = dataRDO.Date };
            var result = EtapaModel.ObterEtapasParaRDO(param);            
            return Ok(result);
        }

        [HttpGet]
        [Route("api/Etapa/ObterEtapaTarefa")]
        public IHttpActionResult ObterEtapaTarefa(int? id = null, string titulo = "", int? idobra = null, string descricao = "",
            string dataInicial = null, string dataFinalPlanejada = null, string dataInicialExecutada = null,
            string dataFinalExecutada = null, int statusTarefa = 0)
        {
            var filter = new EtapaViewModel
            {
                Id = id ?? 0,
                Titulo = titulo,
                IdObra = idobra ?? 0,
                descricao = descricao,
                dataInicial = string.IsNullOrEmpty(dataInicial) ? DateTime.MinValue : Convert.ToDateTime(dataInicial),
                dataFinalPlanejada = string.IsNullOrEmpty(dataFinalPlanejada) ? default(DateTime) : Convert.ToDateTime(dataFinalPlanejada),
                dataInicialExecutada = string.IsNullOrEmpty(dataInicialExecutada) ? default(DateTime) : Convert.ToDateTime(dataInicialExecutada),
                dataFinalExecutada = string.IsNullOrEmpty(dataFinalExecutada) ? default(DateTime) : Convert.ToDateTime(dataFinalExecutada),
                idStatus = statusTarefa
            };
            var result = EtapaModel.ObterEtapaTarefa(filter);
            return Ok(result);
        }
    }
}
