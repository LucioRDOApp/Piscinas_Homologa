using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static laudosPiscinasProject.Api.Models.ColaboradorModel;
using static laudosPiscinasProject.Api.Models.EquipamentosModel;

namespace laudosPiscinasProject.Api.Controllers
{
    public class ObraController : ApiController
    {
        [HttpPost]
        public List<ObraViewModel> ObterObras([FromBody] dynamic param)
        {
            return ObraModel.Lista(param);
        }

        public PagedList CarregarLista(dynamic param)
        {
            return PagedList.create((int)param.page, 10, ObraModel.Lista(param));
        }

        [HttpPost]
        public HttpResponseMessage Salvar([FromBody] dynamic param)
        {
            try
            {
                ObraViewModel resultado = ObraModel.Salvar(param);
                return Request.CreateResponse(HttpStatusCode.OK, resultado);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPost]
        public bool Deletar([FromBody] dynamic param)
        {
            return ObraModel.Deletar(param);
        }

        [HttpPost]
        public ObraViewModel ObterObra([FromBody] dynamic param)
        {
            try
            {
                return ObraModel.ObterRegistro(param);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public List<ColaboradorViewModel> ObterColaboradoresObra([FromBody] dynamic param)
        {
            return ObraModel.ObterColaboradoresObra(param);
        }

        [HttpPost]
        public List<EquipamentoViewModel> ObterEquipamentosObra([FromBody] dynamic param)
        {
            return ObraModel.ObterEquipamentosObra(param);
        }

        [HttpPost]
        public bool Convidar([FromBody] dynamic param)
        {
            return ObraModel.Convidar(param);
        }

        [HttpPost]
        public bool VerificarConvite([FromBody] dynamic param)
        {
            return ObraModel.VerificarConvite(param);
        }

        [HttpGet]
        public int QuantidadeMaximaFotos(int id)
        {
            return ObraModel.QuantidadeMaximaFotos(id);
        }

    }
}
