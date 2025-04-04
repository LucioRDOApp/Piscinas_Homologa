using laudosPiscinasProject.Api.Models;
using System.Web.Http;

namespace laudosPiscinasProject.Api.Controllers
{
    public class RelatorioProdutividadeController : ApiController
    {
        [HttpPost]
        public byte[] GerarRelatorio([FromBody] dynamic param)
        {
            return RelatorioProdutividadeModel.GerarRelatorio(param);
        }    
    }
}