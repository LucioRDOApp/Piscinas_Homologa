using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static laudosPiscinasProject.Api.Models.GraficoModel;

namespace laudosPiscinasProject.Api.Controllers
{
    public class GraficoController : ApiController
    {
        [HttpPost]
        public ICollection<RdoGeradoViewModel> ContagemRdoGerado([FromBody] dynamic param)
        {
            var ColaboradorId = param.ColaboradorId.Value ?? 0;

            if (ColaboradorId == 0)
            {
                throw new Exception("O Colaborador não foi fornecido.");
            }

            try
            {
                return GraficoModel.ContarRdosGerados((int)ColaboradorId, param);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public ICollection<RdoAtrasadoViewModel> ContagemRdoAtrasado([FromBody] dynamic param)
        {
            var ColaboradorId = param.ColaboradorId ?? 0;

            if (ColaboradorId == 0)
            {
                throw new Exception("O Colaborador não foi fornecido.");
            }

            return GraficoModel.ContarRdosAtrasados((int)ColaboradorId, param);
        }

        [HttpPost]
        public ICollection<DiaImprodutivoViewModel> ContagemDiaImprodutivo([FromBody] dynamic param)
        {
            var ColaboradorId = param.ColaboradorId ?? 0;

            if (ColaboradorId == 0)
            {
                throw new Exception("O Colaborador não foi fornecido.");
            }

            return GraficoModel.ContarDiasImprodutivos((int)ColaboradorId, param);
        }

        [HttpPost]
        public ICollection<GraficoModel.TarefaViewModel> ContagemTarefa([FromBody] dynamic param)
        {
            var ColaboradorId = param.ColaboradorId ?? 0;

            if (ColaboradorId == 0)
            {
                throw new Exception("O Colaborador não foi fornecido.");
            }

            return GraficoModel.ContarTarefa((int)ColaboradorId, param);
        }

        [HttpPost]
        public ICollection<StatusTarefaGraficoViewModel> ContagemStatusTarefa([FromBody] dynamic param)
        {
            var ColaboradorId = param.ColaboradorId ?? 0;

            if (ColaboradorId == 0)
            {
                throw new Exception("O Colaborador não foi fornecido.");
            }

            return GraficoModel.ContarStatusTarefa((int)ColaboradorId, param);
        }

        [HttpPost]
        public ICollection<ComentarioViewModel> ContagemComentario([FromBody] dynamic param)
        {
            var ColaboradorId = param.ColaboradorId ?? 0;

            if (ColaboradorId == 0)
            {
                throw new Exception("O Colaborador não foi fornecido.");
            }

            return GraficoModel.ContarComentario((int)ColaboradorId, param);
        }
    }
}