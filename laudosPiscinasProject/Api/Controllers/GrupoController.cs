using LaudosPiscinasClass;
using laudosPiscinasProject.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AngularWebApi.Api.Controllers
{
    public class GrupoController : ApiController
    {
        [HttpPost]
        public GrupoViewModel ObterGrupo([FromBody] dynamic param)
        {
            return GrupoModel.ObterRegistro(param);
        }

        [HttpPost]
        public bool Deletar([FromBody] dynamic param)
        {
            return GrupoModel.Deletar(param);
        }

        [HttpPost]
        public object CarregaListaGrupoPaginaAcao([FromBody] dynamic param)
        {
            return GrupoModel.CarregaListaGrupoPaginaAcao(param);
        }

        [HttpPost]
        public bool Salvar([FromBody] dynamic param)
        {
            var resultado = GrupoModel.Salvar(param);
            return resultado;
        }

        public PagedList CarregarLista(dynamic param)
        {
            return PagedList.create((int)param.page, 10, GrupoModel.Lista(param));
            //return GrupoModel.Lista(param);
        }

        public List<GrupoViewModel> Lista(dynamic param)
        {
            return GrupoModel.Lista(param);
        }

        public List<GrupoViewModel> ListaPeloGrupo(dynamic param)
        {
            return GrupoModel.ListaPeloGrupo(param);
        }
        

        //não sei qual é o caso desse método. já estava ai. ver se já esta sendo usado ou se vai ser usado depois.
        public bool AcaoEstaDisponivelGrupoPagina(grupo grupo, int idAcao, int idPag)
        {
            try
            {
                if (grupo == null)
                {
                    return false;
                }

                List<grupo_pagina_acao> ListGrupoPaginaAcao = grupo.grupo_pagina_acao.ToList();
                if (ListGrupoPaginaAcao.Where(gpa => gpa.pagina_acao.pagina.pag_id_pagina == idPag && gpa.pagina_acao.acao.aca_id_acao == idAcao && gpa.gpa_id_grupo == grupo.gru_id_grupo).Count() > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

    }

}
