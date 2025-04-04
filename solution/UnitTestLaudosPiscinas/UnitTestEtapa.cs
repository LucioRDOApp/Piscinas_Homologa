using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using laudosPiscinasProject.Api.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;

namespace UnitTestRdo
{
    [TestClass]
    public class UnitTestEtapa
    {
        [TestMethod]
        public void ListagemEtapas()
        {
            var filter = new EtapaViewModel
            {
                Id = 0,
                Titulo = "",
                IdObra = 0,
                descricao = "",
                dataInicial = DateTime.MinValue,
                dataFinalPlanejada = default(DateTime),
                dataInicialExecutada = default(DateTime),
                dataFinalExecutada = default(DateTime),
                idStatus = 0
            };

            List<EtapaViewModel> etapas = EtapaModel.Retrieve(filter);

            Assert.IsNotNull(etapas);
        }

        [TestMethod]
        public void CadastroEtapa()
        {
            EtapaViewModel etapa = new EtapaViewModel();
            etapa.Id = 0;
            etapa.IdObra = 117;
            etapa.Ordem = 6;
            etapa.Titulo = "Teste Unitario 2";
            
            int reponse = EtapaModel.Create(etapa);

            Assert.IsTrue(reponse == 1);
        }


        [TestMethod]
        [ExpectedException(typeof(DbUpdateException),
        "Etapa sem o id da obra.")]
        public void CadastroEtapaSemIdObraErro()
        {
            EtapaViewModel etapa = new EtapaViewModel();
            etapa.Id = 0;
            etapa.IdObra = 0;
            etapa.Ordem = 6;
            etapa.Titulo = "Teste Unitario 2";

            int reponse = EtapaModel.Create(etapa);

            Assert.IsTrue(reponse == 1);
        }

        [TestMethod]
        public void AlterarEtapa()
        {
            EtapaViewModel etapa = new EtapaViewModel();
            etapa.Id = 122;
            etapa.IdObra = 117;
            etapa.Ordem = 6;
            etapa.Titulo = "Teste Unitario 21";

            int reponse = EtapaModel.Update(etapa);

            Assert.IsTrue(reponse == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException),
        "Etapa sem o id da obra.")]
        public void AlterarEtapaSemIdObraErro()
        {
            EtapaViewModel etapa = new EtapaViewModel();
            etapa.Id = 122;
            etapa.IdObra = 0;
            etapa.Ordem = 6;
            etapa.Titulo = "Teste Unitario 21";

            int reponse = EtapaModel.Update(etapa);

            Assert.IsTrue(reponse == 1);
        }

        [TestMethod]
        public void ExcluirEtapa()
        {
            int idEtapa = 120;
            int reponse = EtapaModel.Delete(idEtapa);

            Assert.IsTrue(reponse == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException),
        "Etapa sem o id da obra.")]
        public void ExcluirEtapaComEtapaInexistente()
        {
            int idEtapa = 0;
            int reponse = EtapaModel.Delete(idEtapa);

            Assert.IsTrue(reponse == 1);
        }
    }
}
