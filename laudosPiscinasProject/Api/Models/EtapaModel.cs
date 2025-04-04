using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace laudosPiscinasProject.Api.Models
{
    public class EtapaModel
    {
        private IQueryable<etapa> Filter(EtapaViewModel filter, LaudosPiscinasEntities context)
        {
            IQueryable<etapa> query = context.etapa;
            if (filter != null)
            {
                if (filter.Id > 0)
                {
                    query = query.Where(e => e.eta_id_etapa == filter.Id);
                }
                if (!string.IsNullOrEmpty(filter.Titulo))
                {
                    query = query.Where(e => e.eta_ds_etapa.Contains(filter.Titulo));
                }

                //a consulta só pode ser feita com um IdObra
                query = query.Where(e => e.obra.obr_id_obra == filter.IdObra);


                if (!string.IsNullOrEmpty(filter.Descricao)) // Renomeado: descricao → Descricao
                {
                    query = query.Where(e => e.tarefa.All(t => t.tar_ds_tarefa.ToLower().Contains(filter.Descricao.ToLower())));

                    var tempEtapas = query.ToList();

                    foreach (var etapa in tempEtapas)
                    {
                        etapa.tarefa = etapa.tarefa.Where(x => x.tar_ds_tarefa.ToLower().Contains(filter.Descricao.ToLower())).ToList();
                    }

                    query = tempEtapas.AsQueryable();
                }
                if (filter.DataInicial > DateTime.MinValue) // Renomeado: dataInicial → DataInicial
                {

                    query = query.Where(tar => tar.tarefa.Any(t => t.tar_dt_inicio == filter.DataInicial.Date));

                    var tempEtapas = query.ToList();

                    foreach (var etapa in tempEtapas)
                    {
                        etapa.tarefa = etapa.tarefa.Where(t => t.tar_dt_inicio == filter.DataInicial.Date).ToList();
                    }

                    query = tempEtapas.AsQueryable();
                }
                if (filter.DataFinalPlanejada > DateTime.MinValue) // Renomeado: dataFinalPlanejada → DataFinalPlanejada
                {
                    query = query.Where(tar => tar.tarefa.Any(t => t.tar_dt_previsao_fim == filter.DataFinalPlanejada.Date));

                    var tempEtapas = query.ToList();

                    foreach (var etapa in tempEtapas)
                    {
                        etapa.tarefa = etapa.tarefa.Where(t => t.tar_dt_previsao_fim?.Date == filter.DataFinalPlanejada.Date)?.ToList();
                    }

                    query = tempEtapas.AsQueryable();

                }
                if (filter.DataInicialExecutada > DateTime.MinValue) // Renomeado: dataInicialExecutada → DataInicialExecutada
                {
                    List<int> idTarefas = ObterTarefasDataMedicaoInicial(filter.DataInicialExecutada, filter.IdObra);
                    query = query.Where(tar => tar.tarefa.Any(t => idTarefas.Contains(t.tar_id_tarefa)));

                    var tempEtapas = query.ToList();

                    foreach (var etapa in tempEtapas)
                    {
                        etapa.tarefa = etapa.tarefa.Where(t => idTarefas.Contains(t.tar_id_tarefa)).ToList();
                    }

                    query = tempEtapas.AsQueryable();
                }
                if (filter.DataFinalExecutada > DateTime.MinValue) // Renomeado: dataFinalExecutada → DataFinalExecutada
                {
                    query = query.Where(tar => tar.tarefa.Any(t => t.tar_dt_medicao == filter.DataFinalExecutada.Date));

                    var tempEtapas = query.ToList();

                    foreach (var etapa in tempEtapas)
                    {
                        etapa.tarefa = etapa.tarefa.Where(t => t.tar_dt_medicao == filter.DataFinalExecutada.Date).ToList();
                    }

                    query = tempEtapas.AsQueryable();
                }
                if (filter.IdStatus > 0) // Renomeado: idStatus → IdStatus
                {
                    query = query.Where(e => e.tarefa.Any(t => t.tar_id_status == filter.IdStatus));

                    var tempEtapas = query.ToList();

                    foreach (var etapa in tempEtapas)
                    {
                        etapa.tarefa = etapa.tarefa.Where(t => t.tar_id_status == filter.IdStatus).ToList();
                    }

                    query = tempEtapas.AsQueryable();
                }
                if (filter.DataMedicao != null && filter.DataMedicao != DateTime.MinValue)
                {
                    var dataMedicao = ((DateTime)filter.DataMedicao);
                    query = query.Where(e => e.tarefa.Any(t => t.tar_dt_medicao == dataMedicao));

                    var tempEtapas = query.ToList();

                    foreach (var etapa in tempEtapas)
                    {
                        etapa.tarefa = etapa.tarefa.Where(t => t.tar_dt_medicao == dataMedicao).ToList();
                    }

                    query = tempEtapas.AsQueryable();
                }
            }
            return query;
        }

        public static List<EtapaViewModel> Lista(dynamic param)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            string nome = param.titulo.ToString();
            int idObra = String.IsNullOrEmpty(param.idObra.ToString()) ? 0 : Convert.ToInt32(param.idObra);
            IQueryable<etapa> query = context.etapa.Where(e => e.obra.obr_id_obra == idObra);

            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(e => e.eta_ds_etapa.ToLower().Contains(nome.ToLower()));
            }

            List<EtapaViewModel> Lista = new List<EtapaViewModel>();
            query.ToList().ForEach(et => Lista.Add(new EtapaViewModel
            {
                Titulo = et.eta_ds_etapa,
                Ordem = et.eta_nr_orderm,
                Id = et.eta_id_etapa
            }));

            string orderby = param.orderby ?? "";
            string orderbydescending = param.orderbydescending ?? "";

            if (!string.IsNullOrEmpty(orderby))
            {
                return Lista.OrderBy(x => x.Titulo).ToList();
            }
            if (!string.IsNullOrEmpty(orderbydescending))
            {
                return Lista.OrderByDescending(x => x.Titulo).ToList();
            }

            return Lista.OrderBy(et => et.Ordem).ToList();
        }

        public static int Create(EtapaViewModel view)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                var entity = EtapaViewModel.ViewToEntity(view);
                if (ExisteEtapaComMesmaOrdem(entity))
                {
                    throw new System.Exception("Já existe uma Etapa cadastrada com o mesmo número de ordem. Por favor, verifique!");
                }
                else
                {
                    context.etapa.Add(EtapaViewModel.ViewToEntity(view));
                    var result = context.SaveChanges();
                    return result;
                }
            }
        }
        public static List<EtapaViewModel> Retrieve(EtapaViewModel filter, bool rdo = false)
        {
            var result = new List<EtapaViewModel>();

            using (var context = new LaudosPiscinasEntities())
            {
                IQueryable<etapa> query = new EtapaModel().Filter(filter, context);

                var list = query.OrderBy(x => x.eta_nr_orderm).ToList();

                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        result.Add(new EtapaViewModel(item, rdo));
                    }
                }
            }

            return result;
        }

        public static List<EtapaViewModel> ObterEtapasParaRDO(EtapaViewModel filter)
        {
            var result = new List<EtapaViewModel>();

            using (var context = new LaudosPiscinasEntities())
            {
                IQueryable<etapa> query = context.etapa.Where(et => et.eta_id_obra == filter.IdObra);
                var list = query.OrderBy(x => x.eta_nr_orderm).ToList();

                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (item.tarefa.Count > 0)
                        {
                            EtapaViewModel etapa = new EtapaViewModel();

                            etapa.Id = item.eta_id_etapa;
                            etapa.Titulo = item.eta_ds_etapa;
                            etapa.IdObra = item.obra != null ? item.obra.obr_id_obra : item.eta_id_obra;
                            etapa.TituloObra = item.obra != null ? item.obra.obr_ds_obra : "";
                            etapa.Ordem = item.eta_nr_orderm;

                            if (item.tarefa.Count > 0)
                            {
                                etapa.Tarefas = new List<TarefaViewModel>();

                                // Só carregar com status 'Pausada' e 'Em execução'
                                // WF: 06/07/2020 Lúcio pediu que só não carregasse para 'Planejada'
                               
                                foreach (var itemTar in item.tarefa.Where(l=>l.tar_id_status != 1).OrderByDescending(o => o.tar_id_tarefa).OrderByDescending(t => t.tar_dt_medicao).ThenByDescending(t => t.tar_dt_medicao_hora_inicial).GroupBy(t => t.tar_nr_agrupador).Select(t => t.FirstOrDefault()).ToList())
                                {
                                    if (itemTar.tar_id_status != 1)
                                    {
                                        TarefaViewModel newTar = new TarefaViewModel(itemTar, filter.DataMedicao, item.obra.obr_dt_inicio);
                                        newTar.ListaImagem = TarefaModel.ObterTodasImagensHistoricoMedicao(itemTar.tar_nr_agrupador, filter.DataMedicao.Value.Date);
                                        etapa.Tarefas.Add(newTar);
                                    }
                                }
                              
                                etapa.Tarefas = etapa.Tarefas.OrderBy(tar => tar.OrdemStatus).ThenBy(tar => tar.OrdemDataInicial).ToList();
                            }
                            result.Add(etapa);
                        }
                        
                    }
                }
            }

            return result.Where(et => et.Tarefas.Count() > 0).ToList();
        }
        //internal static List<EtapaViewModel> RetrieveForRDO(EtapaViewModel filter)
        //{
        //    var result = new List<EtapaViewModel>();

        //    using (var context = new LaudosPiscinasEntities())
        //    {
        //        IQueryable<etapa> query = context.etapa;

        //        query = query.Where(e => e.eta_id_obra == filter.IdObra);

        //        var dataMedicao = ((DateTime)filter.DataMedicao).Date;

        //        query = query.Where(e => e.tarefa.Any(t => t.tar_dt_medicao == dataMedicao));

        //        query = query.Where(e => e.tarefa.Any(t => t.tar_id_status == 2 || t.tar_id_status == 4));

        //        var list = query.OrderBy(x => x.eta_nr_orderm).ToList();

        //        if (list.Count > 0)
        //        {
        //            foreach (var item in list)
        //            {
        //                result.Add(new EtapaViewModel(item, true));
        //            }
        //        }
        //    }

        //    return result;
        //}
        public static int Update(EtapaViewModel view)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                var entity = context.etapa.Find(view.Id);
                entity.eta_ds_etapa = view.Titulo;

                context.etapa.Add(entity);
                context.Entry(entity).State = System.Data.EntityState.Modified;

                var result = context.SaveChanges();
                return result;
            }
        }
        public static int Delete(int id)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                var entity = context.etapa.Find(id);
                context.etapa.Remove(entity);
                var result = 0;
                try
                {
                    result = context.SaveChanges();
                }
                catch
                {
                    throw new System.Exception("Não foi possível excluir a etapa. Existem registros dependentes!");
                }

                return result;
            }
        }
        internal static EtapaViewModel CreateRetrieve(EtapaViewModel view)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                var entity = EtapaViewModel.ViewToEntity(view);

                if (ExisteEtapaComMesmaOrdem(entity))
                {
                    throw new System.Exception("Já existe uma Etapa cadastrada com o mesmo número de ordem. Por favor, verifique!");
                }
                else
                {
                    context.etapa.Add(entity);
                    context.SaveChanges();
                    var viewComplete = new EtapaViewModel(entity);
                    return viewComplete;
                }
            }
        }
        internal static bool ExisteEtapaComMesmoNome(etapa entity)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                var result = context.etapa.Any(e => e.eta_ds_etapa.Contains(entity.eta_ds_etapa) && e.eta_id_obra == entity.eta_id_obra);
                return result;
            }
        }
        internal static bool ExisteEtapaComMesmaOrdem(etapa entity)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                var result = context.etapa.Any(e => e.eta_nr_orderm == entity.eta_nr_orderm && e.eta_id_obra == entity.eta_id_obra && e.eta_id_etapa != entity.eta_id_etapa);
                return result;
            }
        }

        internal static EtapaViewModel ObterEtapa(int id)
        {
            EtapaViewModel etapa = new EtapaViewModel();
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            etapa mirror = context.etapa.FirstOrDefault(et => et.eta_id_etapa == id);
            etapa.Id = id;
            etapa.IdObra = mirror.eta_id_obra;
            etapa.Titulo = mirror.eta_ds_etapa;
            etapa.Ordem = mirror.eta_nr_orderm;

            return etapa;
        }

        internal static int AtualizarEtapa(EtapaViewModel etapa)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            etapa eta = context.etapa.FirstOrDefault(et => et.eta_id_etapa == etapa.Id);
            eta.eta_ds_etapa = etapa.Titulo;
            eta.eta_nr_orderm = etapa.Ordem;
            if (ExisteEtapaComMesmaOrdem(eta))
            {
                throw new System.Exception("Já existe uma Etapa cadastrada com o mesmo número de ordem. Por favor, verifique!");
            }

            var result = context.SaveChanges();
            return result;
        }
        internal static List<int> ObterTarefasDataMedicaoInicial(DateTime dataInicial, int idObra)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            List<Guid> agrupador = new List<Guid>();
            List<int> idsTarefas = new List<int>();
            if (dataInicial != null && dataInicial != DateTime.MinValue)
            {
                agrupador 
                    = context.tarefa.Where(t => t.etapa.eta_id_obra == idObra).OrderBy(t => t.tar_dt_medicao).GroupBy(t => t.tar_nr_agrupador).Select(t => t.FirstOrDefault()).Where(t => t.tar_dt_medicao == dataInicial.Date).Select(t => new Guid(t.tar_nr_agrupador)).ToList();
            }
            idsTarefas = context.tarefa.Where(t => t.etapa.eta_id_obra == idObra && agrupador.Contains(new Guid(t.tar_nr_agrupador))).OrderByDescending(t => t.tar_dt_medicao).Select(t => t.tar_id_tarefa).ToList();

            return idsTarefas;
        }
        
        //internal static List<int> ObterTarefasDataMedicaoFinal(DateTime dataInicial, int idObra)
        //{
        //    LaudosPiscinasEntities context = new LaudosPiscinasEntities();
        //    List<int> idsTarefas = new List<int>();
        //    return context.tarefa.Where(t => t.etapa.eta_id_obra == idObra).OrderByDescending(t => t.tar_dt_medicao).ThenByDescending(t => t.tar_dt_medicao_hora_inicial).GroupBy(t => t.tar_nr_agrupador).Select(t => t.FirstOrDefault()).Select(t => t.tar_id_tarefa).ToList();
        //}

        public static List<EtapaViewModel> ObterEtapaTarefa(EtapaViewModel filter)
        {
            var context = new LaudosPiscinasEntities();

            IQueryable<etapa> query = context.etapa;

            query = query.Where(e => e.eta_id_obra == filter.IdObra);

            if (filter.Id > 0)
            {
                query = query.Where(e => e.eta_id_etapa == filter.Id);
            }

            var listTemp = query.OrderBy(x => x.eta_nr_orderm).ToList();

            var list = new List<EtapaViewModel>();

            foreach (var item in listTemp)
            {
                var listTarefa = new List<TarefaViewModel>();
                var tarefas = item.tarefa;

                // Correção do InvalidCastException (item.tarefa)
                if (item.tarefa != null)
                {
                    try
                    {
                        if (filter.DataInicial > DateTime.MinValue) // Renomeado: dataInicial → DataInicial
                        {
                            if (filter.DataFinalPlanejada > DateTime.MinValue) // Renomeado: dataFinalPlanejada → DataFinalPlanejada
                            {
                                tarefas = tarefas.Where(t => t.tar_dt_inicio >= filter.DataInicial.Date && t.tar_dt_previsao_fim <= filter.DataFinalPlanejada).ToList();
                            }
                            else
                            {
                                tarefas = tarefas.Where(t => t.tar_dt_inicio == filter.DataInicial.Date).ToList();
                            }
                        }
                        else if (filter.DataFinalPlanejada > DateTime.MinValue) // Renomeado: dataFinalPlanejada → DataFinalPlanejada
                        {
                            tarefas = tarefas.Where(t => t.tar_dt_previsao_fim == filter.DataFinalPlanejada.Date).ToList();
                        }

                        if (filter.DataInicialExecutada > DateTime.MinValue) // Renomeado: dataInicialExecutada → DataInicialExecutada
                        {
                            if (filter.DataFinalExecutada > DateTime.MinValue) // Renomeado: dataFinalExecutada → DataFinalExecutada
                            {
                                tarefas = tarefas.Where(t => t.tar_dt_medicao >= filter.DataInicialExecutada.Date && t.tar_dt_medicao <= filter.DataFinalExecutada).ToList();
                            }
                            else
                            {
                                tarefas = tarefas.Where(t => t.tar_dt_medicao == filter.DataInicialExecutada.Date).ToList();
                            }
                        }
                        else if (filter.DataFinalExecutada > DateTime.MinValue) // Renomeado: dataFinalExecutada → DataFinalExecutada
                        {
                            tarefas = tarefas.Where(t => t.tar_dt_medicao == filter.DataFinalExecutada.Date).ToList();
                        }

                        // aqui limitamos a apresentação para mostrar somente o status atual
                        // perceba que fizemos isso depois os filtros de data, pois precisamos filtrar antes por data de planejamento
                        tarefas = tarefas.OrderByDescending(t => t.tar_id_tarefa).ThenByDescending(t => t.tar_dt_medicao).ThenByDescending(t => t.tar_dt_medicao_hora_final).GroupBy(t => t.tar_nr_agrupador).Select(t => t.FirstOrDefault()).ToList();

                        if (!string.IsNullOrEmpty(filter.Descricao)) // Renomeado: descricao → Descricao
                        {
                            tarefas = tarefas.Where(t => t.tar_ds_tarefa.ToLower().Contains(filter.Descricao.ToLower())).ToList();
                        }

                        if (filter.IdStatus > 0) // Renomeado: idStatus → IdStatus
                        {
                            tarefas = tarefas.Where(t => t.tar_id_status == filter.IdStatus).ToList();
                        }

                        if (tarefas.Count > 0)
                        {
                            foreach (var t in tarefas)
                            {
                                var percentualConcluido = TarefaModel.CalcularPercentualConcluido(t);

                                listTarefa.Add(new TarefaViewModel
                                {
                                    Id = t.tar_id_tarefa,
                                    Descricao = t.tar_ds_tarefa,
                                    NomeStatus = t.status_tarefa.stt_ds_status,
                                    DataInicio = t.tar_dt_inicio.Date.ToString().Substring(0, 10),
                                    DataPrevisaoFim = t.tar_dt_previsao_fim == null ? "" : t.tar_dt_previsao_fim.ToString().Substring(0, 10),
                                    PrimeiraExecucao = TarefaModel._ObterPrimeiroDiaExecutado(t.tar_nr_agrupador),
                                    UltimaExecucao = TarefaModel._ObterUltimoDiaExecutado(t.tar_nr_agrupador),
                                    QuantidadeColaboradores = t.obra_tarefa_colaborador.Count,
                                    QuantidadeEquipamentos = t.obra_tarefa_equipamento.Count,
                                    OrdemStatus = TarefaModel.AjustarOrdenamentoTarefas(t.status_tarefa.stt_ds_status),
                                    OrdemDataInicial = t.tar_dt_inicio,
                                    PercentualConcluido = percentualConcluido > 100 ? 100 : percentualConcluido,
                                    PercentualExtrapolado = (percentualConcluido > 100) ? "false" : "true",
                                    ClasseStatusCss = t.tar_id_status == 1 ? "bg-cinza" : (t.tar_id_status == 2 ? "bg-azul" : (t.tar_id_status == 3 ? "bg-verde" : (t.tar_id_status == 4 ? "bg-laranja" : (t.tar_id_status == 5 ? "bg-vermelho" : "bg-cinza")))),
                                    ExisteExecucao = t.tar_id_status == 1,
                                    listaStatusPermitidos = TarefaModel.PreencherStatusTarefaPermitidos(t)
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log do erro
                        Console.WriteLine($"Erro ao processar item.tarefa: {ex.Message}");
                    }
                }

                list.Add(new EtapaViewModel
                {
                    Id = item.eta_id_etapa,
                    Titulo = item.eta_ds_etapa,
                    Tarefas = listTarefa.OrderBy(tar => tar.OrdemStatus).ThenBy(tar => tar.OrdemDataInicial).ThenBy(tar => tar.Descricao).ToList()
                });
            }

            return list;
        }
    }
    public class EtapaViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Ordem { get; set; }
        public int IdObra { get; set; }
        public string TituloObra { get; set; }

        #region Campos da Tarefa
        /// <summary>
        /// Descrição da Tarefa
        /// </summary>
        public string Descricao { get; set; }  // Renomeado: descricao → Descricao
        /// <summary>
        /// Data de Inicial da Tarefa
        /// </summary>
        public DateTime DataInicial { get; set; }  // Renomeado: dataInicial → DataInicial
        /// <summary>
        /// Data Final Planejada da Tarefa
        /// </summary>
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataFinalPlanejada { get; set; }  // Renomeado: dataFinalPlanejada → DataFinalPlanejada
        /// <summary>
        /// Data Inicial da Tarefa
        /// </summary>
        public DateTime DataInicialExecutada { get; set; }  // Renomeado: dataInicialExecutada → DataInicialExecutada
        /// <summary>
        /// Data Final Executada da Tarefa
        /// </summary>
        public DateTime DataFinalExecutada { get; set; }  // Renomeado: dataFinalExecutada → DataFinalExecutada
        /// <summary>
        /// Id do Status da Tarefa
        /// </summary>
        public int IdStatus { get; set; }  // Renomeado: idStatus → IdStatus
        /// <summary>
        /// Campo para ser utilizado para filtrar a listagem de tarefas no RDO
        /// </summary>
        public DateTime? DataMedicao { get; set; }
        #endregion Campos da Tarefa       

        public ObraViewModel Obra { get; set; }
        public List<TarefaViewModel> Tarefas { get; set; }
        public EtapaViewModel() { }
        public EtapaViewModel(etapa entity, bool rdo = false)
        {
            if (entity != null)
            {
                Id = entity.eta_id_etapa;
                Titulo = entity.eta_ds_etapa;
                IdObra = entity.obra != null ? entity.obra.obr_id_obra : entity.eta_id_obra;
                TituloObra = entity.obra != null ? entity.obra.obr_ds_obra : "";
                Ordem = entity.eta_nr_orderm;

                if (entity.tarefa.Count > 0)
                {
                    Tarefas = new List<TarefaViewModel>();

                    // Se consulta vier da tela de RDO, só carregar com status 'Pausada' e 'Em execução'
                    if (rdo)
                    {
                        foreach (var item in entity.tarefa.Where(t => t.tar_id_status == 2 || t.tar_id_status == 4).OrderByDescending(t => t.tar_dt_medicao).ThenByDescending(t => t.tar_dt_medicao_hora_inicial).GroupBy(t => t.tar_nr_agrupador).Select(t => t.FirstOrDefault()).ToList())
                        {
                            Tarefas.Add(new TarefaViewModel(item));
                        }
                    }
                    else
                    {
                        foreach (var item in entity.tarefa.OrderByDescending(t => t.tar_dt_medicao).ThenByDescending(t => t.tar_dt_medicao_hora_inicial).GroupBy(t => t.tar_nr_agrupador).Select(t => t.FirstOrDefault()).ToList())
                        {
                            Tarefas.Add(new TarefaViewModel(item));
                        }
                    }
                    Tarefas = Tarefas.OrderBy(tar => tar.OrdemStatus).ThenBy(tar => tar.OrdemDataInicial).ToList();
                }
            }
        }
        internal static etapa ViewToEntity(EtapaViewModel view)
        {
            if (view != null)
            {
                var entity = new etapa();
                entity.eta_id_etapa = view.Id;
                entity.eta_ds_etapa = view.Titulo;
                entity.eta_nr_orderm = view.Ordem;
                entity.eta_id_obra = view.IdOb
                entity.eta_id_etapa = view.Id;
                entity.eta_ds_etapa = view.Titulo;
                entity.eta_nr_orderm = view.Ordem;
                entity.eta_id_obra = view.IdObra;
                return entity;
            }
            return null;
        }
    }
}