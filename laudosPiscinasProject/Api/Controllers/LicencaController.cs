using laudosPiscinasProject.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AngularWebApi.Api.Controllers
{
    public class LicencaController : ApiController
    {
        [HttpGet]
        public List<LicencaViewModel> ObterLicencas()
        {
            return LicencaModel.Lista();
        }

        [HttpPost]
        public bool Atualizar([FromBody] dynamic param)
        {
            return LicencaModel.Atualizar(param);
        }

        [HttpPost]
        public bool Remover([FromBody] dynamic param)
        {
            return LicencaModel.Remove(param);
        }

    }

}
