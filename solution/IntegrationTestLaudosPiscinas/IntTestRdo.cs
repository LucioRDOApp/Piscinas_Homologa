using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using RestSharp;

namespace IntegrationTestRdo
{
    [TestClass]
    public class IntTestRdo
    {
        public string urlBase = "http://localhost:58950/api/";

        [TestMethod]
        public void IntSalvarRdo()
        {
            dynamic dymRdo = new System.Dynamic.ExpandoObject();
            dymRdo.dataRdo = "02/09/2019";
            dymRdo.idRdo = 650;
            dymRdo.idObra = 117;
            dymRdo.idColaborador = 16;
            dymRdo.comentario = "Teste";
            dymRdo.climaManhaCheckValue = "b";
            dymRdo.climaTardeCheckValue = "b";
            dymRdo.climaNoiteCheckValue = "b";
            dymRdo.chuvaManhaCheckValue = "s";
            dymRdo.chuvaTardeCheckValue = "s";
            dymRdo.chuvaNoiteCheckValue = "s";
            dymRdo.improdutividadeCondicoesClimaticas = true;
            dymRdo.improdutividadeMateriais = true;
            dymRdo.improdutividadeParalizacoes = true;
            dymRdo.improdutividadeEquipamentos = false;
            dymRdo.improdutividadeContratante = true;
            dymRdo.improdutividadeFornecedores = true;
            dymRdo.improdutividadeMaodeObra = false;
            dymRdo.improdutividadeProjeto = true;
            dymRdo.improdutividadePlanejamento = true;
            dymRdo.improdutividadeAcidente = false;

            dymRdo.listaImagems = new List<System.Dynamic.ExpandoObject>();

            dynamic dymImagem = new System.Dynamic.ExpandoObject();
            dymImagem.idImagem = 21;

            dymRdo.listaImagems.Add(dymImagem);

            dymRdo.listaTarefas = new List<System.Dynamic.ExpandoObject>();

            dynamic dymTarefa = new System.Dynamic.ExpandoObject();
            dymTarefa.id = 1445;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1446;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1447;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1448;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1457;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1459;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1449;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1450;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1451;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1452;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1453;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1454;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1455;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1456;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1458;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1460;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1448;
            dymRdo.listaTarefas.Add(dymTarefa);

            dymTarefa.id = 1447;
            dymRdo.listaTarefas.Add(dymTarefa);

            var client = new RestClient(urlBase);
            var request = new RestRequest("Rdo/Salvar", Method.POST);
            request.AddJsonBody(dymRdo);

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            Assert.IsTrue(response.IsSuccessful);
        }


        [TestMethod]
        public void IntGerarDocumentoRdo()
        {
            dynamic param = new System.Dynamic.ExpandoObject();
            param.idRdo = 650;

            var client = new RestClient(urlBase);
            var request = new RestRequest("Rdo/GerarDocumentoRdo", Method.POST);
            request.AddJsonBody(param);

            IRestResponse response = client.Execute(request);
            var content = response.Content;

            Assert.IsTrue(response.IsSuccessful);
        }
    }
}
