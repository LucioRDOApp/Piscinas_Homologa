using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace UnitTestRdo
{
    [TestClass]
    public class UnitTestMedicao
    {
        public string urlBase = "http://localhost:58950/api/";

        [TestMethod]
        public void RelatorioMedicao()
        {
            dynamic param = new System.Dynamic.ExpandoObject();
            param.DataInicial = "01/09/2019 00:00:00";
            param.DataPrevisaoFinalObra = "04/10/2019 00:00:00";
            param.IdObra = 117;
            param.TipoRelatorio = "PDF";
            
            var client = new RestClient(urlBase);
            var request = new RestRequest("Medicao/GerarRelatorio", Method.POST);
            request.AddJsonBody(param);

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            Assert.IsTrue(response.IsSuccessful);
        }
    }
}
