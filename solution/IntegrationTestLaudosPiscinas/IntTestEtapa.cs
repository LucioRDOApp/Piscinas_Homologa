using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace IntegrationTestRdo
{
    [TestClass]
    public class IntTestEtapa
    {
        public string urlBase = "http://localhost:58950/api/";

        [TestMethod]
        public void IntListagemEtapas()
        {
            dynamic param = new System.Dynamic.ExpandoObject();
            param.Id = 0;
            param.Titulo = "";
            param.IdObra = 0;
            param.descricao = "";
            param.dataInicial = DateTime.MinValue;
            param.dataFinalPlanejada = default(DateTime);
            param.dataInicialExecutada = default(DateTime);
            param.dataFinalExecutada = default(DateTime);
            param.idStatus = 0;
           
            var client = new RestClient(urlBase);
            var request = new RestRequest("Etapa/Get", Method.GET);
            request.AddJsonBody(param);

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            Assert.IsTrue(response.IsSuccessful);
        }

        [TestMethod]
        public void IntCadastroEtapa()
        {
            dynamic param = new System.Dynamic.ExpandoObject();
            param.Id = 0;
            param.IdObra = 117;
            param.Ordem = 6;
            param.Titulo = "Teste Unitario 2";
            
            var client = new RestClient(urlBase);
            var request = new RestRequest("Etapa/Post", Method.POST);
            request.AddJsonBody(param);

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            Assert.IsTrue(response.IsSuccessful);
        }
        
        [TestMethod]
        public void IntAlterarEtapa()
        {
            dynamic param = new System.Dynamic.ExpandoObject();
            string id = "131";
            param.Id = 131;
            param.IdObra = 117;
            param.Ordem = 6;
            param.Titulo = "Teste Unitario 21";
            
            var client = new RestClient(urlBase);
            var request = new RestRequest("Etapa/Put/" + id, Method.PUT);
            request.AddJsonBody(param);
            
            IRestResponse response = client.Execute(request);
            var content = response.Content;

            Assert.IsTrue(response.IsSuccessful);
        }
        
        [TestMethod]
        public void IntExcluirEtapa()
        {
            //dynamic param = new System.Dynamic.ExpandoObject();
            string param = "131";
            
            var client = new RestClient(urlBase);
            var request = new RestRequest("Etapa/Delete/" + param, Method.DELETE);
            request.AddJsonBody(param);

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            Assert.IsTrue(response.IsSuccessful);
        }

    }
}
