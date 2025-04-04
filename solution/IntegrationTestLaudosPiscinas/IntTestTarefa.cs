using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using RestSharp;
using System.Collections.Generic;

namespace IntegrationTestRdo
{
    [TestClass]
    public class IntTestTarefa
    {
        public string urlBase = "http://localhost:58950/api/";

        [TestMethod]
        public void IntAlterarTarefa()
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            dynamic tarefa = new System.Dynamic.ExpandoObject();
            tarefa.Id = 1458;
            tarefa.Descricao = "Teste 21930092019";
            tarefa.Status = 1;
            tarefa.Marcado = false;
            tarefa.ContratanteContratada = "d";
            tarefa.IdColaboradorInsercao = 16;
            tarefa.DataMedicao = DateTime.ParseExact("01/09/2019", "MM/dd/yyyy", ci);
            tarefa.DataMedicaoTela = "01/09/2019 00:00:00";
            tarefa.DataPrevisaoFim = "30/09/2019";
            tarefa.DataInicio = "01/09/2019";
            tarefa.IdEtapa = 118;
            tarefa.IdUnidade = 13;
            tarefa.qtdPlanejada = 25;
            tarefa.valor = Convert.ToDecimal("3,5");
            tarefa.ExisteExecucao = false;
            tarefa.ListaImagens = new List<System.Dynamic.ExpandoObject>();
            tarefa.listaHistoricoTarefa = new List<System.Dynamic.ExpandoObject>();

            dynamic HistoricoTarefa = new System.Dynamic.ExpandoObject();
            HistoricoTarefa.CPFColaborador = "29600570400";
            HistoricoTarefa.DataStatus = DateTime.ParseExact("01/09/2019", "MM/dd/yyyy", ci);
            HistoricoTarefa.DescricaoStatusTarefa = "Planejada";
            HistoricoTarefa.IdTarefa = 1458;
            HistoricoTarefa.NomeColaborador = "Nivaldo Santana";

            tarefa.listaHistoricoTarefa.Add(HistoricoTarefa);

            var client = new RestClient(urlBase);
            var request = new RestRequest("Tarefa/Salvar", Method.POST);
            request.AddJsonBody(tarefa);

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            Assert.IsTrue(response.IsSuccessful);
        }
        
        [TestMethod]
        public void IntObterTarefa()
        {
            dynamic param = new System.Dynamic.ExpandoObject();
            param = 1458;
            
            var client = new RestClient(urlBase);
            var request = new RestRequest("Tarefa/ObterTarefa", Method.POST);
            request.AddJsonBody(param);

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            Assert.IsTrue(response.IsSuccessful);
        }
    }
}
