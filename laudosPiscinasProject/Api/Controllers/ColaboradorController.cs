using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static laudosPiscinasProject.Api.Models.ColaboradorModel;

namespace laudosPiscinasProject.Api.Controllers
{
    public class ColaboradorController : ApiController
    {
        public PagedList CarregarLista(dynamic param)
        {
            return PagedList.create((int)param.page, 10, ColaboradorModel.Lista(param));
        }

        [HttpPost]
        public ColaboradorViewModel Salvar([FromBody] ColaboradorViewModel param)
        {
            ColaboradorViewModel resultado = ColaboradorModel.Salvar(param, true);
            return resultado;
        }

        [HttpPost]
        public ColaboradorViewModel ObterColaboradorCPF([FromBody] dynamic param)
        {
            return ColaboradorModel.ObterColaboradorCPF(param);
        }

        [HttpGet]
        public IHttpActionResult ObterColaboradorNome(string nome)
        {
            var filter = new ColaboradorViewModel(){ Nome = nome };
            return  Ok(ColaboradorModel.ObterColaboradorNome(filter));
        }

        [HttpPost]
        public bool Deletar([FromBody] dynamic param)
        {
            return ColaboradorModel.Deletar(param);
        }

        [HttpPost]
        public ColaboradorViewModel ObterColaborador([FromBody] dynamic param)
        {
            return ColaboradorModel.ObterRegistro(param);
        }


        [HttpPost]
        public bool AtualizarSenha([FromBody] dynamic param)
        {
            return ColaboradorModel.AtualizarSenha(param);
        }

        [HttpPost]
        public HttpResponseMessage RecuperarSenha([FromBody] dynamic param)
        {
            try
            {
                string retorno = ColaboradorModel.RecuperarSenha(param);
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage ObterColaboradoresPorPerfil([FromBody] dynamic param)
        {
            try
            {
                int idPerfil = Convert.ToInt32(param); //O perfil na verdade é o grupo do colaborador
                List<ColaboradorViewModel> retorno = ColaboradorModel.ObterColaboradorPorPerfil(idPerfil);
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPost]
        public ColaboradorViewModel ObterColaboradorDoSistema([FromBody] dynamic param)
        {
            int idColaborador = Convert.ToInt32(param);
            return ColaboradorModel.ObterColaboradorDoSistema(idColaborador); //Obter o colaborador existente no sistema
        }
    }
}