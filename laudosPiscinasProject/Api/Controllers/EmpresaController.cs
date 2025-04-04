using laudosPiscinasProject.Api.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace laudosPiscinasProject.Api.Controllers
{
    public class EmpresaController : ApiController
    {
        [HttpPost]
        public List<EmpresaViewModel> ListaEmpresa(dynamic param)
        {
            return EmpresaModel.ListaEmpresa(param);
        }

        [HttpPost]
        public PagedList CarregarLista(dynamic param)
        {
            return PagedList.create((int)param.page, 10, EmpresaModel.Lista(param));
        }

        [HttpPost]
        public EmpresaViewModel Salvar([FromBody] dynamic param)
        {
            return EmpresaModel.Salvar(param, true);
        }

        [HttpPost]
        public bool Deletar([FromBody] dynamic param)
        {
            return EmpresaModel.Deletar(param);
        }

        [HttpPost]
        public EmpresaViewModel ObterEmpresaCNPJ([FromBody] dynamic param)
        {
            return EmpresaModel.ObterRegistroCNPJ(param);
        }

        [HttpPost]
        public EmpresaViewModel ObterEmpresa([FromBody] dynamic param)
        {
            return EmpresaModel.ObterRegistro(param);
        }

        [HttpPost]
        public bool VerificarPermissaoLicencaEmpresa([FromBody] dynamic param)
        {
            return EmpresaModel.VerificarPermissaoLicencaEmpresa(param);
        }
    }
}
