using LaudosPiscinasClass;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace laudosPiscinasProject.Api.Models
{
    public class RdoModel
    {
        private static CultureInfo cultureInfo = new CultureInfo("pt-BR");

        public static List<RdoViewModel> Lista(dynamic param)
        {
            int idObra = param.idObra ?? 0;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            List<rdo> query = context.Set<rdo>().ToList();

            query = query.Where(rdo => rdo.rdo_id_obra == idObra).ToList();

            int statusRdo = param.statusRdo ?? 0;
            DateTime dataInicial = String.IsNullOrEmpty(Convert.ToString(param.dataInicial)) ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataInicial));
            DateTime dataFinal = String.IsNullOrEmpty(Convert.ToString(param.dataFinal)) ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataFinal));

            if (statusRdo > 0)
            {
                query = query.Where(rdo => rdo.rdo_id_status == statusRdo).ToList();
            }
            if (dataInicial > DateTime.MinValue)
            {
                query = query.Where(rdo => rdo.rdo_dt_rdo >= dataInicial).ToList();
            }
            if (dataFinal > DateTime.MinValue)
            {
                query = query.Where(rdo => rdo.rdo_dt_rdo <= dataFinal).ToList();
            }

            List<RdoViewModel> Lista = new List<RdoViewModel>();
            query.OrderByDescending(x => x.rdo_dt_rdo).ToList().ForEach(rdo => Lista.Add(new RdoViewModel
            {
                DataRdo = rdo.rdo_dt_rdo == null || rdo.rdo_dt_rdo == DateTime.MinValue ? "" : rdo.rdo_dt_rdo.Date.ToString().Substring(0, 10),
                IdRdo = rdo.rdo_id_rdo,
                StatusRdo = rdo.rdo_id_status,
                Comentario = rdo.rdo_ds_comentario_assinatura,
                ComentarioAssinatura = rdo.rdo_ds_comentario_assinatura,
                DiaDaSemana = cultureInfo.DateTimeFormat.DayNames[(int)rdo.rdo_dt_rdo.Date.DayOfWeek],
                //rdo.rdo_dt_rdo.Date.DayOfWeek.ToString(),

                ClimaManhaCheckValue = rdo.rdo_ds_clima_manha,
                ClimaTardeCheckValue = rdo.rdo_ds_clima_tarde,
                ClimaNoiteCheckValue = rdo.rdo_ds_clima_noite,
                ChuvaManhaCheckValue = rdo.rdo_ds_chuva_manha,
                ChuvaTardeCheckValue = rdo.rdo_ds_chuva_tarde,
                ChuvaNoiteCheckValue = rdo.rdo_ds_chuva_noite,

                //QtdTarefas = rdo.historico_tarefa_rdo.Count,
                QtdTarefas = rdo.rdo_tarefa.GroupBy(et => et.tarefa.tar_nr_agrupador).ToList().Count,
                QtdMaquinas = ObterQuantidadeMaquinas(rdo),
                QtdColaboradores = ObterQuantidadeColaboradores(rdo),
                statusContratanteContratadaDonoRdo = rdo.colaborador.obra_colaborador.FirstOrDefault(col => col.oco_id_obra == rdo.rdo_id_obra).oco_st_contratante_contratada,
                DescricaoStatus = rdo.status_rdo.str_ds_status

            }));


            return Lista;
        }
        public static int ObterQuantidadeMaquinas(rdo objRdo)
        {
            int qtdMaquinas = 0;

            foreach (var tarefa in objRdo.rdo_tarefa)
            {
                qtdMaquinas += tarefa.tarefa.obra_tarefa_equipamento.Count;
            }

            return qtdMaquinas;
        }
        public static int ObterQuantidadeColaboradores(rdo objRdo)
        {
            int qtdColaboradores = 0;
            List<colaborador> colaboradores = new List<colaborador>();

            foreach (var tarefa in objRdo.rdo_tarefa)
            {
                tarefa.tarefa.obra_tarefa_colaborador.Select(otc => otc.obra_colaborador.colaborador).ToList().ForEach(otc => colaboradores.Add(otc));
            }
            qtdColaboradores = colaboradores.GroupBy(otc => otc.col_nm_colaborador).Count();
            return qtdColaboradores;
        }
        public static bool ExisteRdoPendente(dynamic param)
        {
            int idObra = param.idObra ?? 0;
            bool returnValue = false;
            string data = param.dataRdo.ToString();

            DateTime dataRdo = Convert.ToDateTime(data);
            DateTime dataRdoAnterior = dataRdo.AddDays(-1);
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            rdo _rdo = context.rdo.Where(x => x.rdo_dt_rdo == dataRdoAnterior && x.rdo_id_obra == idObra).FirstOrDefault() ?? new rdo();
            obra _obra = context.obra.Where(x => x.obr_id_obra == idObra).FirstOrDefault() ?? new obra();
            if (_obra.obr_dt_inicio.Date == dataRdo.Date)
            {
                return false;
            }

            returnValue = _rdo.rdo_id_rdo > 0;
            return !returnValue;
        }

        public static RdoViewModel Salvar(dynamic param)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            if (string.IsNullOrEmpty(param.dataRdo.ToString()))
            {
                throw new Exception("A data deve ser preenchida");
            }

            int idColaborador = param.idColaborador ?? 0;
            int idObra = param.idObra ?? 0;
            DateTime dataRdo = Convert.ToDateTime(param.dataRdo.ToString());
            obra _obra = context.obra.FirstOrDefault(x => x.obr_id_obra == idObra);

            if (_obra != null && dataRdo < _obra.obr_dt_inicio)
            {
                throw new Exception("Não é possível gerar um RDO anterior a data inicial da obra.");
            }
            if (_obra != null && _obra.obr_dt_fim != null && _obra.obr_dt_fim != DateTime.MinValue && dataRdo > _obra.obr_dt_fim)
            {
                throw new Exception("Não é possível gerar um RDO posterior a data final da obra.");
            }

            //if (ExisteRdoPendente(param))
            //{
            //    DateTime data = Convert.ToDateTime(param.dataRdo.ToString());
            //    data = data.AddDays(-1);
            //    string dataRetorno = data.ToString().Substring(0, 10);
            //    throw new Exception("O RDO não pôde ser gerado pois existe um RDO pendente para o dia: " + dataRetorno);
            //    //throw new Exception("Não existem lançamentos para o dia" + dataRetorno + ". Deseja gerar o RDO para esta data com os lançamentos do dia anterior?");
            //
            //}

            rdo _rdo = context.rdo.Where(x => x.rdo_dt_rdo == dataRdo && x.rdo_id_obra == idObra).FirstOrDefault() ?? new rdo();

            if (_rdo.rdo_id_rdo > 0 && _rdo.status_rdo != null && (_rdo.status_rdo.str_ds_status.ToLower().Contains("assinado")))
            {
                throw new Exception("Não é possível sobrescrever um RDO que já foi assinado.");
            }

            // WF 10/07/2020 - Lúcio solicitou que retirasse esta validação
            // if (!(bool)param.existeTarefasComHistoricos && !(bool)param.existeTarefasSemHistoricos)
            // {
            //     throw new Exception("Não existem registros para serem gerados no relatório.");
            // }

            _rdo.rdo_dt_rdo = Convert.ToDateTime(dataRdo);
            _rdo.rdo_id_status = 1;

            //todo: rdo_ds_comentario é rdo_ds_comentario_geracao mesmo?
            _rdo.rdo_ds_comentario_geracao = param.comentario;
            _rdo.rdo_tp_comentario_geracao = param.tipoComentarioGeracao == 0 ? null : param.tipoComentarioGeracao == 1 ? "P" : "N"; // Like/Deslike (P/N)
            _rdo.rdo_ds_clima_manha = param.climaManhaCheckValue;
            _rdo.rdo_ds_clima_tarde = param.climaTardeCheckValue;
            _rdo.rdo_ds_clima_noite = param.climaNoiteCheckValue;

            _rdo.rdo_ds_chuva_manha = param.chuvaManhaCheckValue;
            _rdo.rdo_ds_chuva_tarde = param.chuvaTardeCheckValue;
            _rdo.rdo_ds_chuva_noite = param.chuvaNoiteCheckValue;
            _rdo.rdo_id_obra = idObra;
            _rdo.rdo_dt_geracao = DateTime.Now;

            var improdutividade = new improdutividade();
            improdutividade.imp_st_clima = param.improdutividadeCondicoesClimaticas ?? false;
            improdutividade.imp_st_material = param.improdutividadeMateriais ?? false;
            improdutividade.imp_st_paralizacao = param.improdutividadeParalizacoes ?? false;
            improdutividade.imp_st_equipamento = param.improdutividadeEquipamentos ?? false;
            improdutividade.imp_st_contratante = param.improdutividadeContratante ?? false;
            improdutividade.imp_st_fornecedores = param.improdutividadeFornecedores ?? false;
            improdutividade.imp_st_maodeobra = param.improdutividadeMaodeObra ?? false;
            improdutividade.imp_st_projeto = param.improdutividadeProjeto ?? false;
            improdutividade.imp_st_planejamento = param.improdutividadePlanejamento ?? false;
            improdutividade.imp_st_acidentes = param.improdutividadeAcidente ?? false;

            context.improdutividade.Add(improdutividade);
            context.SaveChanges();

            _rdo.rdo_id_improdutividade = improdutividade.imp_id_improdutividade;
            if (context.rdo.ToList().Any(x => x.rdo_id_obra == idObra && x.rdo_dt_rdo == _rdo.rdo_dt_rdo))
            {
                context.rdo.Attach(_rdo);
                context.Entry(_rdo).State = EntityState.Modified;
            }
            else
            {
                _rdo.rdo_id_colaborador = idColaborador; //Só adicionar o colaborador no cadastro do rdo, na edição permanecer o usuário que criou.
                context.rdo.Add(_rdo);
            }

            bool result = context.SaveChanges() > 0;
            SalvarHistoricoTarefa(context, param, _rdo);
            if (param.listaImagems != null)
            {
                IncluirImagens(param.listaImagems, _rdo.rdo_id_rdo);
            }

            return PreencherViewModel(_rdo, param.listaImagems != null);
        }

        //private static void IncluirImagens(dynamic listaImagems, int idRDO)
        //{
        //    LaudosPiscinasEntities rdoContext = new LaudosPiscinasEntities();
        //    List<rdo_imagem> rdoImagens = rdoContext.rdo_imagem.Where(ri => ri.rim_id_rdo == idRDO).ToList();



        //    foreach (var item in listaImagems)
        //    {
        //        int _idImagem = item.idImagem;
        //        var rDOImagem = new rdo_imagem();
        //        rDOImagem.rim_id_rdo = idRDO;
        //        rDOImagem.rim_id_imagem = _idImagem;
        //        if (rdoImagens.Where(ri => ri.rim_id_imagem == _idImagem).Count() == 0)
        //        {
        //            using (var context = new LaudosPiscinasEntities())
        //            {
        //                context.rdo_imagem.Add(rDOImagem);

        //                // atualiza imagem para que ela não possa ser
        //                // excluída do sistema
        //                var imagem = context.imagem.Find(_idImagem);
        //                imagem.ima_id_historico_tarefa_rdo = idRDO;
        //                context.Entry(imagem).State = EntityState.Modified;
        //                context.SaveChanges();
        //            }
        //        }
        //        else
        //        {
        //            rdoImagens.Remove(rDOImagem);
        //        }
        //    }
        //    foreach (rdo_imagem item in rdoImagens)
        //    {
        //        rdoContext.rdo_imagem.Remove(item);
        //        rdoContext.SaveChanges();
        //    }
        //}

        private static void IncluirImagens(dynamic listaImagems, int idRDO)
        {
            LaudosPiscinasEntities rdoContext = new LaudosPiscinasEntities();
            List<rdo_imagem> rdoImagens = rdoContext.rdo_imagem.Where(ri => ri.rim_id_rdo == idRDO).ToList();

            if (rdoImagens.Count > 0)
            {
                foreach (var item in rdoImagens)
                {
                    rdoContext.rdo_imagem.Remove(item);

                    imagem i = rdoContext.imagem.Find(item.rim_id_imagem);
                    i.ima_id_historico_tarefa_rdo = null;
                    //rdoContext.imagem.Add(i);
                    rdoContext.Entry(i).State = EntityState.Modified;

                    rdoContext.SaveChanges();
                }
            }

            foreach (var item in listaImagems)
            {
                int _idImagem = item.idImagem;
                var rDOImagem = new rdo_imagem();
                rDOImagem.rim_id_rdo = idRDO;
                rDOImagem.rim_id_imagem = _idImagem;
                using (var context = new LaudosPiscinasEntities())
                {
                    context.rdo_imagem.Add(rDOImagem);

                    // atualiza imagem para que ela não possa ser
                    // excluída do sistema
                    var imagem = context.imagem.Find(_idImagem);
                    imagem.ima_id_historico_tarefa_rdo = idRDO;
                    context.Entry(imagem).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }

        public static RdoViewModel PreencherViewModel(rdo _rdo, bool relatorioFotografico)
        {
            RdoViewModel returnObj = new RdoViewModel();
            returnObj.IdRdo = _rdo.rdo_id_rdo;
            returnObj.IdObra = _rdo.rdo_id_obra;
            returnObj.gerarRelatorioFotografico = relatorioFotografico;
            returnObj.DataRdo = _rdo.rdo_dt_rdo.Date.ToString().Substring(0, 10);
            return returnObj;
        }
        public static byte[] GerarDocumentoRdo(int idRdo, bool gerarRelatorioFotografico = false)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            rdo _rdo = context.rdo.FirstOrDefault(x => x.rdo_id_rdo == idRdo);
            if (_rdo.rdo_imagem.Count() > 0)
            {
                gerarRelatorioFotografico = true;
            }
            // List<rdo_tarefa> listaHistoricoTarefas = _rdo.rdo_tarefa.ToList();

            List<equipamento> listaEquipamentos = new List<equipamento>();
            List<tarefa> listaTarefas = new List<tarefa>();
            List<cargo> listaCargos = new List<cargo>();
            List<obra_tarefa_colaborador> listaTarefasColaboradores = new List<obra_tarefa_colaborador>();
            List<acidente> listaAcidentesAux = context.acidente.ToList() ?? new List<acidente>();
            List<acidente> listaAcidentes = new List<acidente>();

            foreach (var item in _rdo.obra.etapa)
            {
                foreach (tarefa tar in item.tarefa)
                {
                    foreach (acidente aci in listaAcidentesAux)
                    {
                        DateTime dt = aci.aci_dt_data_hora ?? DateTime.MinValue;
                        if (dt.ToString("dd/MM/yyyy") == _rdo.rdo_dt_rdo.ToString("dd/MM/yyyy"))
                        {
                            if (tar.tar_id_tarefa == aci.aci_id_tarefa)
                            {
                                listaAcidentes.Add(aci);
                            }
                        }
                    }
                }
            }

            foreach (rdo_tarefa tarefa in _rdo.rdo_tarefa)
            {
                tarefa.tarefa.obra_tarefa_equipamento.ToList().ForEach(x => {
                    var equipamento = ObterEquipamento(x.ote_id_obra_equipamento);
                    if (listaEquipamentos.Count(c => c.equ_id_equipamento == equipamento.equ_id_equipamento) == 0) {
                        listaEquipamentos.Add(equipamento);
                    }
                });


                listaTarefas.Add(tarefa.tarefa);

                foreach (obra_tarefa_colaborador item in tarefa.tarefa.obra_tarefa_colaborador)
                {
                    if (listaTarefasColaboradores.Where(tc => tc.obra_colaborador.colaborador.col_nm_colaborador == item.obra_colaborador.colaborador.col_nm_colaborador && tc.obra_colaborador.oco_id_cargo == item.obra_colaborador.oco_id_cargo).Count() == 0)
                    {
                        cargo cargo = ObterObraColaborador(item.otc_id_obra_colaborador).cargo;
                        listaTarefasColaboradores.Add(item);
                    }

                    //listaCargos.Add(cargo);

                }
            }

            listaTarefasColaboradores = listaTarefasColaboradores.Distinct().ToList();

            foreach (var item in listaTarefasColaboradores)
            {
                var cargo = ObterObraColaborador(item.otc_id_obra_colaborador).cargo;
                if (item.obra_colaborador.grupo.gru_nm_nome.Equals("Terceirizado"))
                    cargo.car_ds_cargo = $"{cargo.car_ds_cargo} (Terceirizado)";

                listaCargos.Add(cargo);
            }



            var cargosAgrupados = listaCargos.GroupBy(c => c.car_ds_cargo).
                     Select(group =>
                         new
                         {
                             DescricaoCargo = group.Key,
                             QuantidadeCargo = group.Count()
                         });

            listaEquipamentos = listaEquipamentos.Distinct().ToList();

            var equipamentosAgrupados = listaEquipamentos.GroupBy(e => e.equ_id_tipo_equipamento).
                     Select(group =>
                         new
                         {
                             DescricaoEquipamento = group.FirstOrDefault().tipo_equipamento.teq_nm_tipo_equipamento,
                             QuantidadeEquipamento = listaEquipamentos.Where(eq => eq.equ_id_tipo_equipamento == group.FirstOrDefault().tipo_equipamento.teq_id_tipo_equipamento).Count()
                         });


            DataTable dtCargosAgrupados = new DataTable();
            dtCargosAgrupados.Columns.Add("DescricaoCargo");
            dtCargosAgrupados.Columns.Add("QuantidadeCargo");

            cargosAgrupados = cargosAgrupados.OrderBy(o => o.DescricaoCargo);

            foreach (var item in cargosAgrupados)
            {
                dtCargosAgrupados.Rows.Add(item.DescricaoCargo, item.QuantidadeCargo);
            }


            DataTable dtEquipamentosAgrupados = new DataTable();
            dtEquipamentosAgrupados.Columns.Add("DescricaoEquipamento");
            dtEquipamentosAgrupados.Columns.Add("QuantidadeEquipamento");

            equipamentosAgrupados = equipamentosAgrupados.OrderBy(o => o.DescricaoEquipamento);
            foreach (var item in equipamentosAgrupados)
            {
                dtEquipamentosAgrupados.Rows.Add(item.DescricaoEquipamento, item.QuantidadeEquipamento);
            }


            DataTable dtTarefas = new DataTable();
            dtTarefas.Columns.Add("tar_ds_tarefa");
            dtTarefas.Columns.Add("tar_ds_comentario");
            dtTarefas.Columns.Add("tar_ds_status");
            dtTarefas.Columns.Add("tar_ds_etapa");


            foreach (var item in listaTarefas.OrderBy(o => o.status_tarefa.stt_id_status))
            {
                dtTarefas.Rows.Add(item.tar_ds_tarefa, item.tar_ds_comentario, item.status_tarefa.stt_ds_status, item.etapa.eta_ds_etapa);
            }


            DataTable dtClima = new DataTable();
            dtClima.Columns.Add("ClimaManha");
            dtClima.Columns.Add("ClimaTarde");
            dtClima.Columns.Add("ClimaNoite");
            dtClima.Columns.Add("ChuvaManha");
            dtClima.Columns.Add("ChuvaTarde");
            dtClima.Columns.Add("ChuvaNoite");



            dtClima.Rows.Add(_rdo.rdo_ds_clima_manha == "b" ? "Bom" : (_rdo.rdo_ds_clima_manha == "r" ? "Ruim" : "Não Informado"),
                                            _rdo.rdo_ds_clima_tarde == "b" ? "Bom" : (_rdo.rdo_ds_clima_tarde == "r" ? "Ruim" : "Não Informado"),
                                            _rdo.rdo_ds_clima_noite == "b" ? "Bom" : (_rdo.rdo_ds_clima_noite == "r" ? "Ruim" : "Não Informado"),
                                            _rdo.rdo_ds_chuva_manha == "m" ? "Muita Chuva" : (_rdo.rdo_ds_chuva_manha == "p" ? "Pouca Chuva" : (_rdo.rdo_ds_chuva_manha == "s" ? "Sem Chuva" : "Não Informado")),
                                            _rdo.rdo_ds_chuva_tarde == "m" ? "Muita Chuva" : (_rdo.rdo_ds_chuva_tarde == "p" ? "Pouca Chuva" : (_rdo.rdo_ds_chuva_tarde == "s" ? "Sem Chuva" : "Não Informado")),
                                            _rdo.rdo_ds_chuva_noite == "m" ? "Muita Chuva" : (_rdo.rdo_ds_chuva_noite == "p" ? "Pouca Chuva" : (_rdo.rdo_ds_chuva_noite == "s" ? "Sem Chuva" : "Não Informado")));



            DataTable dtAcidentes = new DataTable();
            dtAcidentes.Columns.Add("aci_ds_acidente");
            dtAcidentes.Columns.Add("aci_st_afastamento");


            DataTable dtAssinaturaContratante = new DataTable();
            dtAssinaturaContratante.Columns.Add("responsavel_assinatura");
            dtAssinaturaContratante.Columns.Add("data_hora");
            dtAssinaturaContratante.Columns.Add("cargo");
            dtAssinaturaContratante.Columns.Add("ip");


            //_rdo
            assinatura_rdo assinaturaContratante = context.assinatura_rdo.FirstOrDefault(x => x.ass_id_rdo == _rdo.rdo_id_rdo && x.obra_colaborador.grupo.gru_nm_nome.ToLower().Contains("contratante")) ?? new assinatura_rdo();
            if (assinaturaContratante.ass_id_obra_colaborador_assinante > 0)
            {
                dtAssinaturaContratante.Rows.Add(assinaturaContratante.obra_colaborador.colaborador.col_nm_colaborador, assinaturaContratante.ass_dt_assinatura, assinaturaContratante.obra_colaborador.cargo.car_ds_cargo, assinaturaContratante.ass_ds_ip);
            }
            else
            {
                dtAssinaturaContratante.Rows.Add("Não assinado", "Não assinado", "Não assinado", "Não assinado");
            }




            DataTable dtAssinaturaContratada = new DataTable();
            dtAssinaturaContratada.Columns.Add("responsavel_assinatura");
            dtAssinaturaContratada.Columns.Add("data_hora");
            dtAssinaturaContratada.Columns.Add("cargo");
            dtAssinaturaContratada.Columns.Add("ip");


            //_rdo
            assinatura_rdo assinaturaContratada = context.assinatura_rdo.FirstOrDefault(x => x.ass_id_rdo == _rdo.rdo_id_rdo && x.obra_colaborador.grupo.gru_nm_nome.ToLower().Contains("contratada")) ?? new assinatura_rdo();
            if (assinaturaContratada.ass_id_obra_colaborador_assinante > 0)
            {
                dtAssinaturaContratada.Rows.Add(assinaturaContratada.obra_colaborador.colaborador.col_nm_colaborador, assinaturaContratada.ass_dt_assinatura, assinaturaContratada.obra_colaborador.cargo.car_ds_cargo, assinaturaContratada.ass_ds_ip);
            }
            else
            {
                dtAssinaturaContratada.Rows.Add("Não assinado", "Não assinado", "Não assinado", "Não assinado");
            }

            foreach (var item in listaAcidentes)
            {
                dtAcidentes.Rows.Add(item.aci_ds_acidente, item.aci_st_afastamento == "s" ? "Sim" : (item.aci_st_afastamento == "n" ? "Não" : "Não Informado"));
            }

            var dtImagem = new DataTable();
            dtImagem.Columns.Add("tarefa");
            dtImagem.Columns.Add("imagem", typeof(byte[]));
            dtImagem.Columns.Add("idImagem");
            dtImagem.Columns.Add("tarefa1");
            dtImagem.Columns.Add("imagem1", typeof(byte[]));
            dtImagem.Columns.Add("idImagem1");
            dtImagem.Columns.Add("tarefa2");
            dtImagem.Columns.Add("imagem2", typeof(byte[]));
            dtImagem.Columns.Add("idImagem2");
            dtImagem.Columns.Add("tarefa3");
            dtImagem.Columns.Add("imagem3", typeof(byte[]));
            dtImagem.Columns.Add("idImagem3");



            //rdo_imagem linha = new rdo_imagem();
            int count = 0;
            //foreach (var item in _rdo.rdo_imagem)
            //{
            //    if (count %2 == 1)
            //    {
            //        dtImagem.Rows.Add(item.imagem.tarefa.etapa.eta_ds_etapa + " / " + item.imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + item.imagem.ima_ds_caminho), item.rim_id_imagem, linha.imagem.tarefa.etapa.eta_ds_etapa + " / " + linha.imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linha.imagem.ima_ds_caminho), linha.rim_id_imagem);
            //        linha = null;
            //    }
            //    else
            //    {
            //        linha = item;
            //    }

            //    if (_rdo.rdo_imagem.Count == (count - 1))
            //    {
            //        dtImagem.Rows.Add(item.imagem.tarefa.etapa.eta_ds_etapa + " / " + item.imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + item.imagem.ima_ds_caminho), item.rim_id_imagem);

            //    }
            //    count++;
            //}
            try
            {
                List<rdo_imagem> linhaLista = _rdo.rdo_imagem.OrderBy(rdi => rdi.imagem.tarefa.etapa.eta_nr_orderm).ToList();
                string nomeTarefa = "";

                while (count < linhaLista.Count())
                {
                    int dif = linhaLista.Count() - count;
                    nomeTarefa = linhaLista[count].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count].imagem.tarefa.tar_ds_tarefa;
                    if (dif >= 4 && nomeTarefa == linhaLista[count].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count].imagem.tarefa.tar_ds_tarefa && nomeTarefa == linhaLista[count + 1].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 1].imagem.tarefa.tar_ds_tarefa && nomeTarefa == linhaLista[count + 2].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 2].imagem.tarefa.tar_ds_tarefa && nomeTarefa == linhaLista[count + 3].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 3].imagem.tarefa.tar_ds_tarefa)
                    {
                        dif = 4;
                    }

                    else if (dif >= 3 && nomeTarefa == linhaLista[count].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count].imagem.tarefa.tar_ds_tarefa && nomeTarefa == linhaLista[count + 1].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 1].imagem.tarefa.tar_ds_tarefa && nomeTarefa == linhaLista[count + 2].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 2].imagem.tarefa.tar_ds_tarefa)
                    {
                        dif = 3;
                    }

                    else if (dif >= 2 && nomeTarefa == linhaLista[count].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count].imagem.tarefa.tar_ds_tarefa && nomeTarefa == linhaLista[count + 1].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 1].imagem.tarefa.tar_ds_tarefa)
                    {
                        dif = 2;
                    }

                    else if (dif >= 1 && nomeTarefa == linhaLista[count].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count].imagem.tarefa.tar_ds_tarefa)
                    {
                        dif = 1;
                    }

                    switch (dif)
                    {
                        case 3:
                            dtImagem.Rows.Add(nomeTarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count].imagem.ima_ds_caminho), linhaLista[count].rim_id_imagem,
                           linhaLista[count + 1].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 1].imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count + 1].imagem.ima_ds_caminho), linhaLista[count + 1].rim_id_imagem,
                           linhaLista[count + 2].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 2].imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count + 2].imagem.ima_ds_caminho), linhaLista[count + 2].rim_id_imagem);
                            count += 3;
                            break;
                        case 2:
                            dtImagem.Rows.Add(nomeTarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count].imagem.ima_ds_caminho), linhaLista[count].rim_id_imagem,
                            linhaLista[count + 1].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 1].imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count + 1].imagem.ima_ds_caminho), linhaLista[count + 1].rim_id_imagem);
                            count += 2;
                            break;
                        case 1:
                            dtImagem.Rows.Add(nomeTarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count].imagem.ima_ds_caminho), linhaLista[count].rim_id_imagem);
                            count += 1;
                            break;
                        default:
                            dtImagem.Rows.Add(nomeTarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count].imagem.ima_ds_caminho), linhaLista[count].rim_id_imagem,
                            linhaLista[count + 1].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 1].imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count + 1].imagem.ima_ds_caminho), linhaLista[count + 1].rim_id_imagem,
                            linhaLista[count + 2].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 2].imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count + 2].imagem.ima_ds_caminho), linhaLista[count + 2].rim_id_imagem,
                            linhaLista[count + 3].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 3].imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count + 3].imagem.ima_ds_caminho), linhaLista[count + 3].rim_id_imagem);
                            count += 4;
                            break;
                    }
                }
            }
            catch (Exception ex) {
                
            }           

            dtImagem.AcceptChanges();
            //for (int i = 0; i < length; i++)
            //{
            //    convertToBytes();
            //}



            return GenerateReport(dtCargosAgrupados, dtEquipamentosAgrupados, _rdo, dtTarefas, dtClima, dtAcidentes, dtAssinaturaContratante, dtAssinaturaContratada, dtImagem, gerarRelatorioFotografico);
        }
        public static byte[] GenerateReport(DataTable dtCargosAgrupados, DataTable dtEquipamentosAgrupados, rdo rdo, DataTable tarefas, DataTable dtClima, DataTable dtAcidentes, DataTable dtAssinaturaContratante, DataTable dtAssinaturaContratada, DataTable dtImagem, bool gerarRelatorioFotografico)
        {
            DataTable dtDadosRdo = new DataTable();

            string mappath = System.Web.HttpContext.Current.Server.MapPath("~/Api/Contents/Reports/Rdo_def.rdlc");
            LocalReport ReportViewer = new LocalReport();
            ReportViewer.ReportPath = mappath;
            ReportViewer.EnableExternalImages = true; // para permitir adicionar a imagem da logomarca dinamicamente
            string enderecoObra = rdo.obra.obr_ds_logradouro;
            string municipioObra = rdo.obra.municipio.mun_ds_municipio;
            bool licencaLiberaLogo = rdo.obra.empresa.licenca.lic_st_permite_logo_rdo;
            string logoContratada = rdo.obra.obr_ds_foto;

            if (!String.IsNullOrEmpty(rdo.obra.obr_ds_numero))
            {
                enderecoObra = enderecoObra + ", " + rdo.obra.obr_ds_numero;
            }

            if (!String.IsNullOrEmpty(rdo.obra.obr_ds_complemento))
            {
                enderecoObra = enderecoObra + ", " + rdo.obra.obr_ds_complemento;
            }

            if (!String.IsNullOrEmpty(rdo.obra.obr_ds_bairro))
            {
                enderecoObra = enderecoObra + ", " + rdo.obra.obr_ds_bairro;
            }

            enderecoObra = enderecoObra + ", " + municipioObra + " - " + rdo.obra.municipio.uf.ufe_ds_sigla;

            if (!String.IsNullOrEmpty(rdo.obra.obr_ds_cep))
            {
                enderecoObra = enderecoObra + ", CEP: " + Convert.ToUInt64(rdo.obra.obr_ds_cep).ToString(@"00000-000");
            }

            enderecoObra = enderecoObra + ".";

            //else
            //{
            //    enderecoObra = enderecoObra + " .";
            //}


            //int diasRestantes = (Convert.ToDateTime(rdo.obra.obr_dt_previsao_fim).Date - DateTime.Now.Date).Days;

            int diasRestantes = (Convert.ToDateTime(rdo.obra.obr_dt_previsao_fim).Date - (rdo.rdo_dt_rdo).Date).Days;

            if (diasRestantes < 0)
            {
                diasRestantes = 0;
            }

            string diaDaSemana = cultureInfo.DateTimeFormat.GetDayName(rdo.rdo_dt_rdo.DayOfWeek);

            ReportViewer.DataSources.Add(new ReportDataSource("dtCargosAgrupados", dtCargosAgrupados));
            ReportViewer.DataSources.Add(new ReportDataSource("dtEquipamentosAgrupados", dtEquipamentosAgrupados));
            ReportViewer.DataSources.Add(new ReportDataSource("tarefas", tarefas));
            ReportViewer.DataSources.Add(new ReportDataSource("dtCargosAgrupados", dtCargosAgrupados));
            ReportViewer.DataSources.Add(new ReportDataSource("dtClima", dtClima));
            ReportViewer.DataSources.Add(new ReportDataSource("dtDadosRdo", dtDadosRdo));
            ReportViewer.DataSources.Add(new ReportDataSource("dtAcidentes", dtAcidentes));
            ReportViewer.DataSources.Add(new ReportDataSource("dtAssinaturaContratante", dtAssinaturaContratante));
            ReportViewer.DataSources.Add(new ReportDataSource("dtAssinaturaContratada", dtAssinaturaContratada));
            ReportViewer.DataSources.Add(new ReportDataSource("dtImagem", dtImagem));

            ReportViewer.SetParameters(new ReportParameter("NomeObra", rdo.obra.obr_ds_obra));
            ReportViewer.SetParameters(new ReportParameter("StatusRdo", rdo.status_rdo.str_ds_status));
            ReportViewer.SetParameters(new ReportParameter("DataRdo", rdo.rdo_dt_rdo.ToString("dd/MM/yyyy")));
            ReportViewer.SetParameters(new ReportParameter("DataRdoDiaSemana", $"{diaDaSemana.ToUpper()}"));
            ReportViewer.SetParameters(new ReportParameter("DataInicioObra", rdo.obra.obr_dt_inicio.ToString("dd/MM/yyyy")));
            ReportViewer.SetParameters(new ReportParameter("DiasDecorridosObra", (rdo.rdo_dt_rdo.AddDays(1) - rdo.obra.obr_dt_inicio.Date).Days.ToString()));
            //ReportViewer.SetParameters(new ReportParameter("TipoContratanteContrada", rdo.rdo_id_status == 2 ? "COMENTÁRIO CONTRATANTE" : rdo.rdo_id_status == 3 ? "COMENTÁRIO CONTRATADA" : ""));
            ReportViewer.SetParameters(new ReportParameter("ComentarioRdo", rdo.colaborador.obra_colaborador.FirstOrDefault(oc => oc.oco_id_obra == rdo.rdo_id_obra).oco_st_contratante_contratada == "d" ? rdo.rdo_ds_comentario_geracao : rdo.rdo_ds_comentario_assinatura)); //Obter comentário contratada
            ReportViewer.SetParameters(new ReportParameter("ComentarioAssinaturaRdo", rdo.colaborador.obra_colaborador.FirstOrDefault(oc => oc.oco_id_obra == rdo.rdo_id_obra).oco_st_contratante_contratada == "t" ? rdo.rdo_ds_comentario_geracao : rdo.rdo_ds_comentario_assinatura)); //Obter comentario contratante
            ReportViewer.SetParameters(new ReportParameter("EnderecoObra", enderecoObra));
            ReportViewer.SetParameters(new ReportParameter("MunicipioObra", municipioObra + " - " + rdo.obra.municipio.uf.ufe_ds_sigla));
            ReportViewer.SetParameters(new ReportParameter("PrevisaoFinalObra", Convert.ToDateTime(rdo.obra.obr_dt_previsao_fim).ToString("dd/MM/yyyy")));
            ReportViewer.SetParameters(new ReportParameter("DiasRestantes", diasRestantes.ToString()));
            ReportViewer.SetParameters(new ReportParameter("HabilitarRelatorioFotografico", Convert.ToString(gerarRelatorioFotografico)));
            ReportViewer.SetParameters(new ReportParameter("AssinaturaContratante", dtAssinaturaContratante.Rows[0].ItemArray[0].ToString().Equals("Não assinado") ? "" : "Assinado")); 
            ReportViewer.SetParameters(new ReportParameter("AssinaturaContratada", dtAssinaturaContratada.Rows[0].ItemArray[0].ToString().Equals("Não assinado") ? "" : "Assinado")); 

            string basePath = System.Configuration.ConfigurationManager.AppSettings["basePath"];
            basePath = basePath.Remove(basePath.Length - 1);

            if (!licencaLiberaLogo || string.IsNullOrEmpty(logoContratada))
            {
                logoContratada = "/Assets/images/logo.jpg";
                ReportViewer.SetParameters(new ReportParameter("logoContratada", new Uri(HttpContext.Current.Server.MapPath(basePath + logoContratada)).AbsoluteUri)); //adiciona logomarca 
            }
            else
            {
                ReportViewer.SetParameters(new ReportParameter("logoContratada", new Uri(HttpContext.Current.Server.MapPath(basePath + logoContratada)).AbsoluteUri)); //adiciona logomarca 
            }

            byte[] bytes = ReportViewer.Render("Pdf");

            return bytes;
        }
        public static byte[] convertToBytes(string path)
        {
            if (File.Exists(path))
            {
                var fs = new FileStream(path, FileMode.Open);

                if (fs != null)
                {
                    var br = new BinaryReader(fs);
                    byte[] imgbyteFotoFamilia = new byte[fs.Length + 1];
                    imgbyteFotoFamilia = br.ReadBytes((int)fs.Length);
                    br.Close();
                    fs.Close();

                    return imgbyteFotoFamilia;
                }
            }
            return new Byte[] { };
        }

        public static equipamento ObterEquipamento(int idObraEquipamento)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            return context.obra_equipamento.FirstOrDefault(x => x.oeq_id_obra_equipamento == idObraEquipamento).equipamento;
        }
        public static obra_colaborador ObterObraColaborador(int idObraColaborador)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            return context.obra_colaborador.FirstOrDefault(x => x.oco_id_obra_colaborador == idObraColaborador);
        }
        public static bool removerRegistrosHistoricoTarefa(rdo rdo)
        {
            List<rdo_tarefa> list = rdo.rdo_tarefa.ToList();
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            foreach (rdo_tarefa item in list)
            {
                //todo: ver porque remover registros do historico tarefa
                //context.historico_tarefa_colaborador.Where(x => x.htc_id_historico_tarefa_rdo == item.his_id_historico_tarefa_rdo).ToList().ForEach(y => context.historico_tarefa_colaborador.Remove(y));
                //context.historico_tarefa_equipamento.Where(x => x.hte_id_historico_tarefa_rdo == item.his_id_historico_tarefa_rdo).ToList().ForEach(y => context.historico_tarefa_equipamento.Remove(y));
                //context.historico_tarefa_rdo.Where(x => x.his_id_historico_tarefa_rdo == item.his_id_historico_tarefa_rdo).ToList().ForEach(y => context.historico_tarefa_rdo.Remove(y));
            }



            return context.SaveChanges() > 0;
        }
        public static bool SalvarHistoricoTarefa(LaudosPiscinasEntities context, dynamic param, rdo rdo)
        {
            bool result = true;
            List<tarefa> tarefas = context.tarefa.Where(ta => ta.etapa.eta_id_obra == rdo.rdo_id_obra).ToList();
            List<rdo_tarefa> rdoTarefas = context.rdo_tarefa.Where(rt => rt.rta_id_rdo == rdo.rdo_id_rdo).ToList();
            rdo_tarefa htr = new rdo_tarefa();

            foreach (var item in param.listaTarefas)
            {
                int idTarefa = item.id;
                Guid agrupador = new Guid(tarefas.FirstOrDefault(ta => ta.tar_id_tarefa == idTarefa).tar_nr_agrupador);
                List<tarefa> tarefasHistoricosNesteDia = tarefas.Where(ta => ta.tar_id_status != 1 && new Guid(ta.tar_nr_agrupador) == agrupador && ta.tar_dt_medicao == rdo.rdo_dt_rdo).ToList();
                if (tarefasHistoricosNesteDia.Count == 0)
                {
                    if (tarefas.FirstOrDefault(ta => new Guid(ta.tar_nr_agrupador) == agrupador && ta.tar_dt_medicao == rdo.rdo_dt_rdo.AddDays(-1)) != null)
                    {
                        tarefasHistoricosNesteDia = tarefas.Where(ta => ta.tar_id_status != 1 && new Guid(ta.tar_nr_agrupador) == agrupador && ta.tar_dt_medicao == rdo.rdo_dt_rdo.AddDays(-1)).ToList();
                        foreach (tarefa tar in tarefasHistoricosNesteDia)
                        {
                            idTarefa = TarefaModel.SalvarNovoHistorico(tar); //Salvar novo histórico a partir de um existente e mudar para o ID correspodente ao novo histórico.
                            if (rdoTarefas.Where(rt => rt.rta_id_tarefa == idTarefa).Count() == 0)
                            {
                                htr.rta_id_rdo = rdo.rdo_id_rdo;
                                htr.rta_id_tarefa = idTarefa;

                                context.rdo_tarefa.Add(htr);

                                context.SaveChanges();
                            }
                            else
                            {
                                rdoTarefas.Remove(rdoTarefas.FirstOrDefault(rt => rt.rta_id_tarefa == idTarefa));
                            }
                        }
                    }
                }
                else
                {
                    foreach (tarefa tar in tarefasHistoricosNesteDia)
                    {
                        if (rdoTarefas.Where(rt => rt.rta_id_tarefa == idTarefa).Count() == 0)
                        {
                            htr.rta_id_rdo = rdo.rdo_id_rdo;
                            htr.rta_id_tarefa = tar.tar_id_tarefa;

                            context.rdo_tarefa.Add(htr);

                            context.SaveChanges();
                        }
                        else
                        {
                            rdoTarefas.Remove(rdoTarefas.FirstOrDefault(rt => rt.rta_id_tarefa == idTarefa));
                        }
                    }
                }
            }
            foreach (rdo_tarefa item in rdoTarefas)
            {
                context.rdo_tarefa.Remove(item);
                context.SaveChanges();
            }
            return result;
        }

        public static bool SalvarHistoricoColaboradores(rdo_tarefa htr, LaudosPiscinasEntities context, int idTarefa)
        {
            bool result = true;
            tarefa objTarefa = context.tarefa.FirstOrDefault(x => x.tar_id_tarefa == idTarefa);
            List<obra_tarefa_colaborador> listaObraTarefaColaborador = objTarefa.obra_tarefa_colaborador.ToList();

            foreach (obra_tarefa_colaborador otc in listaObraTarefaColaborador)
            {
                //historico_tarefa_colaborador objHistoricoTarefaColaborador = new historico_tarefa_colaborador();
                //objHistoricoTarefaColaborador = context.historico_tarefa_colaborador.FirstOrDefault(
                //    x => x.htc_id_historico_tarefa_rdo == htr.his_id_historico_tarefa_rdo &&
                //    x.htc_id_obra_colaborador == otc.otc_id_obra_colaborador) ?? new historico_tarefa_colaborador();

                //objHistoricoTarefaColaborador.htc_id_historico_tarefa_rdo = htr.his_id_historico_tarefa_rdo;
                //objHistoricoTarefaColaborador.htc_id_obra_colaborador = otc.otc_id_obra_colaborador;

                //if (objHistoricoTarefaColaborador.htc_id_tarefa_colaborador > 0)
                //{
                //    context.historico_tarefa_colaborador.Attach(objHistoricoTarefaColaborador);
                //    context.Entry(objHistoricoTarefaColaborador).State = EntityState.Modified;
                //}
                //else
                //{
                //    context.historico_tarefa_colaborador.Add(objHistoricoTarefaColaborador);
                //}

                result = context.SaveChanges() > 0;
            }


            return result;
        }
        public static bool SalvarHistoricoEquipamentos(rdo_tarefa htr, LaudosPiscinasEntities context, int idTarefa)
        {
            bool result = true;
            tarefa objTarefa = context.tarefa.FirstOrDefault(x => x.tar_id_tarefa == idTarefa);
            List<obra_tarefa_equipamento> listaObraTarefaEquipamento = objTarefa.obra_tarefa_equipamento.ToList();
            foreach (obra_tarefa_equipamento ote in listaObraTarefaEquipamento)
            {
                //historico_tarefa_equipamento objHistoricoTarefaEquipamento = new historico_tarefa_equipamento();
                //objHistoricoTarefaEquipamento = context.historico_tarefa_equipamento.FirstOrDefault(
                //    x => x.hte_id_historico_tarefa_rdo == htr.his_id_historico_tarefa_rdo &&
                //    x.hte_id_obra_equipamento == ote.ote_id_obra_equipamento) ?? new historico_tarefa_equipamento();

                //objHistoricoTarefaEquipamento.hte_id_historico_tarefa_rdo = htr.his_id_historico_tarefa_rdo;
                //objHistoricoTarefaEquipamento.hte_id_obra_equipamento = ote.ote_id_obra_equipamento;

                //if (objHistoricoTarefaEquipamento.hte_id_tarefa_equipamento > 0)
                //{
                //    context.historico_tarefa_equipamento.Attach(objHistoricoTarefaEquipamento);
                //    context.Entry(objHistoricoTarefaEquipamento).State = EntityState.Modified;
                //}
                //else
                //{
                //    context.historico_tarefa_equipamento.Add(objHistoricoTarefaEquipamento);
                //}

                result = context.SaveChanges() > 0;
            }


            return result;
        }
        internal static bool Deletar(dynamic param)
        {
            int idRdo = (int)param.idRdo;
            int idObra = (int)param.idObra;
            //int idRdo = (int)param.idRdo;

            RdoViewModel rdo = new RdoViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            try
            {
                // os Rdos não serão excluidos só os relacionamentos, já deixei pronto para quando mudar.
                //context.obra_Rdo_Rdo.Where(x => x.ote_id_Rdo == idRdo).ToList().ForEach(y => context.obra_Rdo_Rdo.Remove(y));
                //context.obra_Rdo.Where(x => x.oeq_id_obra == idObra).ToList().ForEach(y => context.obra_Rdo.Remove(y));
                context.rdo.Where(x => x.rdo_id_rdo == idRdo).ToList().ForEach(y => context.rdo.Remove(y));


                context.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        internal static RdoViewModel ObterRegistro(dynamic param)
        {
            int idRdo = (int)param;
            //int idObra = 0;


            RdoViewModel rdo = new RdoViewModel();
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            rdo resultado = context.rdo.FirstOrDefault(rdoIns => rdoIns.rdo_id_rdo == idRdo);

            rdo.IdRdo = resultado.rdo_id_rdo;
            rdo.DataRdo = resultado.rdo_dt_rdo == null || resultado.rdo_dt_rdo == DateTime.MinValue ? "" : resultado.rdo_dt_rdo.Date.ToString().Substring(0, 10);
            rdo.Comentario = resultado.rdo_ds_comentario_geracao;
            rdo.StatusRdo = resultado.rdo_id_status;
            rdo.ClimaManhaCheckValue = resultado.rdo_ds_clima_manha;
            rdo.ClimaTardeCheckValue = resultado.rdo_ds_clima_tarde;
            rdo.ClimaNoiteCheckValue = resultado.rdo_ds_clima_noite;
            rdo.ChuvaManhaCheckValue = resultado.rdo_ds_chuva_manha;
            rdo.ChuvaTardeCheckValue = resultado.rdo_ds_chuva_tarde;
            rdo.ChuvaNoiteCheckValue = resultado.rdo_ds_chuva_noite;
            rdo.tipoComentarioAssinatura = resultado.rdo_tp_comentario_assinatura;
            rdo.tipoComentarioGeracao = resultado.rdo_tp_comentario_geracao == null ? "0" : resultado.rdo_tp_comentario_geracao == "P" ? "1" : "2";
            rdo.improdutividadeCondicoesClimaticas = resultado.improdutividade.imp_st_clima;
            rdo.improdutividadeMateriais = resultado.improdutividade.imp_st_material;
            rdo.improdutividadeParalizacoes = resultado.improdutividade.imp_st_paralizacao;
            rdo.improdutividadeEquipamentos = resultado.improdutividade.imp_st_equipamento;
            rdo.improdutividadeContratante = resultado.improdutividade.imp_st_contratante;
            rdo.improdutividadeFornecedores = resultado.improdutividade.imp_st_fornecedores;
            rdo.improdutividadeProjeto = resultado.improdutividade.imp_st_projeto;
            rdo.improdutividadePlanejamento = resultado.improdutividade.imp_st_planejamento;
            rdo.improdutividadeAcidente = resultado.improdutividade.imp_st_acidentes;
            rdo.improdutividadeMaodeObra = resultado.improdutividade.imp_st_maodeobra;
            rdo.statusContratanteContratadaDonoRdo = resultado.colaborador.obra_colaborador.FirstOrDefault(oc => oc.oco_id_obra == resultado.rdo_id_obra).oco_st_contratante_contratada;

            rdo.listaTarefas = new List<TarefaViewModel>();

            //var filter = new EtapaViewModel
            //{
            //    Id = 0,
            //    Titulo = "",
            //    IdObra = resultado.rdo_id_obra,
            //    descricao = "",
            //    dataInicial = DateTime.MinValue,
            //    dataFinalPlanejada = default(DateTime),
            //    dataInicialExecutada = default(DateTime),
            //    dataFinalExecutada = default(DateTime),
            //    idStatus = 0
            //};
            //rdo.listaEtapas = EtapaModel.Retrieve(filter);
            rdo.listaImagens = ImagemModel.ObterImagensRdo(idRdo);
            rdo.listaTarefas = TarefaModel.ListaTarefaRdo(idRdo);


            return rdo;
        }
        internal static bool Assinar(dynamic param)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            int idRdo = (int)param.rdo.idRdo;
            int idAssinante = (int)param.rdo.idAssinante;
            string ip = param.objIp.ip;
            rdo _rdo = context.rdo.Where(x => x.rdo_id_rdo == idRdo).FirstOrDefault() ?? new rdo();


            obra_colaborador _obra_colaborador = context.obra_colaborador.Where(x => x.oco_id_obra_colaborador == idAssinante).FirstOrDefault() ?? new obra_colaborador();
            string tipoAssinante = _obra_colaborador.oco_st_contratante_contratada;
            tipoAssinante = _obra_colaborador.grupo.gru_nm_nome.ToLower().Contains("contratante") ? "t" : (_obra_colaborador.grupo.gru_nm_nome.ToLower().Contains("contratada") ? "d" : "");


            if (_rdo.status_rdo != null && (_rdo.status_rdo.str_ds_status.ToLower().Contains("contratada") && tipoAssinante == "d"))
            {
                throw new Exception("Esse RDO já foi assinado pela contratada.");
            }
            if (_rdo.status_rdo != null && (_rdo.status_rdo.str_ds_status.ToLower().Contains("contratante") && tipoAssinante == "d"))
            {
                throw new Exception("Esse RDO já foi assinado pela contratada.");
            }
            if (_rdo.status_rdo != null && (_rdo.status_rdo.str_ds_status.ToLower().Contains("contratante") && tipoAssinante == "t"))
            {
                throw new Exception("Esse RDO já foi assinado pela contratante.");
            }


            if (tipoAssinante == "t")
            {
                _rdo.rdo_id_status = 2;



            }
            else if (tipoAssinante == "d")
            {
                _rdo.rdo_id_status = 3;

            }

            if (!String.IsNullOrEmpty(Convert.ToString(param.rdo.comentarioAssinatura)))
            {
                _rdo.rdo_ds_comentario_assinatura = param.rdo.comentarioAssinatura;
                _rdo.rdo_tp_comentario_assinatura = param.rdo.tipoComentarioAssinatura == 0 || String.IsNullOrEmpty(Convert.ToString(param.rdo.comentarioAssinatura)) ? null : param.rdo.tipoComentarioAssinatura == 1 ? "P" : "N";
            }

            if (_rdo.rdo_id_rdo > 0)
            {
                context.rdo.Attach(_rdo);
                context.Entry(_rdo).State = EntityState.Modified;
            }

            bool result = context.SaveChanges() > 0;


            assinatura_rdo _assinatura_rdo = context.assinatura_rdo.Where(x => x.ass_id_obra_colaborador_assinante == idAssinante && x.ass_id_rdo == idRdo).FirstOrDefault() ?? new assinatura_rdo();
            _assinatura_rdo.ass_id_obra_colaborador_assinante = idAssinante;
            _assinatura_rdo.ass_id_rdo = idRdo;
            _assinatura_rdo.ass_dt_assinatura = DateTime.Now;
            _assinatura_rdo.ass_ds_ip = ip;



            if (_assinatura_rdo.ass_id_assinatura > 0)
            {
                context.assinatura_rdo.Attach(_assinatura_rdo);
                context.Entry(_assinatura_rdo).State = EntityState.Modified;
            }
            else
            {
                context.assinatura_rdo.Add(_assinatura_rdo);
            }

            if (result)
            {
                result = context.SaveChanges() > 0;
            }

            return result;
        }
    }
    public class RdoViewModel
    {
        public bool? gerarRelatorioFotografico { get; set; }
        public long IdRdo { get; set; }
        public int IdObra { get; set; }
        public int QtdTarefas { get; set; }
        public int QtdMaquinas { get; set; }
        public int QtdColaboradores { get; set; }
        public string DataRdo { get; set; }
        public string DiaDaSemana { get; set; }
        public int StatusRdo { get; set; }
        public string DescricaoStatus { get; set; }
        public string Comentario { get; set; }
        public string tipoComentarioAssinatura { get; set; }
        public string tipoComentarioGeracao { get; set; }
        public string statusContratanteContratadaDonoRdo { get; set; }
        public string ComentarioAssinatura { get; set; }
        public string ClimaManhaCheckValue { get; set; }
        public string ClimaTardeCheckValue { get; set; }
        public string ClimaNoiteCheckValue { get; set; }
        public string ChuvaManhaCheckValue { get; set; }
        public string ChuvaTardeCheckValue { get; set; }
        public string ChuvaNoiteCheckValue { get; set; }
        public bool improdutividadeCondicoesClimaticas { get; set; }
        public bool improdutividadeMateriais { get; set; }
        public bool improdutividadeParalizacoes { get; set; }
        public bool improdutividadeEquipamentos { get; set; }
        public bool improdutividadeContratante { get; set; }
        public bool improdutividadeFornecedores { get; set; }
        public bool improdutividadeMaodeObra { get; set; }
        public bool improdutividadeProjeto { get; set; }
        public bool improdutividadePlanejamento { get; set; }
        public bool improdutividadeAcidente { get; set; }
        public virtual ICollection<TarefaViewModel> listaTarefas { get; set; }
        public virtual ICollection<ImagemViewModel> listaImagens { get; set; }
        public virtual ICollection<EtapaViewModel> listaEtapas { get; set; }
        public RdoViewModel()
        {
            this.listaTarefas = new HashSet<TarefaViewModel>();
        }
    }
}