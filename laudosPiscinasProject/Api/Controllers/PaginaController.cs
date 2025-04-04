using laudosPiscinasProject.Api.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace laudosPiscinasProject.Api.Controllers
{
    public class PaginaController : ApiController
    {
        [HttpPost]
        public PagedList CarregarLista(dynamic param)
        {
            return PagedList.create((int)param.page, 10, PaginaModel.Lista(param));
        }

        [HttpPost]
        public List<PaginaViewModel> Lista(dynamic param)
        {
            return PaginaModel.Lista(param);
        }

        [HttpPost]
        public bool Salvar([FromBody] dynamic param)
        {
            var resultado = PaginaModel.Salvar(param);            
            return resultado;
        }

        [HttpPost]
        public PaginaViewModel ObterPagina([FromBody] dynamic param)
        {
            return PaginaModel.ObterRegistro(param);
        }

        [HttpPost]
        public object CarregaListaAcao()
        {
            return PaginaModel.CarregaListaAcao();
        }

        [HttpPost]
        public bool Deletar([FromBody] dynamic param)
        {
            return PaginaModel.Deletar(param);
        }
    }
}
