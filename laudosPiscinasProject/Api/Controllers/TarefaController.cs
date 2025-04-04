using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace laudosPiscinasProject.Api.Controllers
{
    public class TarefaController : ApiController
    {
        [HttpPost]
        public PagedList CarregarLista(dynamic param)
        {
            return PagedList.create((int)param.page, 10, TarefaModel.Lista(param));
        }

        [HttpPost]
        public List<TarefaViewModel> CarregarListaSimples(dynamic param)
        {
            return TarefaModel.Lista(param);
        }

        [HttpPost]
        public List<HistoricoTarefaViewModel> CarregarHistoricoTarefa(dynamic param)
        {
            int id = Convert.ToInt32(Convert.ToString(param.id)); 
            return TarefaModel.PreencherHistoricoTarefa(id);
        }

        public List<TarefaViewModel> CarregarListaTarefasRdo(dynamic param)
        {
            return TarefaModel.ListaRdo(param);
        }

        //[HttpPost]
        //public bool SalvarCabeca([FromBody] dynamic param)
        //{
        //    var resultado = TarefaModel.Salvar(param);
        //    return resultado;
        //}
        [HttpPost]
        public HttpResponseMessage Salvar([FromBody] TarefaViewModel param)
        {
            try
            {
                bool resultado = false;
                if (param.Id > 0)
                {
                    resultado = TarefaModel.Update(param) > 0;
                }
                else
                {
                    resultado = TarefaModel.Salvar(param);
                }
                return Request.CreateResponse(HttpStatusCode.OK, resultado);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public bool AtualizarStatus([FromBody] dynamic param)
        {
            var resultado = TarefaModel.AtualizarStatus(param);
            return true;
        }
        
        public HttpResponseMessage AtualizarStatusEmMassa([FromBody] dynamic param)
        {
            try
            {
                bool retorno = TarefaModel.AtualizarStatusEmMassa(param);
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
            int idTarefa = Convert.ToInt32(param.id);
            return TarefaModel.Deletar(idTarefa);
        }

        [HttpPost]
        public TarefaViewModel ObterTarefa([FromBody] dynamic param)
        {
            return TarefaModel.ObterRegistro(param);
        }

        [HttpPost]
        public object ObterImagensTarefa([FromBody] dynamic param)
        {
            //return ImagemModel.ObterImagensTarefa(param);
            return ImagemModel.ObterImagens((int)param);
        }
    }
}