using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace laudosPiscinasProject.Api.Controllers
{
    public class LaudoController : ApiController
    {
        [HttpPost]
        public List<LaudoViewModel> DashboardGrafico(dynamic param)
        {
            return LaudoModel.DashboardGrafico(param);
        }

        [HttpPost]
        public PagedList CarregarLista(dynamic param)
        {
            return PagedList.create((int)param.page, 10, LaudoModel.Lista(param));
        }

        [HttpPost]
        public HttpResponseMessage Salvar([FromBody] dynamic param)
        {
            try
            {
                LaudoViewModel retorno = LaudoModel.Salvar(param);
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                // Obtenha o nome do arquivo de log
                var logFileName = "C:\\inetpub\\wwwroot\\piscinas\\Uploads\\LaudoModel_Log.txt";

                // Crie ou abra o arquivo de log
                using (var streamWriter = new StreamWriter(logFileName, true))
                {
                    // Escreva a mensagem de erro
                    streamWriter.WriteLine(ex.Message);

                    // Escreva a pilha de chamadas
                    streamWriter.WriteLine(ex.StackTrace);

                    // Escreva uma linha em branco
                    streamWriter.WriteLine();
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPost]
        public bool Deletar([FromBody] dynamic param)
        {
            return RdoModel.Deletar(param);
        }

        [HttpPost]
        public RdoViewModel ObterRdo([FromBody] dynamic param)
        {
            return RdoModel.ObterRegistro(param);
        }

        [HttpPost]
        public byte[] GerarDocumentoLaudo([FromBody] dynamic param)
        {
            try
            {
                int idRdo = param.idRdo ?? 0;
                bool gerarRelatorioFotografico = param.gerarRelatorioFotografico;
                return LaudoModel.GerarDocumentoRdo(idRdo, gerarRelatorioFotografico);
            }catch(Exception ex)
            {
                // Obtenha o nome do arquivo de log
                var logFileName = "C:\\inetpub\\wwwroot\\piscinas\\Uploads\\LaudoModel_Log.txt";

                // Crie ou abra o arquivo de log
                using (var streamWriter = new StreamWriter(logFileName, true))
                {
                    // Escreva a mensagem de erro
                    streamWriter.WriteLine(ex.Message);

                    // Escreva a pilha de chamadas
                    streamWriter.WriteLine(ex.StackTrace);

                    // Escreva uma linha em branco
                    streamWriter.WriteLine();
                }
            }

            return null;
        }

        [HttpPost]
        public bool Assinar([FromBody] dynamic param)
        {
            return RdoModel.Assinar(param);
        }
    }
}