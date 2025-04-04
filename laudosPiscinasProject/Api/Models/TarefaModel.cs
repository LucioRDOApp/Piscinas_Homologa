using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using static laudosPiscinasProject.Api.Models.UnidadeMedidaModel;

namespace laudosPiscinasProject.Api.Models
{
    public class TarefaModel
    {
        public static List<TarefaViewModel> Lista(dynamic param)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            IQueryable<tarefa> query = context.Set<tarefa>();

            if (param != null)
            {
                string descricao = param.descricao.ToString();
                string statusTarefa = param.statusTarefa.ToString();
                int idStatus = statusTarefa == "" ? 0 : Convert.ToInt32(statusTarefa);
                int idObra = param.idObra ?? 0;
                DateTime dataInicial = String.IsNullOrEmpty(Convert.ToString(param.dataInicial)) ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataInicial));
                DateTime dataFinalPlanejada = String.IsNullOrEmpty(Convert.ToString(param.dataFinalPlanejada)) ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataFinalPlanejada));
                DateTime dataInicialExecutada = String.IsNullOrEmpty(Convert.ToString(param.dataInicialExecutada)) ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataInicialExecutada));
                DateTime dataFinalExecutada = String.IsNullOrEmpty(Convert.ToString(param.dataFinalExecutada)) ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataFinalExecutada));
                int idEtapa = param.idEtapa ?? 0;
                //int statusTarefa = param.statusTarefa ?? 0;

                if (!string.IsNullOrEmpty(descricao))
                {
                    query = query.Where(tar => tar.tar_ds_tarefa.ToLower().Contains(descricao));
                }
                if (idStatus > 0)
                {
                    query = query.Where(tar => tar.tar_id_status == idStatus);
                }
                if (idObra > 0)
                {
                    query = query.Where(tar => tar.etapa.eta_id_obra == idObra);
                }
                if (dataInicial > DateTime.MinValue)
                {
                    query = query.Where(tar => tar.tar_dt_inicio >= dataInicial);
                }
                if (dataFinalPlanejada > DateTime.MinValue)
                {
                    query = query.Where(tar => tar.tar_dt_previsao_fim <= dataFinalPlanejada);
                }
                if (dataInicialExecutada > DateTime.MinValue)
                {
                    //todo: ver a como filtrar pela data
                    //query = query.Where(tar => tar.historico_tarefa_rdo.Min(his => his.his_dt_data) >= dataInicialExecutada);
                }
                if (dataFinalExecutada > DateTime.MinValue)
                {
                    //todo: ver a como filtrar pela data
                    //query = query.Where(tar => tar.historico_tarefa_rdo.Max(his => his.his_dt_data) <= dataFinalExecutada);
                }
                if (idEtapa > 0)
                {
                    query = query.Where(tar => tar.etapa.eta_id_etapa == idEtapa);
                }
            }

            List<TarefaViewModel> Lista = new List<TarefaViewModel>();

            List<tarefa> tarefas = query.ToList();

            tarefas.ForEach(tar =>
            {
                TarefaViewModel model = new TarefaViewModel();
                model.Descricao = tar.tar_ds_tarefa;
                model.Id = tar.tar_id_tarefa;
                model.Comentario = tar.tar_ds_comentario;
                model.DataInicio = tar.tar_dt_inicio.Date.ToString().Substring(0, 10);
                model.DataPrevisaoFim = tar.tar_dt_previsao_fim == null ? "" : tar.tar_dt_previsao_fim.ToString().Substring(0, 10);
                //model.IdObra = tar.etapa.eta_id_obra;
                model.IdEtapa = tar.tar_id_etapa;
                model.IdUnidade = tar.tar_id_unidade ?? 0;
                model.HorasTrabalhadas = tar.tar_nr_horas_trabalhadas ?? 0;
                model.QtdConstruida = tar.tar_nr_qtd_construida ?? 0;
                model.Status = tar.tar_id_status;
                model.NomeStatus = tar.status_tarefa.stt_ds_status;
                model.QuantidadeColaboradores = tar.obra_tarefa_colaborador.Count;
                model.QuantidadeEquipamentos = tar.obra_tarefa_equipamento.Count;
                model.PercentualConcluido = CalcularPercentualConcluido(tar) > 100 ? 100 : CalcularPercentualConcluido(tar);
                model.PercentualExtrapolado = (CalcularPercentualConcluido(tar) > 100) ? "false" : "true";
                model.PrimeiraExecucao = ObterPrimeiroDiaExecutado(tar);
                model.UltimaExecucao = ObterUltimoDiaExecutado(tar);
                model.ExisteExecucao = !(ObterPrimeiroDiaExecutado(tar) > DateTime.MinValue);
                model.listaHistoricoTarefa = new List<HistoricoTarefaViewModel>(); //PreencherHistoricoTarefa(tar);
                model.listaStatusPermitidos = PreencherStatusTarefaPermitidos(tar);
                model.ClasseStatusCss = tar.tar_id_status == 1 ? "bg-cinza" : (tar.tar_id_status == 2 ? "bg-azul" : (tar.tar_id_status == 3 ? "bg-verde" : (tar.tar_id_status == 4 ? "bg-laranja" : (tar.tar_id_status == 5 ? "bg-vermelho" : "bg-cinza"))));
                Lista.Add(model);
            });


            string orderby = param.orderby ?? "";
            string orderbydescending = param.orderbydescending ?? "";

            if (!string.IsNullOrEmpty(orderby))
            {
                return Lista.OrderBy(x => x.Descricao).ToList();
            }
            if (!string.IsNullOrEmpty(orderbydescending))
            {
                return Lista.OrderByDescending(x => x.Descricao).ToList();
            }

            return Lista;
        }


        public static List<StatusTarefaViewModel> PreencherStatusTarefaPermitidos(tarefa tar)
        {
            return StatusTarefaModel.ListaStatusTarefaPermitidos(tar.tar_id_status);
        }


        public static List<TarefaViewModel> ListaRdo(dynamic param)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            IQueryable<tarefa> query = context.Set<tarefa>();

            if (param != null)
            {
                string descricao = param.descricao.ToString();
                string statusTarefa = param.statusTarefa.ToString();
                int idStatus = statusTarefa == "" ? 0 : Convert.ToInt32(statusTarefa);
                int idObra = param.idObra ?? 0;
                int idEtapa = param.idEtapa ?? 0;
                DateTime dataInicial = String.IsNullOrEmpty(Convert.ToString(param.dataInicial)) ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataInicial));
                DateTime dataFinalPlanejada = String.IsNullOrEmpty(Convert.ToString(param.dataFinalPlanejada)) ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataFinalPlanejada));
                DateTime data = DateTime.Now.Date;

                query = query.Where(tar => tar.tar_id_status == 2 || (tar.tar_id_status == 3 && tar.tar_dt_ultima_atualizacao != null && tar.tar_dt_ultima_atualizacao == data) || (
                                    (tar.tar_id_status == 4 && tar.tar_dt_ultima_atualizacao != null && tar.tar_dt_ultima_atualizacao == data) ||
                                    ((tar.tar_id_status == 5 && tar.tar_dt_ultima_atualizacao != null && tar.tar_dt_ultima_atualizacao == data))));



                //int statusTarefa = param.statusTarefa ?? 0;

                if (!string.IsNullOrEmpty(descricao))
                {
                    query = query.Where(tar => tar.tar_ds_tarefa.ToLower().Contains(descricao));
                }
                if (idStatus > 0)
                {
                    query = query.Where(tar => tar.tar_id_status == idStatus);
                }
                if (idObra > 0)
                {
                    query = query.Where(tar => tar.etapa.eta_id_obra == idObra);
                }
                if (idEtapa > 0)
                {
                    query = query.Where(tar => tar.tar_id_etapa == idEtapa);
                }
                if (dataInicial > DateTime.MinValue)
                {
                    query = query.Where(tar => tar.tar_dt_inicio >= dataInicial);
                }
                if (dataFinalPlanejada > DateTime.MinValue)
                {
                    query = query.Where(tar => tar.tar_dt_previsao_fim <= dataFinalPlanejada);
                }



            }

            List<TarefaViewModel> Lista = new List<TarefaViewModel>();
            query.ToList().ForEach(tar => Lista.Add(new TarefaViewModel
            {
                Descricao = tar.tar_ds_tarefa,
                Id = tar.tar_id_tarefa,
                Comentario = tar.tar_ds_comentario,
                DataInicio = tar.tar_dt_inicio.Date.ToString().Substring(0, 10),
                DataPrevisaoFim = tar.tar_dt_previsao_fim == null ? "" : tar.tar_dt_previsao_fim.ToString().Substring(0, 10),
                //IdObra = tar.etapa.eta_id_obra,
                IdEtapa = tar.tar_id_etapa,
                IdUnidade = tar.tar_id_unidade ?? 0,
                HorasTrabalhadas = tar.tar_nr_horas_trabalhadas ?? 0,
                QtdConstruida = tar.tar_nr_qtd_construida ?? 0,
                Status = tar.tar_id_status,
                NomeStatus = tar.status_tarefa.stt_ds_status,
                QuantidadeColaboradores = tar.obra_tarefa_colaborador.Count,
                QuantidadeEquipamentos = tar.obra_tarefa_equipamento.Count,
                PercentualConcluido = CalcularPercentualConcluido(tar) > 100 ? 100 : CalcularPercentualConcluido(tar),
                PercentualExtrapolado = (CalcularPercentualConcluido(tar) > 100) ? "#d43636" : "",
                PrimeiraExecucao = ObterPrimeiroDiaExecutado(tar),
                UltimaExecucao = ObterUltimoDiaExecutado(tar),
                ExisteExecucao = !(ObterPrimeiroDiaExecutado(tar) > DateTime.MinValue),
                listaHistoricoTarefa = PreencherHistoricoTarefa(tar.tar_id_tarefa),
                ClasseStatusCss = tar.tar_id_status == 1 ? "bg-cinza" : (tar.tar_id_status == 2 ? "bg-azul" : (tar.tar_id_status == 3 ? "bg-verde" : (tar.tar_id_status == 4 ? "bg-vermelho" : "bg-cinza")))


            }));


            string orderby = param.orderby ?? "";
            string orderbydescending = param.orderbydescending ?? "";

            if (!string.IsNullOrEmpty(orderby))
            {
                return Lista.OrderBy(x => x.Descricao).ToList();
            }
            if (!string.IsNullOrEmpty(orderbydescending))
            {
                return Lista.OrderByDescending(x => x.Descricao).ToList();
            }

            return Lista;
        }


        //public static double CalcularPercentualConcluido(tarefa tar)
        //{
        //    double result = 0;
        //    double totalDiasPlanejados = (((DateTime)tar.tar_dt_previsao_fim).Date - tar.tar_dt_inicio.Date).TotalDays + 1;
        //    double diasExecutados = 0;
        //    tar.historico_tarefa_rdo.ToList().ForEach(x => diasExecutados++);
        //    result = Math.Round((diasExecutados / totalDiasPlanejados * 100), 2);
        //    return result;
        //}
        public static double CalcularPercentualConcluido(tarefa tar)
        {
            if (tar.tar_id_status == 1)
                return 0;

            double result = 0;
            double totalDiasPlanejados = tar.tar_dt_previsao_fim.Value.Day;
            using (var context = new LaudosPiscinasEntities())
            {
                if (tar.tar_dt_medicao_hora_inicial == null && tar.tar_dt_medicao_hora_final == null)
                {
                    result = 0;
                }
                else
                {
                    tarefa tarefaUltimo = context.tarefa.Where(tare => tare.tar_nr_agrupador == tar.tar_nr_agrupador).OrderByDescending(tare => tare.tar_dt_medicao).First();
                    tarefa tarefaPrimeiro = context.tarefa.Where(tare => tare.tar_nr_agrupador == tar.tar_nr_agrupador).OrderBy(tare => tare.tar_dt_medicao).First();
                    result = Math.Round(((tarefaUltimo.tar_dt_medicao.AddDays(1) - tarefaPrimeiro.tar_dt_medicao).TotalSeconds * 100) / ((Convert.ToDateTime(tar.tar_dt_previsao_fim).AddDays(1).Date - tar.tar_dt_inicio).TotalSeconds), 2);
                }
            }

            //tar.whe .ToList().ForEach(x => diasExecutados++);
            return result;
        }

        public static DateTime ObterPrimeiroDiaExecutado(tarefa tar)
        {
            DateTime result = DateTime.MinValue;
            //todo: como obter o primeiro dia executado
            //result = tar.historico_tarefa_rdo.ToList().Min(x => x.his_dt_data) ?? DateTime.MinValue;
            return result;
        }
        public static DateTime _ObterPrimeiroDiaExecutado(string agrupador)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                DateTime result = DateTime.MinValue;

                result = context.tarefa.Where(t => t.tar_nr_agrupador == agrupador).ToList().Min(t => t.tar_dt_medicao);
                return result;
            }
        }

        public static DateTime ObterUltimoDiaExecutado(tarefa tar)
        {
            DateTime result = DateTime.MinValue;
            //todo: como obter o último dia executado
            //result = tar.historico_tarefa_rdo.ToList().Max(x => x.his_dt_data) ?? DateTime.MinValue;
            return result;
        }
        public static DateTime _ObterUltimoDiaExecutado(string agrupador)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                DateTime result = DateTime.MinValue;
                result = context.tarefa.Where(t => t.tar_nr_agrupador == agrupador).ToList().Max(t => t.tar_dt_medicao);
                return result;
            }
        }


        public static List<HistoricoTarefaViewModel> PreencherHistoricoTarefa(int id)
        {
            var historicoTarefa = new List<HistoricoTarefaViewModel>();

            using (var context = new LaudosPiscinasEntities())
            {
                var tarefa = context.tarefa.Find(id);
                if (tarefa != null)
                {
                    //var list = context.tarefa?.Where(t => t.tar_nr_agrupador == tarefa.tar_nr_agrupador && t.tar_id_status != 1).OrderByDescending(t => t.tar_dt_medicao).ThenByDescending(t => t.tar_dt_medicao_hora_inicial).ToList();
                    var list = context.tarefa?
                        .Where(t => t.tar_nr_agrupador == tarefa.tar_nr_agrupador)
                        .OrderByDescending(t => t.tar_id_status)
                        .ThenByDescending(t => t.tar_dt_medicao)
                        .ThenByDescending(t => t.tar_dt_medicao_hora_inicial)
                        .ToList();

                    foreach (var item in list)
                    {
                        historicoTarefa.Add(new HistoricoTarefaViewModel(item));
                    }
                }
            }

            return historicoTarefa;
            //List<HistoricoTarefaViewModel> result = new List<HistoricoTarefaViewModel>();

            //LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            //IQueryable<rdo_tarefa> query = context.Set<rdo_tarefa>();
            ////query = query.Where(x => x.his_id_tarefa == id).OrderBy(x => x.his_dt_data);

            ////var historicosAgrupados = query.ToList().GroupBy(h => h.his_dt_data).
            ////         Select(group =>
            ////             new
            ////             {
            ////                 Data = group.Key
            ////             });


            ////historicosAgrupados.ToList().ForEach(x => result.Add(new HistoricoTarefaViewModel
            ////{
            ////    DataStatus = HistoricoTarefaRdoModel.ObterHistorico(id, x.Data ?? DateTime.MinValue).his_dt_data ?? DateTime.MinValue,
            ////    DescricaoStatusTarefa = HistoricoTarefaRdoModel.ObterHistorico(id, x.Data ?? DateTime.MinValue).status_tarefa.stt_ds_status,
            ////    IdHistoricoTarefa = HistoricoTarefaRdoModel.ObterHistorico(id, x.Data ?? DateTime.MinValue).his_id_historico_tarefa_rdo,
            ////    IdStatusTarefa = HistoricoTarefaRdoModel.ObterHistorico(id, x.Data ?? DateTime.MinValue).his_id_status,
            ////    IdTarefa = HistoricoTarefaRdoModel.ObterHistorico(id, x.Data ?? DateTime.MinValue).his_id_tarefa,
            ////    DescricaoTarefa = HistoricoTarefaRdoModel.ObterHistorico(id, x.Data ?? DateTime.MinValue).tarefa.tar_ds_tarefa,
            ////    CPFColaborador = HistoricoTarefaRdoModel.ObterHistorico(id, x.Data ?? DateTime.MinValue).rdo.colaborador.col_nr_cpf,
            ////    NomeColaborador = HistoricoTarefaRdoModel.ObterHistorico(id, x.Data ?? DateTime.MinValue).rdo.colaborador.col_nm_colaborador
            ////}));


            //return result;
        }

        //public static bool SalvarCabeca(dynamic param)
        //{
        //    int idTarefa = param.id ?? 0;
        //    int idObra = param.idObra ?? 0;
        //    int idEtapa = param.idEtapa ?? 0;

        //    if (string.IsNullOrEmpty(param.descricao.ToString()))
        //    {
        //        throw new Exception("O titulo deve ser preenchido");
        //    }
        //    if (string.IsNullOrEmpty(param.dataInicio.ToString()))
        //    {
        //        throw new Exception("O data inicial deve ser preenchida");
        //    }
        //    if (string.IsNullOrEmpty(param.dataPrevisaoFim.ToString()))
        //    {
        //        throw new Exception("O data de previsão de final deve ser preenchida");
        //    }
        //    try
        //    {
        //        if ((Convert.ToDateTime(param.dataInicio.ToString())) > (Convert.ToDateTime(param.dataPrevisaoFim.ToString())))
        //        {
        //            throw new Exception("A data final não pode ser menor do que a data inicial.");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw new Exception("As datas selecionadas estão inválidas!");
        //    }


        //    LaudosPiscinasEntities context = new LaudosPiscinasEntities();
        //    obra _obra = context.obra.Where(x => x.obr_id_obra == idObra).FirstOrDefault() ?? new obra();
        //    if ((Convert.ToDateTime(param.dataInicio.ToString())) < (Convert.ToDateTime(_obra.obr_dt_inicio)))
        //    {
        //        throw new Exception("A data inicial da tarefa não pode ser menor do que a data inicial da obra.");
        //    }
        //    if ((Convert.ToDateTime(param.dataPrevisaoFim.ToString())) < (Convert.ToDateTime(_obra.obr_dt_inicio)))
        //    {
        //        throw new Exception("A data de previsão de final da tarefa não pode ser menor do que a data inicial da obra.");
        //    }

        //    tarefa _tarefa = context.tarefa.AsNoTracking().Where(x => x.tar_id_tarefa == idTarefa).FirstOrDefault() ?? new tarefa();

        //    _tarefa.tar_ds_tarefa = param.descricao;
        //    //_tarefa.etapa.eta_id_obra = idObra;
        //    _tarefa.tar_id_etapa = idEtapa;
        //    _tarefa.tar_dt_inicio = Convert.ToDateTime(Convert.ToString(param.dataInicio));
        //    _tarefa.tar_dt_previsao_fim = String.IsNullOrEmpty(param.dataPrevisaoFim.ToString()) ? null : Convert.ToDateTime(Convert.ToString(param.dataPrevisaoFim));
        //    //_tarefa.tar_id_status = param.status;
        //    _tarefa.tar_id_unidade = (int)param.idUnidade == 0 ? null : param.idUnidade;
        //    _tarefa.tar_ds_comentario = param.comentario;
        //    _tarefa.tar_nr_qtd_construida = Convert.ToString(param.qtdConstruida) == "" ? 0 : param.qtdConstruida;
        //    _tarefa.tar_nr_horas_trabalhadas = Convert.ToString(param.horasTrabalhadas) == "" ? 0 : param.horasTrabalhadas;
        //    _tarefa.tar_dt_ultima_atualizacao = DateTime.Now.Date;

        //    _tarefa.tar_dt_medicao = param.dataMedicao ?? DateTime.Now.Date;
        //    _tarefa.tar_dt_medicao_hora_inicial = param.horaInicial;
        //    _tarefa.tar_dt_medicao_hora_final = param.horaFinal;

        //    _tarefa.tar_nr_qtd_previsao = param.qtdPlanejada;
        //    _tarefa.tar_vl_valor_unitario = param.valor;

        //    if (_tarefa.tar_id_status == 3)
        //    {
        //        _tarefa.tar_dt_fim = DateTime.Now.Date;
        //    }

        //    if (_tarefa.tar_id_tarefa > 0)
        //    {
        //        int idStatus = Convert.ToInt32(param.status);

        //        if (ExisteInterseccaoHoraio(_tarefa))
        //        {
        //            throw new Exception("Já existe uma medição para o horário informado");
        //        }

        //        if (_tarefa.tar_id_status == idStatus)
        //        {
        //            context.tarefa.Attach(_tarefa);
        //            context.Entry(_tarefa).State = EntityState.Modified;
        //        }
        //        else
        //        {
        //            _tarefa.tar_id_tarefa = 0;
        //            context.tarefa.Add(_tarefa);
        //        }
        //    }
        //    else
        //    {
        //        _tarefa.tar_nr_agrupador = Guid.NewGuid();
        //        _tarefa.tar_dt_insercao = DateTime.Now;
        //        context.tarefa.Add(_tarefa);
        //    }

        //    _tarefa.tar_id_status = param.status;
        //    _tarefa.tar_dt_ultima_atualizacao = DateTime.Now;
        //    _tarefa.tar_id_colaborador_insercao = param.idColaboradorInsercao;

        //    bool result = context.SaveChanges() > 0;


        //    //SalvarListaColaboradores(param, context, _tarefa.etapa.eta_id_obra, _tarefa.tar_id_tarefa);
        //    SalvarListaColaboradores(param, context, idObra, _tarefa.tar_id_tarefa);
        //    //SalvarListaEquipamentos(param, context, _tarefa.etapa.eta_id_obra, _tarefa.tar_id_tarefa);
        //    SalvarListaEquipamentos(param, context, idObra, _tarefa.tar_id_tarefa);
        //    //SalvarListaAcidentes(param, context, _tarefa.etapa.eta_id_obra, _tarefa.tar_id_tarefa);
        //    SalvarListaAcidentes(param, context, idObra, _tarefa.tar_id_tarefa);
        //    //SalvarImagens(param, context, _tarefa.tar_id_tarefa, idObra);


        //    return result;
        //}
        public static bool Salvar(TarefaViewModel view)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                var entity = TarefaViewModel.ViewToEntity(view);
                int idObra = context.etapa.FirstOrDefault(et => et.eta_id_etapa == entity.tar_id_etapa).eta_id_obra;
                int qtdTarefasLicenca = context.obra.FirstOrDefault(ob => ob.obr_id_obra == idObra).empresa.licenca.lic_qtd_tarefas_obra;
                int qtdTarefasObra = context.tarefa.Where(tar => tar.etapa.eta_id_obra == idObra).GroupBy(tar => tar.tar_nr_agrupador).Count();
                
                entity.tar_dt_medicao = entity.tar_dt_inicio.Date;
                entity.tar_dt_insercao = DateTime.Now;
                entity.tar_ds_comentario = view.Comentario;

                if (entity.tar_id_tarefa > 0)
                {
                    Guid agrupador = new Guid(context.tarefa.FirstOrDefault(tar => tar.tar_id_tarefa == entity.tar_id_tarefa).tar_nr_agrupador);
                    List<rdo> rdoTarefa = context.rdo.Where(rd => (rd.rdo_id_status == 2 || rd.rdo_id_status == 3) && rd.rdo_dt_rdo == entity.tar_dt_medicao.Date && rd.rdo_id_obra == idObra && rd.rdo_tarefa.Any(rdt => rdt.tarefa.tar_nr_agrupador == entity.tar_nr_agrupador)).ToList();

                    if (rdoTarefa.Count() > 0 && entity.tar_id_tarefa != 0)
                    {
                        throw new Exception("Já existe RDO assinado para o dia " + entity.tar_dt_inicio.Date.ToString("dd/MM/yyyy") + ". Favor verificar!");
                    }
                }

                if (DataInicioTarefaMenorDataInicioObra(entity))
                {
                    throw new Exception("A Data Inicial da Tarefa não pode ser menor do que a Data Inicial da Obra.");
                }
                if (DataPrevisaoFimObraMenorDataInicioObra(entity))
                {
                    throw new Exception("A Data de Previsão de Final da Tarefa não pode ser menor do que a Data Inicial da Obra.");
                }
                //if (view.Status != 1)
                //{
                //    if (ExisteInterseccaoHorario(entity))
                //    {
                //        throw new Exception("Já existe uma medição para o horário informado");
                //    }
                //}

                if (qtdTarefasObra >= qtdTarefasLicenca)
                {
                    throw new Exception("A licença adquirida só permite adicionar " + qtdTarefasLicenca + " tarefas por obra. Favor verificar com o administrador.");
                }

                context.tarefa.Add(entity);
                var result = context.SaveChanges();

                SalvarListaColaboradores(view, context, entity);
                SalvarListaEquipamentos(view, context, entity);
                SaveImages(view, context, entity);

                //SalvarListaEquipamentos(param, context, idObra, _tarefa.tar_id_tarefa);
                //SalvarListaAcidentes(param, context, idObra, _tarefa.tar_id_tarefa);
                //SalvarImagens(param, context, _tarefa.tar_id_tarefa, idObra);


                return result > 0;
            }
        }

        public static int SalvarNovoHistorico(tarefa tar)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            tarefa newTar = new tarefa();
            newTar.tar_ds_comentario = tar.tar_ds_comentario;
            newTar.tar_ds_foto = tar.tar_ds_foto;
            newTar.tar_ds_tarefa = tar.tar_ds_tarefa;
            newTar.tar_dt_fim = tar.tar_dt_fim;
            newTar.tar_dt_inicio = tar.tar_dt_inicio;
            newTar.tar_dt_insercao = DateTime.Now;
            newTar.tar_dt_medicao = tar.tar_dt_medicao.AddDays(1);
            newTar.tar_dt_medicao_hora_final = tar.tar_dt_medicao_hora_final;
            newTar.tar_dt_medicao_hora_inicial = tar.tar_dt_medicao_hora_inicial;
            newTar.tar_dt_previsao_fim = tar.tar_dt_previsao_fim;
            newTar.tar_dt_ultima_atualizacao = DateTime.Now;
            newTar.tar_id_colaborador_insercao = tar.tar_id_colaborador_insercao;
            newTar.tar_id_etapa = tar.tar_id_etapa;
            newTar.tar_id_status = tar.tar_id_status;
            newTar.tar_id_tarefa = 0;
            newTar.tar_id_unidade = tar.tar_id_unidade;
            newTar.tar_nr_agrupador = tar.tar_nr_agrupador;
            newTar.tar_nr_horas_trabalhadas = tar.tar_nr_horas_trabalhadas;
            newTar.tar_nr_qtd_construida = tar.tar_nr_qtd_construida ?? 0;
            newTar.tar_nr_qtd_previsao = tar.tar_nr_qtd_previsao;
            newTar.tar_vl_valor_unitario = tar.tar_vl_valor_unitario;

            context.tarefa.Add(newTar);
            context.SaveChanges();

            TarefaViewModel tarViewModel = new TarefaViewModel(tar);
            tarViewModel.Id = newTar.tar_id_tarefa;
            SalvarListaColaboradoresEquipamentosNovoHistorico(tar.obra_tarefa_colaborador, tar.obra_tarefa_equipamento, context, newTar.tar_id_tarefa);
            SaveImages(tarViewModel, context, tar);

            return tar.tar_id_tarefa;
        }

        internal static bool SalvarListaColaboradoresEquipamentosNovoHistorico(ICollection<obra_tarefa_colaborador> colaboradores, ICollection<obra_tarefa_equipamento> equipamentos, LaudosPiscinasEntities context, int idNewTarefa)
        {
            foreach (obra_tarefa_colaborador otc in colaboradores)
            {
                obra_tarefa_colaborador otco = new obra_tarefa_colaborador();
                otco.otc_id_obra_colaborador = otc.otc_id_obra_colaborador;
                otco.otc_id_obra_tarefa_colaborador = 0;
                otco.otc_id_tarefa = idNewTarefa;

                context.obra_tarefa_colaborador.Add(otco);
                context.SaveChanges();
            }

            foreach (obra_tarefa_equipamento ote in equipamentos)
            {
                obra_tarefa_equipamento oteq = new obra_tarefa_equipamento();
                oteq.ote_id_obra_equipamento = ote.ote_id_obra_equipamento;
                oteq.ote_id_obra_tarefa_euipamento = 0;
                oteq.ote_id_tarefa = idNewTarefa;

                context.obra_tarefa_equipamento.Add(oteq);
                context.SaveChanges();
            }

            return true;
        }

        public static int Update(TarefaViewModel view)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                var entity = context.tarefa.Find(view.Id);
                decimal teste = Convert.ToDecimal(view.valor);
                decimal teste2 = Convert.ToDecimal(view.qtdPlanejada);
                entity.tar_dt_medicao = view.DataMedicao ?? DateTime.Now.Date;
                entity.tar_dt_medicao_hora_inicial = view.HoraInicial;
                entity.tar_dt_medicao_hora_final = view.HoraFinal;
                entity.tar_nr_qtd_construida = view.QtdConstruida ?? 0;
                entity.tar_ds_comentario = view.Comentario;
                entity.tar_dt_ultima_atualizacao = DateTime.Now;
                entity.tar_dt_inicio = Convert.ToDateTime(view.DataInicio);
                entity.tar_dt_previsao_fim = Convert.ToDateTime(view.DataPrevisaoFim);
                entity.tar_ds_tarefa = view.Descricao;
                if (view.IdUnidade != 0)
                    entity.tar_id_unidade = view.IdUnidade;
                else
                    entity.tar_id_unidade = null;
                entity.tar_id_etapa = view.IdEtapa;
                entity.tar_nr_qtd_previsao = view.qtdPlanejada;
                entity.tar_vl_valor_unitario = view.valor;
                

                int idObra = context.etapa.FirstOrDefault(et => et.eta_id_etapa == entity.tar_id_etapa).eta_id_obra;
                List<rdo> rdoTarefa = context.rdo.Where(rd => (rd.rdo_id_status == 2 || rd.rdo_id_status == 3) && rd.rdo_dt_rdo == entity.tar_dt_medicao.Date && rd.rdo_id_obra == idObra && rd.rdo_tarefa.Any(rdt => rdt.tarefa.tar_nr_agrupador == entity.tar_nr_agrupador)).ToList();

                if (rdoTarefa.Count() > 0 && entity.tar_id_tarefa != 0)
                {
                    throw new Exception("Já existe RDO assinado para o dia " + entity.tar_dt_inicio.Date.ToString("dd/MM/yyyy") + ". Favor verificar!");
                }

                if (DataInicioTarefaMenorDataInicioObra(entity))
                {
                    throw new Exception("A Data Inicial da Tarefa não pode ser menor do que a Data Inicial da Obra.");
                }
                if (DataPrevisaoFimObraMenorDataInicioObra(entity))
                {
                    throw new Exception("A Data de Previsão de Final da Tarefa não pode ser menor do que a Data Inicial da Obra.");
                }
                // Se for Status Finalizada (3)
                if (view.Status == 3)
                {
                    entity.tar_dt_fim = DateTime.Now.Date;
                }

                context.tarefa.Add(entity);

                // Se o Status não mudar então é um Update
                bool novaMedicao = view.novaMedicao ?? false;

                // Modificação feita no dia 03/12/2019
                // if (view.Status == entity.tar_id_status && !novaMedicao)
                if (!novaMedicao)
                {
                    //if (view.Status != 1)
                    //{
                    //    if (ExisteInterseccaoHorario(entity))
                    //    {
                    //        throw new Exception("Já existe uma medição para o horário informado");
                    //    }
                    //}
                    entity.tar_dt_ultima_atualizacao = DateTime.Now;
                    entity.tar_id_status = view.Status;
                    context.Entry(entity).State = EntityState.Modified;
                }
                else
                {
                    if (view.Status != 1)
                    {
                        int idTarefa = entity.tar_id_tarefa;
                        entity.tar_id_tarefa = 0; //Adicionar o 0 para efetuar o filtro da interseccao de horario corretamente
                        //if (ExisteInterseccaoHorario(entity))
                        //{
                        //    throw new Exception("Já existe uma medição para o horário informado");
                        //}
                        entity.tar_id_tarefa = idTarefa;
                    }
                    ObterBase64Images(view.ListaImagens);
                    view.ListaImagens.Select(c => { c.href = null; return c; }).ToList();
                    //entity.tar_dt_insercao = DateTime.Now;
                    entity.tar_id_status = view.Status;
                }

                var result = context.SaveChanges();

                SalvarListaColaboradores(view, context, entity);
                SalvarListaEquipamentos(view, context, entity);

                SalvarListaAcidentes(view, context, entity);

                SaveImages(view, context, entity);
                return result;
            }
        }

        private static bool SaveImages(TarefaViewModel view, LaudosPiscinasEntities context, tarefa entity)
        {
            var caminhoRelativo = "/uploads/tarefa/" + entity.tar_id_tarefa;
            var caminhoAbsoluto = HostingEnvironment.ApplicationPhysicalPath + caminhoRelativo;
            if (!Directory.Exists(caminhoAbsoluto)) { Directory.CreateDirectory(caminhoAbsoluto); }

            List<imagem> imagensParaRemover = context.imagem.Where(ima => ima.ima_id_tarefa == entity.tar_id_tarefa).ToList();

            if (imagensParaRemover.Count > 0)
            {
                if (view.ListaImagens != null)
                {
                    foreach (var item in view.ListaImagens)
                    {
                        string caminho = item.href ?? "";
                        caminho = caminho.Replace("/homologa/", "");
                        imagensParaRemover.RemoveAll(i => i.ima_ds_caminho == caminho);
                    }
                }
            }

            if (view.ListaImagens != null)
            {
                foreach (var item in view.ListaImagens)
                {
                    if (item.href == null)
                    {
                        string imagem = item.base64;
                        byte[] Imagembytes = Convert.FromBase64String(imagem);
                        var nomeArquivo = "/" + Guid.NewGuid().ToString("N") + ".png";
                        var caminhoAbsolutoArquivo = caminhoAbsoluto + nomeArquivo;

                        using (MemoryStream ms = new MemoryStream(Imagembytes))
                        {
                            using (Bitmap bm2 = new Bitmap(ms))
                            {
                                Bitmap resized = new Bitmap(bm2, new Size(bm2.Width / 2, bm2.Height / 2));
                                ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(enc => enc.MimeType == (string)item.filetype);
                                EncoderParameters imgParams = new EncoderParameters(1);
                                imgParams.Param = new[] { new EncoderParameter(Encoder.Quality, 0L) };

                                resized.Save(caminhoAbsolutoArquivo, codec, imgParams);

                                context.imagem.Add(new imagem
                                {
                                    ima_ds_caminho = caminhoRelativo + nomeArquivo,
                                    ima_dt_imagem = DateTime.Now,
                                    ima_id_tarefa = entity.tar_id_tarefa
                                });
                                context.SaveChanges();
                            }
                        }
                    }
                    //else
                    //{
                    //    int idRDO = 0;
                    //    string _idRDO = item.idRDO;
                    //    int.TryParse(_idRDO, out idRDO);
                    //    if (idRDO == 0)
                    //    {
                    //        string caminho = item.href;
                    //        LaudosPiscinasEntities context1 = new LaudosPiscinasEntities();
                    //        imagem imagem_ = context1.imagem.FirstOrDefault(i => i.ima_ds_caminho == caminho);
                    //        if (imagem_ != null)
                    //        {
                    //            context1.imagem.Remove(imagem_);
                    //            context1.SaveChanges();
                    //        }
                    //        string caminhoArquivo = HostingEnvironment.ApplicationPhysicalPath + caminho;
                    //        if (File.Exists(caminhoArquivo))
                    //        {
                    //            File.Delete(caminhoArquivo);
                    //        }
                    //    }
                    //}

                }
                if (imagensParaRemover.Count() > 0)
                {
                    foreach (var imagem in imagensParaRemover)
                    {
                        if (imagem.ima_id_historico_tarefa_rdo == null)
                        {
                            context.imagem.Remove(imagem);
                            context.SaveChanges();
                            string caminhoArquivo = HostingEnvironment.ApplicationPhysicalPath + imagem.ima_ds_caminho;
                            if (File.Exists(caminhoArquivo))
                            {
                                File.Delete(caminhoArquivo);
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static List<dynamic> ObterBase64Images(List<dynamic> images) //Obter as imagens da tarefa antiga em base64
        {
            foreach (var item in images)
            {
                var caminhoAbsoluto = HostingEnvironment.ApplicationPhysicalPath + item.href;

                if (File.Exists(caminhoAbsoluto))
                {
                    item.base64 = Convert.ToBase64String(File.ReadAllBytes(caminhoAbsoluto));
                }
            }
            return images;
        }

        private static bool DataPrevisaoFimObraMenorDataInicioObra(tarefa tarefa)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                var obra = context.obra.Where(x => x.etapa.Any(e => e.eta_id_etapa == tarefa.tar_id_etapa)).FirstOrDefault();
                if (tarefa.tar_dt_previsao_fim < obra.obr_dt_inicio)
                {
                    return true;
                }
                return false;
            }
        }

        private static bool DataInicioTarefaMenorDataInicioObra(tarefa tarefa)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                var obra = context.obra.Where(x => x.etapa.Any(e => e.eta_id_etapa == tarefa.tar_id_etapa)).FirstOrDefault();
                if (tarefa.tar_dt_inicio < obra.obr_dt_inicio)
                {
                    return true;
                }
                return false;
            }
        }

        private static bool ExisteInterseccaoHorario(tarefa tarefa)
        {
            TimeSpan zero = new TimeSpan(0, 0, 0);
            if (tarefa.tar_dt_medicao_hora_inicial == null || tarefa.tar_dt_medicao_hora_final == null || (tarefa.tar_dt_medicao_hora_inicial == zero && tarefa.tar_dt_medicao_hora_final == zero))
            {
                return false;
            }
            using (var context = new LaudosPiscinasEntities())
            {
                var horaInicial = (TimeSpan)tarefa.tar_dt_medicao_hora_inicial;
                var horaFinal = (TimeSpan)tarefa.tar_dt_medicao_hora_final;

                var list = context.tarefa.Where(t => t.tar_nr_agrupador == tarefa.tar_nr_agrupador && t.tar_dt_medicao == tarefa.tar_dt_medicao && t.tar_id_tarefa != tarefa.tar_id_tarefa).ToList();

                foreach (var item in list)
                {
                    if (item.tar_dt_medicao_hora_inicial != null)
                    {
                        var itemHoraInicial = (TimeSpan)item.tar_dt_medicao_hora_inicial;
                        var itemHoraFinal = (TimeSpan)item.tar_dt_medicao_hora_final;

                        for (int i = (int)itemHoraInicial.TotalMinutes; i < (int)itemHoraFinal.TotalMinutes; i++)
                        {
                            for (int j = (int)horaInicial.TotalMinutes; j < (int)horaFinal.TotalMinutes; j++)
                            {
                                if (i == j)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static bool SalvarImagens(TarefaViewModel param, LaudosPiscinasEntities context, tarefa entity)
        {
            string PastaDeImagens = ConfigurationManager.AppSettings["PastaDeImagens"] ?? "~/Uploads/Imagens/";
            PastaDeImagens = System.Web.Hosting.HostingEnvironment.MapPath(PastaDeImagens);
            if (!Directory.Exists(PastaDeImagens)) { Directory.CreateDirectory(PastaDeImagens); }

            int idObra = entity.etapa.eta_id_obra;
            int idTarefa = entity.tar_id_etapa;
            string PrimeiraParteNomeDoArquivo = string.Concat(idObra.ToString(), "/", idTarefa.ToString(), "/", DateTime.Today.ToString("yyyyMMdd"), "/");
            if (!Directory.Exists(string.Concat(PastaDeImagens, PrimeiraParteNomeDoArquivo))) { Directory.CreateDirectory(string.Concat(PastaDeImagens, PrimeiraParteNomeDoArquivo)); }

            List<ImageFile> imagens = new List<ImageFile>();
            if (param.ListaImagens != null)
            {
                foreach (var imagem in param.ListaImagens)
                {
                    imagens.Add(new ImageFile
                    {
                        //id = imagem.id == null ? int.MinValue : imagem.id,
                        //filetype = imagem.filetype,
                        //filename = imagem.filename,
                        //filesize = imagem.filesize,
                        //base64 = imagem.base64
                    });
                }
            }

            foreach (var img in context.imagem.Where(x => x.ima_id_tarefa == idTarefa))
            {
                int id = img.ima_id_imagem;
                if (imagens.Where(x => x.id == id).Count() == 0)
                {
                    context.imagem.Remove(img);
                }
            }


            foreach (var imageFile in imagens)
            {
                if (!string.IsNullOrEmpty(imageFile.base64))
                {
                    string NomeDoArquivo = string.Concat(PrimeiraParteNomeDoArquivo, Guid.NewGuid().ToString().Replace("-", "").ToUpper(), imageFile.Extension());
                    context.imagem.Add(new imagem() { ima_ds_caminho = NomeDoArquivo, ima_dt_imagem = DateTime.Today, ima_id_tarefa = idTarefa });
                    File.WriteAllBytes(string.Concat(PastaDeImagens, NomeDoArquivo), Convert.FromBase64String(imageFile.base64));
                }
            }


            bool result = true;

            result = context.SaveChanges() > 0;

            return result;
        }

        internal static void VerificaListaAcidentes(TarefaViewModel param, LaudosPiscinasEntities context, tarefa entity)
        {

            foreach (var item in param.listaAcidentes)
            {
                int idAcidente = item.IdAcidente;
                item.IdTarefa = entity.tar_id_tarefa;
                DateTime data = Convert.ToDateTime(Convert.ToString(item.DataAcidente));
                data = Convert.ToDateTime(data.ToString("dd/MM/yyyy"));

                if (idAcidente == 0)
                {
                    rdo _rdo = context.rdo.FirstOrDefault(rdo => rdo.rdo_id_status > 1 && rdo.rdo_dt_rdo == data && rdo.rdo_tarefa.Any(tar => tar.rta_id_tarefa == entity.tar_id_tarefa));
                    if (_rdo != null)
                    {
                        throw new Exception("O acidente da data " + data.ToString("dd/MM/yyyy") + " não pode ser adicionado pois já existe um RDO assinado.");
                    }
                }
            }

            foreach (var item in param.listaAcidentesRemovidos)
            {
                int idAcidente = item.IdAcidente;
                item.IdTarefa = entity.tar_id_tarefa;
                DateTime data = Convert.ToDateTime(Convert.ToString(item.DataAcidente));
                data = Convert.ToDateTime(data.ToString("dd/MM/yyyy"));


                if (idAcidente > 0)
                {
                    rdo _rdo = context.rdo.FirstOrDefault(rdo => rdo.rdo_id_status > 1 && rdo.rdo_dt_rdo == data && rdo.rdo_tarefa.Any(tar => tar.rta_id_tarefa == entity.tar_id_tarefa));
                    if (_rdo != null)
                    {
                        throw new Exception("O acidente da data " + data.ToString("dd/MM/yyyy") + " não pode ser removido pois já existe um RDO assinado.");
                    }
                }
            }
        }

        //internal static bool SalvarListaAcidentes(dynamic param, LaudosPiscinasEntities context, int idObra, int idTarefa)
        //{
        //    VerificaListaAcidentes(param, context, idObra, idTarefa);

        //    bool result = true;
        //    List<AcidenteViewModel> acidentesAdicionados = new List<AcidenteViewModel>();

        //    foreach (var item in param.listaAcidentes)
        //    {
        //        item.idTarefa = idTarefa;
        //        AcidenteViewModel acidenteSalvo = AcidenteModel.Salvar(item);
        //        acidentesAdicionados.Add(acidenteSalvo);
        //    }

        //    foreach (var item in param.listaAcidentesRemovidos)
        //    {
        //        int idAcidente = item.idAcidente ?? 0;
        //        bool acidenteAdicionado = acidentesAdicionados.Any(x => x.IdAcidente == idAcidente);

        //        if (context.acidente.ToList().Any(x => x.aci_id_acidente == idAcidente) && !acidenteAdicionado)
        //        {
        //            context.acidente_colaborador.Where(x => x.acc_id_acidente == idAcidente).ToList().ForEach(y => context.acidente_colaborador.Remove(y));
        //            context.acidente.Where(x => x.aci_id_acidente == idAcidente).ToList().ForEach(y => context.acidente.Remove(y));
        //        }
        //    }


        //    result = context.SaveChanges() > 0;

        //    return result;
        //}
        internal static bool SalvarListaAcidentes(TarefaViewModel param, LaudosPiscinasEntities context, tarefa tarefa)
        {

            VerificaListaAcidentes(param, context, tarefa);

            bool result = true;
            List<AcidenteViewModel> acidentesAdicionados = new List<AcidenteViewModel>();

            foreach (var item in param.listaAcidentes)
            {
                item.IdTarefa = tarefa.tar_id_tarefa;
                AcidenteViewModel acidenteSalvo = AcidenteModel.Salvar(item);
                acidentesAdicionados.Add(acidenteSalvo);
            }

            foreach (var item in param.listaAcidentesRemovidos)
            {
                int idAcidente = item.IdAcidente;
                bool acidenteAdicionado = acidentesAdicionados.Any(x => x.IdAcidente == idAcidente);

                if (context.acidente.ToList().Any(x => x.aci_id_acidente == idAcidente) && !acidenteAdicionado)
                {
                    context.acidente_colaborador.Where(x => x.acc_id_acidente == idAcidente).ToList().ForEach(y => context.acidente_colaborador.Remove(y));
                    context.acidente.Where(x => x.aci_id_acidente == idAcidente).ToList().ForEach(y => context.acidente.Remove(y));
                }
            }


            result = context.SaveChanges() > 0;

            return result;
        }

        internal static bool SalvarListaColaboradores(TarefaViewModel param, LaudosPiscinasEntities context, tarefa tarefa)
        {
            bool result = true;
            int idObra = context.obra.Single(o => o.etapa.Any(e => e.eta_id_etapa == tarefa.tar_id_etapa)).obr_id_obra;
            var colaboradoresAdicionados = new List<ColaboradorViewModel>();

            //salvar e atualizar o objeto dos outros colaboradores
            foreach (var item in param.listaColaboradores)
            {
                item.IdObra = idObra;
                item.ContratanteContratada = param.ContratanteContratada;

                ColaboradorViewModel colaboradorSalvo = ColaboradorModel.Salvar(item, true);
                int idColaborador = Convert.ToInt32(colaboradorSalvo.IdColaborador);
                ColaboradorModel.AssociarColaboradorTarefa(idObra, idColaborador, tarefa.tar_id_tarefa);
                colaboradoresAdicionados.Add(colaboradorSalvo);
            }

            //remover os coleboradores
            foreach (var item in param.listaColaboradoresRemovidos)
            {
                bool colaboradorAdicionado = colaboradoresAdicionados.Any(x => x.IdColaborador == (int)item.IdColaborador);
                int idObraColaborador = item.IdObraColaborador;

                if (context.obra_tarefa_colaborador.ToList().Any(x => x.otc_id_obra_colaborador == idObraColaborador && x.otc_id_tarefa == param.Id) && !colaboradorAdicionado)
                {
                    context.obra_tarefa_colaborador.Where(x => x.otc_id_obra_colaborador == idObraColaborador && x.otc_id_tarefa == param.Id).ToList().ForEach(y => context.obra_tarefa_colaborador.Remove(y));
                }
            }

            result = context.SaveChanges() > 0;

            return result;
        }
        //internal static bool SalvarListaColaboradores(dynamic param, LaudosPiscinasEntities context, int idObra, int idTarefa)
        //{
        //    bool result = true;
        //    List<ColaboradorViewModel> colaboradoresAdicionados = new List<ColaboradorViewModel>();

        //    //salvar e atualizar o objeto dos outros colaboradores
        //    foreach (var item in param.listaColaboradores)
        //    {
        //        item.idObra = idObra;
        //        item.contratanteContratada = param.contratanteContratada;

        //        ColaboradorViewModel colaboradorSalvo = ColaboradorModel.Salvar(item, true);
        //        int idColaborador = Convert.ToInt32(colaboradorSalvo.IdColaborador);
        //        ColaboradorModel.AssociarColaboradorTarefa(idObra, idColaborador, idTarefa);
        //        colaboradoresAdicionados.Add(colaboradorSalvo);
        //    }

        //    //remover os coleboradores
        //    foreach (var item in param.listaColaboradoresRemovidos)
        //    {
        //        bool colaboradorAdicionado = colaboradoresAdicionados.Any(x => x.IdColaborador == (int)item.idColaborador);
        //        int idObraColaborador = item.idObraColaborador ?? 0;

        //        if (context.obra_tarefa_colaborador.ToList().Any(x => x.otc_id_obra_colaborador == idObraColaborador && x.otc_id_tarefa == idTarefa) && !colaboradorAdicionado)
        //        {
        //            context.obra_tarefa_colaborador.Where(x => x.otc_id_obra_colaborador == idObraColaborador && x.otc_id_tarefa == idTarefa).ToList().ForEach(y => context.obra_tarefa_colaborador.Remove(y));
        //        }
        //    }

        //    result = context.SaveChanges() > 0;

        //    return result;
        //}


        //internal static bool SalvarListaEquipamentos(dynamic param, LaudosPiscinasEntities context, int idObra, int idTarefa)
        //{
        //    bool result = true;
        //    List<EquipamentoViewModel> equipamentosAdicionados = new List<EquipamentoViewModel>();

        //    foreach (var item in param.listaEquipamentos)
        //    {
        //        item.idObra = idObra;
        //        EquipamentoViewModel equipamentoSalvo = EquipamentosModel.Salvar(item);
        //        int idEquipamento = Convert.ToInt32(equipamentoSalvo.Id);
        //        EquipamentosModel.AssociarEquipamentoTarefa(idObra, idEquipamento, idTarefa);
        //        equipamentosAdicionados.Add(equipamentoSalvo);
        //    }


        //    foreach (var item in param.listaEquipamentosRemovidos)
        //    {
        //        bool equipamentoAdicionado = equipamentosAdicionados.Any(x => x.Id == (int)item.id);
        //        int idObraEquipamento = item.idObraEquipamento ?? 0;

        //        if (context.obra_tarefa_equipamento.ToList().Any(x => x.ote_id_obra_equipamento == idObraEquipamento && x.ote_id_tarefa == idTarefa) && !equipamentoAdicionado)
        //        {
        //            context.obra_tarefa_equipamento.Where(x => x.ote_id_obra_equipamento == idObraEquipamento && x.ote_id_tarefa == idTarefa).ToList().ForEach(y => context.obra_tarefa_equipamento.Remove(y));
        //        }
        //    }

        //    result = context.SaveChanges() > 0;

        //    return result;
        //}


        internal static bool SalvarListaEquipamentos(TarefaViewModel param, LaudosPiscinasEntities context, tarefa entity)
        {
            bool result = true;
            List<EquipamentoViewModel> equipamentosAdicionados = new List<EquipamentoViewModel>();
            int IdObra = context.obra.Single(o => o.etapa.Any(e => e.eta_id_etapa == entity.tar_id_etapa)).obr_id_obra;

            foreach (var item in param.listaEquipamentos)
            {
                item.IdObra = IdObra;
                EquipamentoViewModel equipamentoSalvo = EquipamentosModel.Salvar(item);
                int idEquipamento = Convert.ToInt32(equipamentoSalvo.Id);
                EquipamentosModel.AssociarEquipamentoTarefa(IdObra, idEquipamento, entity.tar_id_tarefa);
                equipamentosAdicionados.Add(equipamentoSalvo);
            }


            foreach (var item in param.listaEquipamentosRemovidos)
            {
                bool equipamentoAdicionado = equipamentosAdicionados.Any(x => x.Id == item.Id);
                int idObraEquipamento = item.IdObraEquipamento;

                if (context.obra_tarefa_equipamento.ToList().Any(x => x.ote_id_obra_equipamento == idObraEquipamento && x.ote_id_tarefa == entity.tar_id_tarefa) && !equipamentoAdicionado)
                {
                    context.obra_tarefa_equipamento.Where(x => x.ote_id_obra_equipamento == idObraEquipamento && x.ote_id_tarefa == entity.tar_id_tarefa).ToList().ForEach(y => context.obra_tarefa_equipamento.Remove(y));
                }
            }

            result = context.SaveChanges() > 0;

            return result;
        }

        public static bool AtualizarStatus(dynamic param)
        {
            int idTarefa = param.tarefa.id ?? 0;
            int idStatus = param.idStatus ?? 0;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            tarefa _tarefa = context.tarefa.AsNoTracking().Where(x => x.tar_id_tarefa == idTarefa).FirstOrDefault() ?? new tarefa();
            //_tarefa.tar_id_status = idStatus;
            //_tarefa.tar_dt_ultima_atualizacao = DateTime.Now;
            if (_tarefa.tar_id_status == 3)
            {
                _tarefa.tar_dt_fim = DateTime.Now.Date;
            }


            if (_tarefa.tar_id_tarefa > 0)
            {
                if (_tarefa.tar_id_status == idStatus)
                {
                    context.tarefa.Attach(_tarefa);
                    context.Entry(_tarefa).State = EntityState.Modified;
                }
                else
                {
                    _tarefa.tar_id_status = idStatus;
                    _tarefa.tar_dt_ultima_atualizacao = DateTime.Now;
                    _tarefa.tar_id_colaborador_insercao = param.idColaboradorInsercao;
                    _tarefa.tar_dt_insercao = DateTime.Now;
                    _tarefa.tar_dt_medicao = DateTime.Now;
                    _tarefa.tar_id_tarefa = 0;
                    context.tarefa.Add(_tarefa);
                }
            }
            else
            {
                _tarefa.tar_id_status = idStatus;
                _tarefa.tar_dt_ultima_atualizacao = DateTime.Now;
                _tarefa.tar_id_colaborador_insercao = param.idColaboradorInsercao;
                _tarefa.tar_nr_agrupador = Guid.NewGuid().ToString();
                _tarefa.tar_dt_insercao = DateTime.Now;
                _tarefa.tar_dt_medicao = DateTime.Now;
                context.tarefa.Add(_tarefa);
            }

            bool result = context.SaveChanges() > 0;

            return result;
        }


        public static bool AtualizarStatusEmMassa(dynamic param)
        {
            bool result = false;

            int idStatus = (int)param.status;

            foreach (var item in param.tarefas)
            {
                if (!String.IsNullOrEmpty(Convert.ToString(item)))
                {
                    int idTarefa = Convert.ToInt32(Convert.ToString(item).Remove(0, 10));

                    dynamic dynObj = new System.Dynamic.ExpandoObject();
                    dynObj.tarefa = new { id = idTarefa };
                    dynObj.idStatus = idStatus;
                    result = AtualizarStatus(dynObj);
                }
            }

            return result;
        }


        internal static bool Deletar(int idTarefa)
        {
            bool result = false;

            using (var context = new LaudosPiscinasEntities())
            {
                try
                {
                    var tarefa = context.tarefa.Find(idTarefa);

                    var tarefas = context.tarefa.Where(t => t.tar_nr_agrupador == tarefa.tar_nr_agrupador).ToList();


                    foreach (var item in tarefas)
                    {
                        context.acidente.Where(x => x.aci_id_tarefa == item.tar_id_tarefa).ToList().ForEach(x => x.acidente_colaborador.Where(y => y.acc_id_acidente == x.aci_id_acidente).ToList().ForEach(y => context.acidente_colaborador.Remove(y)));
                        context.acidente.Where(x => x.aci_id_tarefa == item.tar_id_tarefa).ToList().ForEach(y => context.acidente.Remove(y));
                        context.obra_tarefa_equipamento.Where(x => x.ote_id_tarefa == item.tar_id_tarefa).ToList().ForEach(y => context.obra_tarefa_equipamento.Remove(y));
                        context.obra_tarefa_colaborador.Where(x => x.otc_id_tarefa == item.tar_id_tarefa).ToList().ForEach(y => context.obra_tarefa_colaborador.Remove(y));
                        context.tarefa.Where(x => x.tar_id_tarefa == item.tar_id_tarefa).ToList().ForEach(y => context.tarefa.Remove(y));
                    }

                    result = context.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }

        public static TarefaViewModel ObterRegistro(dynamic param)
        {
            int idTarefa = (int)param;
            int idObra = 0;

            TarefaViewModel tarefa = new TarefaViewModel();
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            tarefa resultado = context.tarefa.FirstOrDefault(tar => tar.tar_id_tarefa == idTarefa);
            idObra = resultado.etapa.eta_id_obra;

            tarefa.Id = resultado.tar_id_tarefa;
            tarefa.Descricao = resultado.tar_ds_tarefa;
            tarefa.Comentario = resultado.tar_ds_comentario;
            tarefa.Status = resultado.tar_id_status;
            tarefa.IdUnidade = resultado.tar_id_unidade ?? 0;
            tarefa.IdEtapa = resultado.tar_id_etapa;
            tarefa.RdoAssinado = context.rdo.Where(rd => (rd.rdo_id_status == 2 || rd.rdo_id_status == 3) && rd.rdo_tarefa.Any(rdt => rdt.rta_id_tarefa == resultado.tar_id_tarefa)).Count() > 0 ? true : false; //Verificar se nesta medição (tarefa) houve Rdo Assinado para não permitir editar esta medição
            //todo: ObterRegistro
            //tarefa.IdObra = resultado.etapa.eta_id_obra;
            tarefa.DataInicio = resultado.tar_dt_inicio.ToString().Substring(0, 10);
            tarefa.DataPrevisaoFim = resultado.tar_dt_previsao_fim == null ? "" : resultado.tar_dt_previsao_fim.ToString().Substring(0, 10);
            tarefa.QtdConstruida = resultado.tar_nr_qtd_construida ?? 0;
            tarefa.HorasTrabalhadas = resultado.tar_nr_horas_trabalhadas ?? 0;
            tarefa.listaHistoricoTarefa = PreencherHistoricoTarefa(resultado.tar_id_tarefa);
            tarefa.qtdPlanejada = decimal.Round(Convert.ToDecimal(resultado.tar_nr_qtd_previsao), 2, MidpointRounding.AwayFromZero);
            tarefa.valor = decimal.Round(resultado.tar_vl_valor_unitario ?? 0, 2, MidpointRounding.AwayFromZero);
            tarefa.DataMedicaoTela = resultado.tar_dt_medicao.ToString();
            tarefa.HoraInicial = resultado.tar_dt_medicao_hora_inicial;
            tarefa.HoraFinal = resultado.tar_dt_medicao_hora_final;

            tarefa.listaColaboradores = new List<ColaboradorViewModel>();
            tarefa.listaEquipamentos = new List<EquipamentoViewModel>();

            List<obra_tarefa_colaborador> listaTarefaColaborador = context.obra_tarefa_colaborador.Where(otc => otc.otc_id_tarefa == idTarefa).ToList();
            List<obra_tarefa_equipamento> listaTarefaEquipamento = context.obra_tarefa_equipamento.Where(ote => ote.ote_id_tarefa == idTarefa).ToList();
            List<acidente> listaAcidenteTarefa = context.acidente.Where(aci => aci.tarefa.tar_nr_agrupador == resultado.tar_nr_agrupador).ToList();


            listaTarefaColaborador.ForEach(x =>
                 tarefa.listaColaboradores.Add(ColaboradorModel.ObterRegistro(idObra, x.obra_colaborador.oco_id_colaborador)
                )
            );


            listaTarefaEquipamento.ForEach(x =>
                 tarefa.listaEquipamentos.Add(EquipamentosModel.ObterRegistro(idObra, x.obra_equipamento.oeq_id_equipamento)
                )
            );


            listaAcidenteTarefa.ForEach(x =>
                 tarefa.listaAcidentes.Add(AcidenteModel.ObterRegistro(x.aci_id_acidente)
                )
            );

            return tarefa;
        }

        public static List<TarefaViewModel> ListaTarefaRdo(int idRdo)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            List<TarefaViewModel> Lista = new List<TarefaViewModel>();
            foreach (rdo_tarefa rdt in context.rdo_tarefa.Where(rt => rt.rta_id_rdo == idRdo).ToList())
            {
                TarefaViewModel Tvm = new TarefaViewModel();
                if (Lista.Where(tar => tar.Agrupador == rdt.tarefa.tar_nr_agrupador.ToString()).Count() == 0)
                {
                    Tvm.Agrupador = rdt.tarefa.tar_nr_agrupador.ToString();
                    Tvm.Id = rdt.rta_id_tarefa;
                    Tvm.IdEtapa = rdt.tarefa.tar_id_etapa;
                    Tvm.IdRdo = rdt.rta_id_rdo;
                    Lista.Add(Tvm);
                }
            }

            return Lista;
        }

        public static bool ExisteTarefaDiaAnterior(int idTarefa, int idEtapa, DateTime dataTarefa, string agrupador)
        {
            DateTime dataRdoAnterior = dataTarefa.AddDays(-1);
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            tarefa tarefa = context.tarefa.Where(x => x.tar_dt_medicao == dataRdoAnterior && x.tar_id_etapa == idEtapa && x.tar_nr_agrupador == agrupador).FirstOrDefault() ?? new tarefa();
            if (tarefa.tar_id_tarefa > 0)
            {
                return true;
            }
            return false;
        }

        public static bool ExisteTarefaDiaRequisitado(int idTarefa, int idEtapa, DateTime dataTarefa, string agrupador)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            tarefa tarefa = context.tarefa.Where(x => x.tar_dt_medicao == dataTarefa && x.tar_id_etapa == idEtapa && x.tar_nr_agrupador == agrupador).FirstOrDefault() ?? new tarefa();
            if (tarefa.tar_id_tarefa > 0)
            {
                return true;
            }
            return false;
        }

        public static List<ImagemViewModel> ObterTodasImagensHistoricoMedicao(string agrupador, DateTime filtroDataMedicao) // Obter todas as imagens que antecede o filtro de medição
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            List<imagem> imagens = context.imagem.Where(x => x.tarefa.tar_nr_agrupador == agrupador && x.tarefa.tar_dt_medicao <= filtroDataMedicao).ToList();
            List<ImagemViewModel> ListaImagem = new List<ImagemViewModel>();
            if (imagens.Count > 0)
            {
                string basePath = ConfigurationManager.AppSettings["basePath"];
                basePath = basePath.Remove(basePath.Length - 1);
                foreach (var item in imagens)
                {
                    ListaImagem.Add(new ImagemViewModel
                    {
                        idImagem = item.ima_id_imagem,
                        dsCaminho = basePath + item.ima_ds_caminho
                    });
                }
            }

            return ListaImagem;
        }


        public static string RetornaMimeType(string extensao)
        {
            switch (extensao)
            {
                case "jpg":
                    return "image/jpeg";
                case "png":
                    return "image/png";
                default:
                    return "";
            }
        }

        public static int AjustarOrdenamentoTarefas(string status) //Método criado para ajudar no ordenamento das tarefas nas etapas
        {
            switch (status)
            {
                case "Em Execução":
                    return 1;
                case "Pausada":
                    return 2;
                case "Planejada":
                    return 3;
                case "Finalizada":
                    return 4;
                case "Cancelada":
                    return 5;
                default:
                    return 6;
            }
        }

    }
    public struct ImageFile
    {
        public string filetype { get; set; }
        public string filename { get; set; }
        public string filesize { get; set; }
        public string base64 { get; set; }
        public string href { get; set; }
        public int id { get; set; }
        public int? idRDO { get; set; }

        public string Extension()
        {
            string extension = string.Concat(".", filename.Split('.')[filename.Split('.').Length - 1]).ToLower() ?? "";
            return extension;
        }
    }
    public class HistoricoTarefaViewModel
    {
        public int IdHistoricoTarefa { get; set; }
        public int IdTarefa { get; set; }
        public int IdStatusTarefa { get; set; }
        public DateTime DataStatus { get; set; }
        public string DescricaoStatusTarefa { get; set; }
        public string DescricaoTarefa { get; set; }
        public string CPFColaborador { get; set; }
        public string NomeColaborador { get; set; }
        public TimeSpan? HoraInicial { get; set; }
        public TimeSpan? HoraFinal { get; set; }
        public float? QtdConstruida { get; set; }
        public bool RdoAssinado { get; set; }
        public decimal? Unidade { get; set; }
        public string UnidadeMedida { get; set; }
        public HistoricoTarefaViewModel() { }
        public HistoricoTarefaViewModel(tarefa item)
        {
            this.IdTarefa = item.tar_id_tarefa; // id do registro que queremos trabalhar (é o registro da tarefa, mas observe que estamos 
                                                // agrupando vários registros de tarefa com o agrupador. Neste caso, estamos editando um 
                                                // dos registros
            this.NomeColaborador = item.colaborador.col_nm_colaborador;
            this.CPFColaborador = item.colaborador.col_nr_cpf;
            this.DataStatus = item.tar_dt_medicao;
            //this.DataStatus = (DateTime)item.tar_dt_ultima_atualizacao;
            this.DescricaoStatusTarefa = item.status_tarefa.stt_ds_status;
            this.HoraInicial = item.tar_dt_medicao_hora_inicial;
            this.HoraFinal = item.tar_dt_medicao_hora_final;
            this.Unidade = item.tar_vl_valor_unitario;
            this.QtdConstruida = item.tar_nr_qtd_construida ?? 0;
            this.UnidadeMedida = item.unidade_de_medida != null ? item.unidade_de_medida.unm_ds_unidade : string.Empty;
            this.RdoAssinado = item.rdo_tarefa.Where(ta => ta.rdo.assinatura_rdo.Any(ass => ass.ass_id_rdo == ta.rta_id_rdo)).Count() > 0 ? true : false;
        }
    }
    public class TarefaViewModel
    {
        public bool? novaMedicao { get; set; }
        public int? Id { get; set; }
        public string Descricao { get; set; }
        public int Status { get; set; }
        public string NomeStatus { get; set; }
        public int IdEtapa { get; set; }
        public int IdRdo { get; set; }
        public int IdUnidade { get; set; }
        public int OrdemStatus { get; set; }
        public DateTime OrdemDataInicial { get; set; }
        public float? QtdConstruida { get; set; }
        public double HorasTrabalhadas { get; set; }
        public string DataInicio { get; set; }
        public string DataPrevisaoFim { get; set; }
        public DateTime PrimeiraExecucao { get; set; }
        public DateTime UltimaExecucao { get; set; }
        public string Comentario { get; set; }
        public string Foto { get; set; }
        public int QuantidadeEquipamentos { get; set; }
        public int QuantidadeColaboradores { get; set; }
        public double PercentualConcluido { get; set; }
        public string PercentualExtrapolado { get; set; }
        public string ClasseStatusCss { get; set; }
        public string Agrupador { get; set; }
        public bool ExisteExecucao { get; set; }
        public bool ExisteRegistroMedicaoDiaAnterior { get; set; }
        public bool ExisteRegistroMedicaoDataRequerida { get; set; }
        public decimal? qtdPlanejada { get; set; }
        public decimal? valor { get; set; }
        public int IdColaboradorInsercao { get; set; }
        public List<ImagemViewModel> ListaImagem { get; set; }

        // todo: saber o que é esta propriedade. parece q é p saber se é contratante ou contratada
        public string ContratanteContratada { get; set; }
        public DateTime? DataMedicao { get; set; }
        public string DataMedicaoTela { get; set; }
        public TimeSpan? HoraInicial { get; set; }
        public TimeSpan? HoraFinal { get; set; }
        public ColaboradorViewModel colaboradorObj { get; set; }
        public EquipamentoViewModel equipamentoObj { get; set; }
        public List<dynamic> ListaImagens { get; set; }
        public virtual ICollection<ColaboradorViewModel> listaColaboradores { get; set; }
        public virtual ICollection<EquipamentoViewModel> listaEquipamentos { get; set; }
        public virtual ICollection<ColaboradorViewModel> listaColaboradoresRemovidos { get; set; }
        public virtual ICollection<EquipamentoViewModel> listaEquipamentosRemovidos { get; set; }
        public virtual ICollection<AcidenteViewModel> listaAcidentesRemovidos { get; set; }
        public virtual ICollection<ColaboradorViewModel> listaColaboradoresObra { get; set; }
        public virtual ICollection<EquipamentoViewModel> listaEquipamentosObra { get; set; }
        public virtual ICollection<HistoricoTarefaViewModel> listaHistoricoTarefa { get; set; }
        public virtual ICollection<AcidenteViewModel> listaAcidentes { get; set; }
        public virtual ICollection<StatusTarefaViewModel> listaStatusPermitidos { get; set; }
        public bool Marcado { get; set; }
        public bool RdoAssinado { get; set; }
        public TarefaViewModel()
        {
            this.listaColaboradores = new HashSet<ColaboradorViewModel>();
            this.listaColaboradoresRemovidos = new HashSet<ColaboradorViewModel>();
            this.listaEquipamentosRemovidos = new HashSet<EquipamentoViewModel>();
            this.listaAcidentesRemovidos = new HashSet<AcidenteViewModel>();
            this.listaEquipamentos = new HashSet<EquipamentoViewModel>();
            this.listaColaboradoresObra = new HashSet<ColaboradorViewModel>();
            this.listaEquipamentosObra = new HashSet<EquipamentoViewModel>();
            this.listaHistoricoTarefa = new HashSet<HistoricoTarefaViewModel>();
            this.listaAcidentes = new HashSet<AcidenteViewModel>();
            this.colaboradorObj = new ColaboradorViewModel();
            this.equipamentoObj = new EquipamentoViewModel();
            this.listaStatusPermitidos = new HashSet<StatusTarefaViewModel>();
        }
        public TarefaViewModel(tarefa entity, DateTime? dataFiltroRdo = null, DateTime? dataInicialObra = null)
        {
            if (entity != null)
            {
                Descricao = entity.tar_ds_tarefa;
                Id = entity.tar_id_tarefa;
                Comentario = entity.tar_ds_comentario;
                DataInicio = entity.tar_dt_inicio.Date.ToString().Substring(0, 10);

                DataPrevisaoFim = entity.tar_dt_previsao_fim == null ? "" : entity.tar_dt_previsao_fim.ToString().Substring(0, 10);
                //todo: construtor
                //IdObra = entity.etapa.eta_id_obra;
                IdEtapa = entity.tar_id_etapa;
                IdUnidade = entity.tar_id_unidade ?? 0;
                HorasTrabalhadas = entity.tar_nr_horas_trabalhadas ?? 0;
                QtdConstruida = entity.tar_nr_qtd_construida ?? 0;
                Status = entity.tar_id_status;
                NomeStatus = entity.status_tarefa.stt_ds_status;
                QuantidadeColaboradores = entity.obra_tarefa_colaborador.Count;
                QuantidadeEquipamentos = entity.obra_tarefa_equipamento.Count;
                PercentualConcluido = TarefaModel.CalcularPercentualConcluido(entity) > 100 ? 100 : TarefaModel.CalcularPercentualConcluido(entity);
                PercentualExtrapolado = (TarefaModel.CalcularPercentualConcluido(entity) > 100) ? "false" : "true";
                PrimeiraExecucao = TarefaModel.ObterPrimeiroDiaExecutado(entity);
                UltimaExecucao = TarefaModel.ObterUltimoDiaExecutado(entity);
                ExisteExecucao = !(TarefaModel.ObterPrimeiroDiaExecutado(entity) > DateTime.MinValue);
                listaHistoricoTarefa = new List<HistoricoTarefaViewModel>(); //PreencherHistoricoTarefa(entity);
                listaStatusPermitidos = TarefaModel.PreencherStatusTarefaPermitidos(entity);
                OrdemStatus = TarefaModel.AjustarOrdenamentoTarefas(entity.status_tarefa.stt_ds_status);
                OrdemDataInicial = entity.tar_dt_inicio.Date;
                Agrupador = entity.tar_nr_agrupador.ToString();
                ExisteRegistroMedicaoDiaAnterior = dataFiltroRdo != null && dataInicialObra != null ? TarefaModel.ExisteTarefaDiaAnterior(entity.tar_id_tarefa, entity.tar_id_etapa, dataFiltroRdo.Value.Date, entity.tar_nr_agrupador) && dataInicialObra.Value.Date <= dataFiltroRdo.Value.Date : false;
                ExisteRegistroMedicaoDataRequerida = dataFiltroRdo != null && dataInicialObra != null ? TarefaModel.ExisteTarefaDiaRequisitado(entity.tar_id_tarefa, entity.tar_id_etapa, dataFiltroRdo.Value.Date, entity.tar_nr_agrupador) && dataInicialObra.Value.Date <= dataFiltroRdo.Value.Date : false;
                ClasseStatusCss = entity.tar_id_status == 1 ? "bg-cinza" : (entity.tar_id_status == 2 ? "bg-azul" : (entity.tar_id_status == 3 ? "bg-verde" : (entity.tar_id_status == 4 ? "bg-laranja" : (entity.tar_id_status == 5 ? "bg-vermelho" : "bg-cinza"))));
                if (entity.imagem.Count > 0)
                {
                    ListaImagem = new List<ImagemViewModel>();

                    foreach (var item in entity.imagem)
                    {
                        ListaImagem.Add(new ImagemViewModel
                        {
                            idImagem = item.ima_id_imagem,
                            dsCaminho = item.ima_ds_caminho
                        });
                    }
                }
            }
        }
        internal static tarefa ViewToEntity(TarefaViewModel view)
        {
            if (view != null)
            {
                var entity = new tarefa();
                entity.tar_id_tarefa = view.Id ?? 0;
                entity.tar_ds_tarefa = view.Descricao;
                entity.tar_id_etapa = view.IdEtapa;
                entity.tar_dt_inicio = Convert.ToDateTime(view.DataInicio);
                entity.tar_dt_previsao_fim = Convert.ToDateTime(view.DataPrevisaoFim);
                entity.tar_id_status = view.Status;
                if (view.IdUnidade != 0)
                    entity.tar_id_unidade = view.IdUnidade;
                else
                    entity.tar_id_unidade = null;
                entity.tar_nr_qtd_previsao = view.qtdPlanejada;
                entity.tar_vl_valor_unitario = view.valor;
                entity.tar_id_colaborador_insercao = view.IdColaboradorInsercao;

                entity.tar_dt_ultima_atualizacao = DateTime.Now;
                entity.tar_nr_agrupador = view.Id > 0 ? Guid.Empty.ToString() : Guid.NewGuid().ToString();

                // Medição
                entity.tar_dt_medicao = view.DataMedicao ?? DateTime.Now.Date;
                entity.tar_dt_medicao_hora_inicial = view.HoraInicial;
                entity.tar_dt_medicao_hora_final = view.HoraFinal;
                entity.tar_nr_qtd_construida = view.QtdConstruida ?? 0;

                return entity;
            }
            return null;
        }
    }
}