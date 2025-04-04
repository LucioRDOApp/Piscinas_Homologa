using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static laudosPiscinasProject.Api.Models.EquipamentosModel;

namespace laudosPiscinasProject.Api.Controllers
{
    public class EquipamentosController : ApiController
    {
        
        
        public PagedList CarregarLista(dynamic param)
        {
            return PagedList.create((int)param.page, 10, EquipamentosModel.Lista(param));
        }

        [HttpPost]
        public EquipamentoViewModel Salvar([FromBody] EquipamentoViewModel equipamento)
        {
            EquipamentoViewModel resultado = EquipamentosModel.Salvar(equipamento);
            return resultado;
        }

        [HttpPost]
        public bool Deletar([FromBody] dynamic param)
        {
            return EquipamentosModel.Deletar(param);
        }

        [HttpPost]
        public HttpResponseMessage VerificarExclusao([FromBody] dynamic param)
        {
            try
            {
                bool retorno = EquipamentosModel.VerificarExclusao(param);
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPost]
        public EquipamentoViewModel ObterEquipamento([FromBody] dynamic param)
        {
            return EquipamentosModel.ObterRegistro(param);
        }

        [HttpPost]
        public string ListaMarca()
        {
            return EquipamentosModel.ListaMarca();
        }

        [HttpPost]
        public string ListaModelo()
        {
            return EquipamentosModel.ListaModelo();
        }

        [HttpPost]
        public string ListaTipoEquipamento()
        {
            return EquipamentosModel.ListaTipoEquipamento();
        }


        [HttpGet]
        public IHttpActionResult GetTipoEquipamento()
        {
            return  Ok(TipoEquipamentoModel.Retrieve(new TipoEquipamentoViewModel()));
        }

        //[HttpPost]
        //public string ListaDescricao()
        //{
        //    return EquipamentosModel.ListaDescricao();
        //}
    }
}