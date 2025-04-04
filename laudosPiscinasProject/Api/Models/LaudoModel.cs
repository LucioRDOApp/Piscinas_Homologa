using LaudosPiscinasClass;
//using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Microsoft.Reporting.WinForms;

namespace laudosPiscinasProject.Api.Models
{
    public class LaudoModel
    {
        private static CultureInfo cultureInfo = new CultureInfo("pt-BR");

        public static List<LaudoViewModel> DashboardGrafico(dynamic param)
        {
            int idObra = param.unidadeEscolar ?? 0;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            List<laudo> query = context.Set<laudo>().ToList();

            if(idObra != 0)
                query = query.Where(laudo => laudo.laudo_id_obra == idObra).ToList();

            DateTime dataInicial = String.IsNullOrEmpty(Convert.ToString(param.dataInicial)) ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataInicial));
            DateTime dataFinal = String.IsNullOrEmpty(Convert.ToString(param.dataFinal)) ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataFinal));

            if (dataInicial > DateTime.MinValue)
            {
                query = query.Where(laudo => laudo.laudo_dt_laudo >= dataInicial).ToList();
            }
            if (dataFinal > DateTime.MinValue)
            {
                query = query.Where(laudo => laudo.laudo_dt_laudo <= dataFinal).ToList();
            }

            List<LaudoViewModel> Lista = new List<LaudoViewModel>();
            query.OrderByDescending(x => x.laudo_id_laudo).ToList().ForEach(laudo => Lista.Add(new LaudoViewModel
            {
                laudo_dt_laudo = laudo.laudo_dt_laudo == null || laudo.laudo_dt_laudo == DateTime.MinValue ? "" : laudo.laudo_dt_laudo.Date.ToString().Substring(0, 10),
                laudo_id_laudo = laudo.laudo_id_laudo,
                laudo_id_status = laudo.laudo_id_status,
                laudo_ds_comentario_assinatura = laudo.laudo_ds_comentario_assinatura,


                laudo_tp_nivel_cloro = (bool)laudo.laudo_tp_nivel_cloro,
                laudo_tp_ph = (bool)laudo.laudo_tp_ph,
                laudo_tp_limpidez = (bool)laudo.laudo_tp_limpidez,
                laudo_tp_superficie = (bool)laudo.laudo_tp_superficie,
                laudo_tp_fundo = (bool)laudo.laudo_tp_fundo,
                laudo_tp_nivel_cloro_2 = (bool)laudo.laudo_tp_nivel_cloro_2,
                laudo_tp_nivel_bacterias = (bool)laudo.laudo_tp_nivel_bacterias,
                laudo_tp_nivel_proliferacao = (bool)laudo.laudo_tp_nivel_proliferacao,

                DiaDaSemana = cultureInfo.DateTimeFormat.DayNames[(int)laudo.laudo_dt_laudo.Date.DayOfWeek],
                DescricaoStatus = laudo.status_rdo.str_ds_status
            }));


            return Lista;
        }

        public static List<LaudoViewModel> Lista(dynamic param)
        {
            int idObra = param.idObra ?? 0;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            List<laudo> query = context.Set<laudo>().ToList();

            query = query.Where(rdo => rdo.laudo_id_obra == idObra).ToList();

            int statusRdo = param.statusRdo ?? 0;
            DateTime dataInicial = String.IsNullOrEmpty(Convert.ToString(param.dataInicial)) ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataInicial));
            DateTime dataFinal = String.IsNullOrEmpty(Convert.ToString(param.dataFinal)) ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataFinal));

            if (statusRdo > 0)
            {
                query = query.Where(rdo => rdo.laudo_id_status == statusRdo).ToList();
            }
            if (dataInicial > DateTime.MinValue)
            {
                query = query.Where(rdo => rdo.laudo_dt_laudo >= dataInicial).ToList();
            }
            if (dataFinal > DateTime.MinValue)
            {
                query = query.Where(rdo => rdo.laudo_dt_laudo <= dataFinal).ToList();
            }

            List<LaudoViewModel> Lista = new List<LaudoViewModel>();
            query.OrderByDescending(x => x.laudo_id_laudo).ToList().ForEach(laudo => Lista.Add(new LaudoViewModel
            {
                laudo_dt_laudo = laudo.laudo_dt_laudo == null || laudo.laudo_dt_laudo == DateTime.MinValue ? "" : laudo.laudo_dt_laudo.Date.ToString().Substring(0, 10),
                laudo_id_laudo = laudo.laudo_id_laudo,
                laudo_id_status = laudo.laudo_id_status,
                laudo_ds_comentario_assinatura = laudo.laudo_ds_comentario_assinatura,


                laudo_tp_nivel_cloro = (bool)laudo.laudo_tp_nivel_cloro,
                laudo_tp_ph = (bool)laudo.laudo_tp_ph,
                laudo_tp_limpidez = (bool)laudo.laudo_tp_limpidez,
                laudo_tp_superficie = (bool)laudo.laudo_tp_superficie,
                laudo_tp_fundo = (bool)laudo.laudo_tp_fundo,
                laudo_tp_nivel_cloro_2 = (bool)laudo.laudo_tp_nivel_cloro_2,
                laudo_tp_nivel_bacterias = (bool)laudo.laudo_tp_nivel_bacterias,
                laudo_tp_nivel_proliferacao = (bool)laudo.laudo_tp_nivel_proliferacao,

                DiaDaSemana = cultureInfo.DateTimeFormat.DayNames[(int)laudo.laudo_dt_laudo.Date.DayOfWeek],
                DescricaoStatus = laudo.status_rdo.str_ds_status
                //rdo.rdo_dt_rdo.Date.DayOfWeek.ToString(),

                //QtdTarefas = rdo.historico_tarefa_rdo.Count,
                //QtdTarefas = laudo.laudo_tarefa.GroupBy(et => et.tarefa.tar_nr_agrupador).ToList().Count,
                //QtdMaquinas = ObterQuantidadeMaquinas(laudo),
                //QtdColaboradores = ObterQuantidadeColaboradores(laudo),
                //laudo_st. = laudo.colaborador.obra_colaborador.FirstOrDefault(col => col.oco_id_obra == laudo.laudo_id_obra).oco_st_contratante_contratada,
                //status_rdo = laudo.status_rdo.str_ds_status

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

        public static LaudoViewModel Salvar(dynamic param)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            if (string.IsNullOrEmpty(param.dataLaudo.ToString()))
            {
                throw new Exception("A data deve ser preenchida");
            }

            int idColaborador = param.idColaborador ?? 0;
            int idObra = param.idObra ?? 0;
            DateTime dataLaudo = Convert.ToDateTime(param.dataLaudo.ToString());
            obra _obra = context.obra.FirstOrDefault(x => x.obr_id_obra == idObra);

            if (_obra != null && dataLaudo < _obra.obr_dt_inicio)
            {
                throw new Exception("Não é possível gerar um laudo anterior a data inicial da unidade escolar.");
            }
            if (_obra != null && _obra.obr_dt_fim != null && _obra.obr_dt_fim != DateTime.MinValue && dataLaudo > _obra.obr_dt_fim)
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

            laudo _laudo = context.laudo.Where(x => x.laudo_dt_laudo == dataLaudo && x.laudo_id_obra == idObra).FirstOrDefault() ?? new laudo();

            if (_laudo.laudo_id_laudo > 0 && _laudo.status_rdo != null && (_laudo.status_rdo.str_ds_status.ToLower().Contains("assinado")))
            {
                throw new Exception("Não é possível sobrescrever um Laudo que já foi assinado.");
            }

            // WF 10/07/2020 - Lúcio solicitou que retirasse esta validação
            // if (!(bool)param.existeTarefasComHistoricos && !(bool)param.existeTarefasSemHistoricos)
            // {
            //     throw new Exception("Não existem registros para serem gerados no relatório.");
            // }

            _laudo.laudo_dt_laudo = Convert.ToDateTime(dataLaudo);
            _laudo.laudo_id_status = 1;

            _laudo.laudo_tp_nivel_cloro = param.nivelCloro == null ? false : param.nivelCloro;
            _laudo.laudo_tp_ph = param.ph == null ? false : param.ph;
            _laudo.laudo_tp_limpidez = param.limpidez == null ? false : param.limpidez;
            _laudo.laudo_tp_superficie = param.superficie == null ? false : param.superficie;
            _laudo.laudo_tp_fundo = param.fundo == null ? false : param.fundo;
            _laudo.laudo_tp_nivel_cloro_2 = param.nivelCloro2 == null ? false : param.nivelCloro2;
            _laudo.laudo_tp_nivel_bacterias = param.bacterias == null ? false : param.bacterias;
            _laudo.laudo_tp_nivel_proliferacao = param.proliferacao == null ? false : param.proliferacao;

            _laudo.laudo_ds_comentario_geracao = param.comentario;
            _laudo.laudo_tp_comentario_geracao = param.tipoComentarioGeracao == 0 ? null : param.tipoComentarioGeracao == 1 ? "P" : "N"; // Like/Deslike (P/N)

            _laudo.laudo_id_obra = idObra;
            _laudo.laudo_dt_geracao = DateTime.Now;

            if (context.laudo.ToList().Any(x => x.laudo_id_obra == idObra && x.laudo_dt_laudo == _laudo.laudo_dt_laudo))
            {
                context.laudo.Attach(_laudo);
                context.Entry(_laudo).State = EntityState.Modified;
            }
            else
            {
                _laudo.laudo_id_colaborador = idColaborador; //Só adicionar o colaborador no cadastro do rdo, na edição permanecer o usuário que criou.
                context.laudo.Add(_laudo);
            }

            bool result = context.SaveChanges() > 0;
            if (param.listaImagens != null)
            {
                IncluirImagens(param.listaImagens, _laudo, context);
            }

            return PreencherViewModel(_laudo, param.listaImagens != null);
        }

        private static void IncluirImagens(dynamic listaImagems, laudo laudo, LaudosPiscinasEntities context)
        {
            LaudosPiscinasEntities rdoContext = new LaudosPiscinasEntities();
            List<rdo_imagem> rdoImagens = rdoContext.rdo_imagem.Where(ri => ri.rim_id_rdo == laudo.laudo_id_laudo).ToList();

            if (rdoImagens.Count > 0)
            {
                foreach (var item in rdoImagens)
                {
                    rdoContext.rdo_imagem.Remove(item);

                    imagem i = rdoContext.imagem.Find(item.rim_id_imagem);

                    rdoContext.imagem.Remove(i);
                }

                rdoContext.SaveChanges();
            }

            using (context = new LaudosPiscinasEntities())
            {
                foreach (var item in listaImagems)
                {
                    SaveImages(item, context, laudo);

                    var _idImagem = context.imagem
                        .Where(x => x.ima_id_tarefa == laudo.laudo_id_laudo)
                        .OrderByDescending(x => x.ima_id_imagem)
                        .Select(x => x.ima_id_imagem)
                        .FirstOrDefault();

                    var rDOImagem = new rdo_imagem();
                    rDOImagem.rim_id_rdo = laudo.laudo_id_laudo;
                    rDOImagem.rim_id_imagem = _idImagem;

                    context.rdo_imagem.Add(rDOImagem);
                }
                // atualiza imagem para que ela não possa ser
                // excluída do sistema
                //var imagem = context.imagem.Find(_idImagem);
                //imagem.ima_id_historico_tarefa_rdo = laudo.laudo_id_laudo;
                //context.Entry(imagem).State = EntityState.Modified;
                context.SaveChanges();
            }
            
        }

        private static void SaveImages(dynamic item, LaudosPiscinasEntities context, laudo entity)
        {
            var caminhoRelativo = "/uploads/tarefa/" + entity.laudo_id_laudo;
            var caminhoAbsoluto = HostingEnvironment.ApplicationPhysicalPath + caminhoRelativo;
            if (!Directory.Exists(caminhoAbsoluto)) { Directory.CreateDirectory(caminhoAbsoluto); }
            
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
                        ima_id_tarefa = entity.laudo_id_laudo
                    });

                    context.SaveChanges();
                }
            }
        }


        public static LaudoViewModel PreencherViewModel(laudo _laudo, bool relatorioFotografico)
        {
            LaudoViewModel returnObj = new LaudoViewModel();
            returnObj.laudo_id_laudo = _laudo.laudo_id_laudo;
            returnObj.laudo_id_obra = _laudo.laudo_id_obra;
            returnObj.gerarRelatorioFotografico = relatorioFotografico;
            returnObj.laudo_dt_laudo = _laudo.laudo_dt_laudo.Date.ToString().Substring(0, 10);
            return returnObj;
        }
        public static byte[] GerarDocumentoRdo(int idRdo, bool gerarRelatorioFotografico = false)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            laudo _rdo = context.laudo.FirstOrDefault(x => x.laudo_id_laudo == idRdo);
            // List<rdo_tarefa> listaHistoricoTarefas = _rdo.rdo_tarefa.ToList();

            List<equipamento> listaEquipamentos = new List<equipamento>();
            List<tarefa> listaTarefas = new List<tarefa>();
            List<cargo> listaCargos = new List<cargo>();
            List<obra_tarefa_colaborador> listaTarefasColaboradores = new List<obra_tarefa_colaborador>();
            List<acidente> listaAcidentesAux = context.acidente.ToList() ?? new List<acidente>();
            List<acidente> listaAcidentes = new List<acidente>();

            //foreach (var item in _rdo.obra.etapa)
            //{
            //    foreach (tarefa tar in item.tarefa)
            //    {
            //        foreach (acidente aci in listaAcidentesAux)
            //        {
            //            DateTime dt = aci.aci_dt_data_hora ?? DateTime.MinValue;
            //            if (dt.ToString("dd/MM/yyyy") == _rdo.laudo_dt_laudo.ToString("dd/MM/yyyy"))
            //            {
            //                if (tar.tar_id_tarefa == aci.aci_id_tarefa)
            //                {
            //                    listaAcidentes.Add(aci);
            //                }
            //            }
            //        }
            //    }
            //}

            //foreach (rdo_tarefa tarefa in _rdo.laudo_tarefa)
            //{
            //    tarefa.tarefa.obra_tarefa_equipamento.ToList().ForEach(x => {
            //        var equipamento = ObterEquipamento(x.ote_id_obra_equipamento);
            //        if (listaEquipamentos.Count(c => c.equ_id_equipamento == equipamento.equ_id_equipamento) == 0) {
            //            listaEquipamentos.Add(equipamento);
            //        }
            //    });


            //    listaTarefas.Add(tarefa.tarefa);

            //    foreach (obra_tarefa_colaborador item in tarefa.tarefa.obra_tarefa_colaborador)
            //    {
            //        if (listaTarefasColaboradores.Where(tc => tc.obra_colaborador.colaborador.col_nm_colaborador == item.obra_colaborador.colaborador.col_nm_colaborador && tc.obra_colaborador.oco_id_cargo == item.obra_colaborador.oco_id_cargo).Count() == 0)
            //        {
            //            cargo cargo = ObterObraColaborador(item.otc_id_obra_colaborador).cargo;
            //            listaTarefasColaboradores.Add(item);
            //        }

            //        //listaCargos.Add(cargo);

            //    }
            //}

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



            DataTable dtAcidentes = new DataTable();
            dtAcidentes.Columns.Add("aci_ds_acidente");
            dtAcidentes.Columns.Add("aci_st_afastamento");

            DataTable dtItensLaudo = new DataTable();
            dtItensLaudo.Columns.Add("laudo_tp_nivel_cloro");
            dtItensLaudo.Columns.Add("laudo_tp_ph");
            dtItensLaudo.Columns.Add("laudo_tp_limpidez");
            dtItensLaudo.Columns.Add("laudo_tp_superficie");
            dtItensLaudo.Columns.Add("laudo_tp_fundo");
            dtItensLaudo.Columns.Add("laudo_tp_nivel_cloro_2");
            dtItensLaudo.Columns.Add("laudo_tp_nivel_bacterias");
            dtItensLaudo.Columns.Add("laudo_tp_nivel_proliferacao");

            var nivelCloro = _rdo.laudo_tp_nivel_cloro == true ? "Sim" : "Não";
            var ph = _rdo.laudo_tp_ph == true ? "Sim" : "Não";
            var limpidez = _rdo.laudo_tp_limpidez == true ? "Sim" : "Não";
            var superficie = _rdo.laudo_tp_superficie == true ? "Sim" : "Não";
            var fundo = _rdo.laudo_tp_fundo == true ? "Sim" : "Não";
            var nivelCloro2 = _rdo.laudo_tp_nivel_cloro_2 == true ? "Sim" : "Não";
            var bacterias = _rdo.laudo_tp_nivel_bacterias == true ? "Sim" : "Não";
            var proliferacao = _rdo.laudo_tp_nivel_bacterias == true ? "Sim" : "Não";

            dtItensLaudo.Rows.Add(nivelCloro, ph, limpidez, superficie, fundo, nivelCloro2, bacterias, proliferacao);

            DataTable dtAssinaturaContratante = new DataTable();
            dtAssinaturaContratante.Columns.Add("responsavel_assinatura");
            dtAssinaturaContratante.Columns.Add("cpf");
            dtAssinaturaContratante.Columns.Add("data_hora");
            dtAssinaturaContratante.Columns.Add("cargo");
            dtAssinaturaContratante.Columns.Add("ip");


            //_rdo
            assinatura_rdo assinaturaContratante = context.assinatura_rdo.FirstOrDefault(x => x.ass_id_rdo == _rdo.laudo_id_laudo && x.obra_colaborador.grupo.gru_nm_nome.ToLower().Contains("contratante")) ?? new assinatura_rdo();
            if (assinaturaContratante.ass_id_obra_colaborador_assinante > 0)
            {
                dtAssinaturaContratante.Rows.Add(assinaturaContratante.obra_colaborador.colaborador.col_nm_colaborador, assinaturaContratante.obra_colaborador.colaborador.col_nr_cpf, assinaturaContratante.ass_dt_assinatura, assinaturaContratante.obra_colaborador.cargo.car_ds_cargo, assinaturaContratante.ass_ds_ip);
            }
            else
            {
                dtAssinaturaContratante.Rows.Add("Não assinado", "Não assinado", "Não assinado", "Não assinado");
            }




            DataTable dtAssinaturaContratada = new DataTable();
            dtAssinaturaContratada.Columns.Add("responsavel_assinatura");
            dtAssinaturaContratada.Columns.Add("cpf");
            dtAssinaturaContratada.Columns.Add("data_hora");
            dtAssinaturaContratada.Columns.Add("cargo");
            dtAssinaturaContratada.Columns.Add("ip");


            //_rdo
            assinatura_rdo assinaturaContratada = context.assinatura_rdo.FirstOrDefault(x => x.ass_id_rdo == _rdo.laudo_id_laudo && x.obra_colaborador.grupo.gru_nm_nome.ToLower().Contains("contratada")) ?? new assinatura_rdo();
            if (assinaturaContratada.ass_id_obra_colaborador_assinante > 0)
            {
                dtAssinaturaContratada.Rows.Add(assinaturaContratada.obra_colaborador.colaborador.col_nm_colaborador, assinaturaContratada.obra_colaborador.colaborador.col_nr_cpf, assinaturaContratada.ass_dt_assinatura, assinaturaContratada.obra_colaborador.cargo.car_ds_cargo, assinaturaContratada.ass_ds_ip);
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
            dtImagem.Columns.Add("imagem", typeof(byte[]));
            dtImagem.Columns.Add("idImagem");
            dtImagem.Columns.Add("imagem1", typeof(byte[]));
            dtImagem.Columns.Add("idImagem1");
            dtImagem.Columns.Add("imagem2", typeof(byte[]));
            dtImagem.Columns.Add("idImagem2");
            dtImagem.Columns.Add("imagem3", typeof(byte[]));
            dtImagem.Columns.Add("idImagem3");



            rdo_imagem linha = new rdo_imagem();
            //int count = 0;

            var rdo_imagem = context.imagem.Where(x => x.ima_id_tarefa == _rdo.laudo_id_laudo).ToList();

            if (rdo_imagem.Count > 0)
                gerarRelatorioFotografico = true;

            for (int i = 0; i < rdo_imagem.Count(); i += 4)
            {
                List<imagem> imagens = rdo_imagem.Skip(i).Take(4).ToList();

                if (imagens.Count < 4)
                {
                    int imagensFaltantes = 4 - imagens.Count;
                    for (int j = 0; j < imagensFaltantes; j++)
                    {
                        imagens.Add(new imagem());
                    }
                }

                dtImagem.Rows.Add(
                   convertToBytes(HostingEnvironment.ApplicationPhysicalPath + imagens[0].ima_ds_caminho),
                   imagens[0].ima_id_imagem,
                   convertToBytes(HostingEnvironment.ApplicationPhysicalPath + imagens[1].ima_ds_caminho),
                   imagens[1].ima_id_imagem,
                   convertToBytes(HostingEnvironment.ApplicationPhysicalPath + imagens[2].ima_ds_caminho),
                   imagens[2].ima_id_imagem,
                   convertToBytes(HostingEnvironment.ApplicationPhysicalPath + imagens[3].ima_ds_caminho),
                   imagens[3].ima_id_imagem);
            }

            //foreach (var item in rdo_imagem)
            //{
               

                //if (count % 2 == 1)
                //{
                //    dtImagem.Rows.Add(convertToBytes(HostingEnvironment.ApplicationPhysicalPath + item.imagem.ima_ds_caminho), item.rim_id_imagem, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linha.imagem.ima_ds_caminho), linha.rim_id_imagem);
                //    linha = null;
                //}
                //else
                //{
                //    linha = item;
                //}

                //if (_rdo.rdo_imagem.Count == (count - 1))
                //{
                //    dtImagem.Rows.Add(convertToBytes(HostingEnvironment.ApplicationPhysicalPath + item.imagem.ima_ds_caminho), item.rim_id_imagem);

                //}
                //count++;
            //}
            //try
            //{
            //    List<rdo_imagem> linhaLista = _rdo.rdo_imagem.OrderByDescending(rdi => rdi.imagem.ima_id_imagem).ToList();
            //    string nomeTarefa = "";

            //    while (count < linhaLista.Count())
            //    {
            //        int dif = linhaLista.Count() - count;
            //        nomeTarefa = linhaLista[count].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count].imagem.tarefa.tar_ds_tarefa;
            //        if (dif >= 4 && nomeTarefa == linhaLista[count].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count].imagem.tarefa.tar_ds_tarefa && nomeTarefa == linhaLista[count + 1].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 1].imagem.tarefa.tar_ds_tarefa && nomeTarefa == linhaLista[count + 2].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 2].imagem.tarefa.tar_ds_tarefa && nomeTarefa == linhaLista[count + 3].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 3].imagem.tarefa.tar_ds_tarefa)
            //        {
            //            dif = 4;
            //        }

            //        else if (dif >= 3 && nomeTarefa == linhaLista[count].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count].imagem.tarefa.tar_ds_tarefa && nomeTarefa == linhaLista[count + 1].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 1].imagem.tarefa.tar_ds_tarefa && nomeTarefa == linhaLista[count + 2].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 2].imagem.tarefa.tar_ds_tarefa)
            //        {
            //            dif = 3;
            //        }

            //        else if (dif >= 2 && nomeTarefa == linhaLista[count].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count].imagem.tarefa.tar_ds_tarefa && nomeTarefa == linhaLista[count + 1].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 1].imagem.tarefa.tar_ds_tarefa)
            //        {
            //            dif = 2;
            //        }

            //        else if (dif >= 1 && nomeTarefa == linhaLista[count].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count].imagem.tarefa.tar_ds_tarefa)
            //        {
            //            dif = 1;
            //        }

            //        switch (dif)
            //        {
            //            case 3:
            //                dtImagem.Rows.Add(nomeTarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count].imagem.ima_ds_caminho), linhaLista[count].rim_id_imagem,
            //               linhaLista[count + 1].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 1].imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count + 1].imagem.ima_ds_caminho), linhaLista[count + 1].rim_id_imagem,
            //               linhaLista[count + 2].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 2].imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count + 2].imagem.ima_ds_caminho), linhaLista[count + 2].rim_id_imagem);
            //                count += 3;
            //                break;
            //            case 2:
            //                dtImagem.Rows.Add(nomeTarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count].imagem.ima_ds_caminho), linhaLista[count].rim_id_imagem,
            //                linhaLista[count + 1].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 1].imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count + 1].imagem.ima_ds_caminho), linhaLista[count + 1].rim_id_imagem);
            //                count += 2;
            //                break;
            //            case 1:
            //                dtImagem.Rows.Add(nomeTarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count].imagem.ima_ds_caminho), linhaLista[count].rim_id_imagem);
            //                count += 1;
            //                break;
            //            default:
            //                dtImagem.Rows.Add(nomeTarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count].imagem.ima_ds_caminho), linhaLista[count].rim_id_imagem,
            //                linhaLista[count + 1].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 1].imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count + 1].imagem.ima_ds_caminho), linhaLista[count + 1].rim_id_imagem,
            //                linhaLista[count + 2].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 2].imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count + 2].imagem.ima_ds_caminho), linhaLista[count + 2].rim_id_imagem,
            //                linhaLista[count + 3].imagem.tarefa.etapa.eta_ds_etapa + " / " + linhaLista[count + 3].imagem.tarefa.tar_ds_tarefa, convertToBytes(HostingEnvironment.ApplicationPhysicalPath + linhaLista[count + 3].imagem.ima_ds_caminho), linhaLista[count + 3].rim_id_imagem);
            //                count += 4;
            //                break;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //}

            dtImagem.AcceptChanges();
            //for (int i = 0; i < length; i++)
            //{
            //    convertToBytes();
            //}



            return GenerateReport(dtCargosAgrupados, dtEquipamentosAgrupados, _rdo, dtTarefas, dtClima, dtAcidentes, dtAssinaturaContratante, dtAssinaturaContratada, dtImagem, dtItensLaudo, gerarRelatorioFotografico);
        }
        public static byte[] GenerateReport(DataTable dtCargosAgrupados, DataTable dtEquipamentosAgrupados, laudo rdo, DataTable tarefas, DataTable dtClima, DataTable dtAcidentes, DataTable dtAssinaturaContratante, DataTable dtAssinaturaContratada, DataTable dtImagem, DataTable dtItensLaudo, bool gerarRelatorioFotografico)
        {
            DataTable dtDadosRdo = new DataTable();

            string mappath = System.Web.HttpContext.Current.Server.MapPath("~/Api/Contents/Reports/Teste.rdlc");
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

            int diasRestantes = (Convert.ToDateTime(rdo.obra.obr_dt_previsao_fim).Date - (rdo.laudo_dt_laudo).Date).Days;

            if (diasRestantes < 0)
            {
                diasRestantes = 0;
            }

            string diaDaSemana = cultureInfo.DateTimeFormat.GetDayName(rdo.laudo_dt_laudo.DayOfWeek);

            ReportViewer.DataSources.Add(new ReportDataSource("dtCargosAgrupados", dtCargosAgrupados));
            ReportViewer.DataSources.Add(new ReportDataSource("dtEquipamentosAgrupados", dtEquipamentosAgrupados));
            ReportViewer.DataSources.Add(new ReportDataSource("tarefas", tarefas));
            ReportViewer.DataSources.Add(new ReportDataSource("dtCargosAgrupados", dtCargosAgrupados));
            ReportViewer.DataSources.Add(new ReportDataSource("dtClima", dtClima));
            ReportViewer.DataSources.Add(new ReportDataSource("dtDadosRdo", dtDadosRdo));
            ReportViewer.DataSources.Add(new ReportDataSource("dtAcidentes", dtAcidentes));
            ReportViewer.DataSources.Add(new ReportDataSource("dtAssinaturaContratante", dtAssinaturaContratante));
            ReportViewer.DataSources.Add(new ReportDataSource("dtAssinaturaContratada", dtAssinaturaContratada));
            ReportViewer.DataSources.Add(new ReportDataSource("dtItensLaudo", dtItensLaudo));
            ReportViewer.DataSources.Add(new ReportDataSource("dtImagem", dtImagem));

            ReportViewer.SetParameters(new ReportParameter("NomeObra", rdo.obra.obr_ds_obra));
            ReportViewer.SetParameters(new ReportParameter("StatusRdo", rdo.status_rdo.str_ds_status));
            ReportViewer.SetParameters(new ReportParameter("DataRdo", rdo.laudo_dt_laudo.ToString("dd/MM/yyyy")));
            ReportViewer.SetParameters(new ReportParameter("DataRdoDiaSemana", $"{diaDaSemana.ToUpper()}"));
            ReportViewer.SetParameters(new ReportParameter("DataInicioObra", rdo.obra.obr_dt_inicio.ToString("dd/MM/yyyy")));
            ReportViewer.SetParameters(new ReportParameter("DiasDecorridosObra", (rdo.laudo_dt_laudo.AddDays(1) - rdo.obra.obr_dt_inicio.Date).Days.ToString()));
            //ReportViewer.SetParameters(new ReportParameter("TipoContratanteContrada", rdo.rdo_id_status == 2 ? "COMENTÁRIO CONTRATANTE" : rdo.rdo_id_status == 3 ? "COMENTÁRIO CONTRATADA" : ""));
            ReportViewer.SetParameters(new ReportParameter("ComentarioRdo", rdo.colaborador.obra_colaborador.FirstOrDefault(oc => oc.oco_id_obra == rdo.laudo_id_obra).oco_st_contratante_contratada == "d" ? rdo.laudo_ds_comentario_geracao : rdo.laudo_ds_comentario_assinatura)); //Obter comentário contratada
            ReportViewer.SetParameters(new ReportParameter("ComentarioAssinaturaRdo", rdo.colaborador.obra_colaborador.FirstOrDefault(oc => oc.oco_id_obra == rdo.laudo_id_obra).oco_st_contratante_contratada == "t" ? rdo.laudo_ds_comentario_geracao : rdo.laudo_ds_comentario_assinatura)); //Obter comentario contratante
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
    public class LaudoViewModel
    {
        public bool? gerarRelatorioFotografico { get; set; }
        public int laudo_id_laudo { get; set; }
        public int laudo_id_status { get; set; }
        public int laudo_id_obra { get; set; }
        public string laudo_dt_laudo { get; set; }
        public string laudo_ds_comentario_assinatura { get; set; }
        public int laudo_id_colaborador { get; set; }
        public System.DateTime laudo_dt_geracao { get; set; }
        public string laudo_tp_comentario_assinatura { get; set; }
        public string laudo_ds_comentario_geracao { get; set; }
        public string laudo_tp_comentario_geracao { get; set; }
        public bool laudo_tp_nivel_cloro { get; set; }
        public bool laudo_tp_ph { get; set; }
        public bool laudo_tp_limpidez { get; set; }
        public bool laudo_tp_superficie { get; set; }
        public bool laudo_tp_fundo { get; set; }
        public bool laudo_tp_nivel_cloro_2 { get; set; }
        public bool laudo_tp_nivel_bacterias { get; set; }
        public bool laudo_tp_nivel_proliferacao { get; set; }
        public string DiaDaSemana { get; set; }
        public string DescricaoStatus { get; set; }
        public virtual colaborador colaborador { get; set; }
        public virtual status_rdo status_rdo { get; set; }
        public virtual obra obra { get; set; }

        public LaudoViewModel()
        {

        }
    }
}