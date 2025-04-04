using laudosPiscinasProject.Api.Models;
using System.Collections.Generic;
using System.Web.Http;
using static laudosPiscinasProject.Api.Models.CargoModel;

namespace laudosPiscinasProject.Api.Controllers
{
    public class CargoController : ApiController
    {
        public List<CargoViewModel> Lista()
        {
            return CargoModel.Lista();
        }
    }
}