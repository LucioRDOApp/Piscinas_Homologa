using System.Collections.Generic;
using System.Web.Http;
using laudosPiscinasProject.Api.Models;

namespace laudosPiscinasProject.Api.Controllers
{
    public class MunicipioController : ApiController
    {
        [HttpPost]
        public List<MunicipioViewModel> ListaMunicipio([FromBody] dynamic param)
        {
            return MunicipioModel.ListaMunicipio((int)param);
        }

        public object ListaUF()
        {
            return MunicipioModel.ListaUF();
        }


        [HttpPost]
        public List<MunicipioViewModel> ListaMunicipioConvidada([FromBody] dynamic param)
        {
            return MunicipioModel.ListaMunicipioConvidada((int)param);
        }

        public object ListaUFConvidada()
        {
            return MunicipioModel.ListaUFConvidada();
        }
    }
}
