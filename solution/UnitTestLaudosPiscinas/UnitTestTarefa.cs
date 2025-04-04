using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using laudosPiscinasProject.Api.Models;

namespace UnitTestRdo
{
    [TestClass]
    public class UnitTestTarefa
    {
        [TestMethod]
        public void AlterarTarefa()
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            TarefaViewModel tarefa = new TarefaViewModel();
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
            tarefa.ListaImagens = new System.Collections.Generic.List<dynamic>();
            tarefa.listaHistoricoTarefa.Add(
                new HistoricoTarefaViewModel()
                {
                    CPFColaborador = "29600570400",
                    DataStatus = DateTime.ParseExact("01/09/2019", "MM/dd/yyyy", ci),
                    DescricaoStatusTarefa = "Planejada",
                    IdTarefa = 1458,
                    NomeColaborador = "Nivaldo Santana"
                }
            );

            int response = TarefaModel.Update(tarefa);

            Assert.IsTrue(response == 1);

        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException),
        "Descrição com valor nulo.")]
        public void AlterarTarefaImagensNullErro()
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            TarefaViewModel tarefa = new TarefaViewModel();
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
            tarefa.listaHistoricoTarefa.Add(
                new HistoricoTarefaViewModel()
                {
                    CPFColaborador = "29600570400",
                    DataStatus = DateTime.ParseExact("01/09/2019", "MM/dd/yyyy", ci),
                    DescricaoStatusTarefa = "Planejada",
                    IdTarefa = 1458,
                    NomeColaborador = "Nivaldo Santana"
                }
            );

            int response = TarefaModel.Update(tarefa);

            Assert.IsTrue(response == 1);

        }

        [TestMethod]
        public void ObterTarefa()
        {
            int idTarefa = 1458;
            TarefaViewModel tarefa = TarefaModel.ObterRegistro(idTarefa);

            Assert.IsNotNull(tarefa);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException),
        "Id da Tarefa Inexistente.")]
        public void ObterTarefaIdInexistenteErro()
        {
            int idTarefa = 0;
            TarefaViewModel tarefa = TarefaModel.ObterRegistro(idTarefa);

            Assert.IsNotNull(tarefa);
        }
    }
}
