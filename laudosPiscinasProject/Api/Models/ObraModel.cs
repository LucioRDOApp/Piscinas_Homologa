using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace laudosPiscinasProject.Api.Models
{
    public class ObraModel
    {
        public static List<ObraViewModel> Lista(dynamic param)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            int idColaborador = param.idColaborador ?? 0;

            List<obra> obraLista = context.obra.Where(obr => obr.obra_colaborador.Count(oco => oco.oco_id_colaborador == idColaborador) > 0).ToList();

            if (param != null)
            {
                string descricao = param.descricao ?? "";
                int contratante = param.idContratante ?? 0;
                int contratada = param.idContratada ?? 0;
                int uf = param.idUf ?? 0;
                int idMunicipio = param.idMunicipio ?? 0;
                int status = param.statusObra ?? 0;
                int setor = param.setor ?? 0;

                DateTime dataInicioDe = String.IsNullOrEmpty(Convert.ToString(param.dataInicioDe)) ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataInicioDe));
                DateTime dataInicioAte = String.IsNullOrEmpty(Convert.ToString(param.dataInicioAte)) ? DateTime.MinValue : Convert.ToDateTime(Convert.ToString(param.dataInicioAte));

                if (!string.IsNullOrEmpty(descricao))
                {
                    obraLista = obraLista.Where(obr => obr.obr_ds_obra.ToLower().Contains(descricao.ToLower())).ToList();
                }
                if (contratante > 0)
                {
                    obraLista = obraLista.Where(obr => obr.obr_id_empresa_contratante == contratante).ToList();
                }
                if (contratada > 0)
                {
                    obraLista = obraLista.Where(obr => obr.obr_id_empresa_contratada == contratada).ToList();
                }
                if (uf > 0)
                {
                    obraLista = obraLista.Where(obr => obr.municipio.mun_id_uf == uf).ToList();
                }
                if (idMunicipio > 0)
                {
                    obraLista = obraLista.Where(obr => obr.municipio.mun_id_municipio == idMunicipio).ToList();
                }
                if (status > 0)
                {
                    if (status == 1)
                    {
                        obraLista = obraLista.Where(obr => obr.obr_dt_fim == null || obr.obr_dt_fim == DateTime.MinValue).ToList();
                    }
                    else
                    {
                        obraLista = obraLista.Where(obr => obr.obr_dt_fim != null && obr.obr_dt_fim != DateTime.MinValue).ToList();
                    }
                }


                if (dataInicioDe > DateTime.MinValue)
                {
                    obraLista = obraLista.Where(obr => obr.obr_dt_inicio == dataInicioDe.Date).ToList();
                }
                if (dataInicioAte > DateTime.MinValue)
                {
                    obraLista = obraLista.Where(obr => obr.obr_dt_previsao_fim == dataInicioAte.Date).ToList();
                }

            }

            List<ObraViewModel> Lista = new List<ObraViewModel>();
            obraLista.ToList().ForEach(obr =>
            {
                grupo grupo = obr.obra_colaborador == null ? null : obr.obra_colaborador.FirstOrDefault(x => x.oco_id_colaborador == idColaborador).grupo;
                Lista.Add(new ObraViewModel
                {
                    Descricao = obr.obr_ds_obra,
                    IdObra = obr.obr_id_obra,
                    CidadeEstado = string.Concat(obr.municipio.mun_ds_municipio, "/", obr.municipio.uf.ufe_ds_sigla),
                    DescricaoContratante = obr.empresa1 != null ? obr.empresa1.emp_nm_fantasia : "",
                    DescricaoContratada = obr.empresa2 != null ? obr.empresa2.emp_nm_fantasia : "",
                    DiasDecorridos = dateRange(DateTime.Now.Date, obr.obr_dt_inicio, (obr.obr_dt_fim == null ? DateTime.MinValue : (DateTime)obr.obr_dt_fim)),
                    DataInicio = String.Format("{0:dd/MM/yyyy}", Convert.ToString(obr.obr_dt_inicio).Substring(0, 10)),
                    DataConclusao = obr.obr_dt_fim != null ? String.Format("{0:dd/MM/yyyy}", Convert.ToString(obr.obr_dt_fim).Substring(0, 10)) : "",
                    ContratanteContratada = grupo == null ? "" : grupo.gru_st_contratante == 1 ? "contratante" : "contratada",
                    StatusBasicaGratuita = grupo == null ? "" : grupo.gru_nm_nome,
                    ObraFinalizada = (String.IsNullOrEmpty(obr.obr_dt_fim.ToString()) || (obr.obr_dt_fim == DateTime.MinValue)),
                    ClasseStatusCss = ClasseStatusCss(obr),
                    ProgressoPorcentagem = ProgressoPorcentagem(obr)
                });
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

        internal static int QuantidadeMaximaFotos(int idObra)
        {
            using (var context = new LaudosPiscinasEntities())
            {
                int resultado = 0;
                try
                {
                    resultado = context.obra.Find(idObra).empresa.licenca.lic_qtd_imagens_tarefas;
                }
                catch { }
                return resultado;
            }
        }

        public static int ProgressoPorcentagem(obra obra)
        {
            DateTime inicio = obra.obr_dt_inicio;
            DateTime fim = (DateTime)obra.obr_dt_previsao_fim;
            DateTime atual = DateTime.Now;

            double total = fim.Subtract(inicio).Days;
            double decorrido = atual.Subtract(inicio).Days;

            if (atual >= fim)
            {
                return 100;
            }
            else if (atual < inicio)
            {
                return 0;
            }

            int result = Convert.ToInt32(Math.Round(100 / total * decorrido, 2));

            return result;
        }
        private static string ClasseStatusCss(obra obra)
        {
            if (ProgressoPorcentagem(obra) == 100)
            {
                List<int> idsTarefas = new List<int>();
                foreach (etapa et in obra.etapa)
                {
                    et.tarefa.GroupBy(c => c.tar_nr_agrupador)
                     .Select(g => g.OrderByDescending(c => c.tar_dt_medicao).ThenByDescending(c => c.tar_dt_medicao_hora_inicial).First())
                     .Select(c => new { c.tar_id_tarefa, c.tar_id_status }).Where(t => t.tar_id_status <= 2 || t.tar_id_status == 4).ToList().ForEach(t => idsTarefas.Add(t.tar_id_tarefa));
                }
                
                bool existeTarefaPendente = idsTarefas.Count() > 0;
                if (existeTarefaPendente)
                {
                    return "bg-vermelho";
                }
                return "bg-verde";
            }
            return "bg-cinza";
        }

        public static int dateRange(DateTime dataAtual, DateTime DataInicio, DateTime DataFim)
        {
            int result = 0;
            TimeSpan dateResult;

            if (DataFim == DateTime.MinValue)
            {
                dateResult = (dataAtual - DataInicio);
            }
            else
            {
                dateResult = (DataFim - DataInicio);
            }
            result = Convert.ToInt32(dateResult.TotalDays);

            return result;
        }

        public static List<ColaboradorViewModel> ObterColaboradoresObra(dynamic param)
        {
            int idObra = param.idObra ?? 0;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            List<ColaboradorViewModel> returnList = new List<ColaboradorViewModel>();
            List<obra_colaborador> listaObraColaborador = context.obra_colaborador.Where(oco => oco.oco_id_obra == idObra).ToList();


            listaObraColaborador.ForEach(x =>
                 returnList.Add(
                new ColaboradorViewModel
                {
                    Nome = x.colaborador.col_nm_colaborador,
                    IdColaborador = x.oco_id_colaborador,
                    TelefonePrincipal = string.IsNullOrEmpty(x.colaborador.col_ds_telefone_principal) ? "" : x.colaborador.col_ds_telefone_principal.Length == 11  ? Int64.Parse(x.colaborador.col_ds_telefone_principal).ToString("(##) #####-####") : Int64.Parse(x.colaborador.col_ds_telefone_principal).ToString("(##) ####-####"),                    
                    DescricaoCargo = x.cargo.car_ds_cargo,
                    Email = x.colaborador.col_ds_email,
                    DescricaoGrupo = x.grupo.gru_nm_nome,
                    Bairro = x.colaborador.col_ds_logradouro,
                    Cargo = x.oco_id_cargo,
                    Cep = x.colaborador.col_ds_cep,
                    Complemento = x.colaborador.col_ds_complemento,
                    Cpf = x.colaborador.col_nr_cpf,
                    Crea = x.colaborador.col_ds_crea,
                    DataContratacao = x.oco_dt_contratacao == null ? "" : String.Format("{0:dd/MM/yyyy}", Convert.ToString(x.oco_dt_contratacao).Substring(0, 10)),
                    IdMunicipio = x.colaborador.col_id_municipio ?? 0,
                    DataNascimento = x.colaborador.col_dt_nascimento == null ? "" : String.Format("{0:dd/MM/yyyy}", Convert.ToString(x.colaborador.col_dt_nascimento).Substring(0, 10)),
                    Grupo = x.oco_id_grupo,
                    Logradouro = x.colaborador.col_ds_logradouro,
                    Uf = x.colaborador.municipio == null ? 0 : x.colaborador.municipio.mun_id_uf,
                    NumeroEndereco = x.colaborador.col_ds_numero,
                    Sexo = x.colaborador.col_ds_sexo,
                    Senha = Seguranca.DecryptTripleDES(x.colaborador.col_ds_senha),
                    TelefoneSecundario = x.colaborador.col_ds_telefone_secundario,
                    IdObraColaborador = x.oco_id_obra_colaborador

                })
            );

            return returnList;
        }

        public static List<EquipamentoViewModel> ObterEquipamentosObra(dynamic param)
        {
            int idObra = param.idObra ?? 0;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            List<EquipamentoViewModel> returnList = new List<EquipamentoViewModel>();
            List<obra_equipamento> listaObraEquipamento = context.obra_equipamento.Where(oeq => oeq.oeq_id_obra == idObra).ToList();


            listaObraEquipamento.ForEach(x =>
                 returnList.Add(
                new EquipamentoViewModel
                {
                    Descricao = x.equipamento.equ_ds_equipamento,
                    Modelo = x.equipamento.equ_ds_modelo,
                    Marca = x.equipamento.equ_ds_marca,
                    FabricanteFornecedor = x.oeq_ds_fabricante_fornecedor,
                    DataAquisicao = x.oeq_dt_aquisicao == null ? "" : String.Format("{0:dd/MM/yyyy}", Convert.ToString(x.oeq_dt_aquisicao).Substring(0, 10)),
                    Id = x.equipamento.equ_id_equipamento,
                    IdObra = x.oeq_id_obra,
                    Contato = x.oeq_ds_contato,
                    Telefone = x.oeq_ds_telefone,
                    TipoAquisicao = x.oeq_tp_aquisicao,
                    TipoEquipamento = x.equipamento.tipo_equipamento.teq_nm_tipo_equipamento,
                    IdObraEquipamento = x.oeq_id_obra_equipamento,
                    DescricaoTipoAquisicao = x.oeq_tp_aquisicao == "S" ? "Não Informado" : (x.oeq_tp_aquisicao == "C" ? "Compra" : (x.oeq_tp_aquisicao == "A" ? "Aluguel" : ""))

                })
            );

            return returnList;
        }

        public static ObraViewModel Salvar(dynamic param)
        {
            if (string.IsNullOrEmpty(param.descricao.ToString()))
            {
                throw new Exception("O titulo deve ser preenchido");
            }
            //if (param.dataInicio.ToString() == "__/__/____" || string.IsNullOrEmpty(Convert.ToString(param.dataInicio)) || (Convert.ToDateTime(Convert.ToString(param.dataInicio)) == DateTime.MinValue))
            if (param.dataInicio.ToString() == "__/__/____")
            {
                throw new Exception("A data inicial deve ser preenchida");
            }
            //if (param.dataPrevisaoFim.ToString() == "__/__/____" || string.IsNullOrEmpty(Convert.ToString(param.dataPrevisaoFim)) || (Convert.ToDateTime(Convert.ToString(param.dataPrevisaoFim)) == DateTime.MinValue))
            if (param.dataPrevisaoFim.ToString() == "__/__/____")
            {
                throw new Exception("A data de previsão de final deve ser preenchida");
            }
            if (string.IsNullOrEmpty(param.logradouro.ToString()))
            {
                throw new Exception("O logradouro deve ser preenchido");
            }
            if (param.idUf == 0)
            {
                throw new Exception("A UF deve ser preenchida");
            }

            if (param.idMunicipio == 0)
            {
                throw new Exception("O município deve ser preenchido");
            }
            if ((string.IsNullOrEmpty(param.idContratante.ToString()) || param.idContratante.ToString() == "0") && (string.IsNullOrEmpty(param.idContratada.ToString()) || param.idContratada.ToString() == "0"))
            {
                throw new Exception("A obra deve possuir pelo menos uma empresa responsável associada como contratante ou contratada");
            }

            int idObra = param.idObra ?? 0;
            int idColaborador = Convert.ToInt32(param.usuario.id) ?? 0;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            obra _obra = context.obra.Where(x => x.obr_id_obra == idObra).FirstOrDefault() ?? new obra();

            _obra.obr_ds_obra = param.descricao;
            if (Convert.ToInt32(param.idContratante) > 0)
            {
                _obra.obr_id_empresa_contratante = param.idContratante;
            }
            if (Convert.ToInt32(param.idContratada) > 0)
            {
                _obra.obr_id_empresa_contratada = param.idContratada;
            }
            if (_obra.obr_id_dono == null)
            {
                //_obra.obr_id_dono = param.idContratada ?? param.idContratada;
                if (_obra.obr_id_empresa_contratante != null && _obra.obr_id_empresa_contratante > 0)
                {
                    _obra.obr_id_dono = param.idContratante;
                }
                else if (_obra.obr_id_empresa_contratada != null && _obra.obr_id_empresa_contratada > 0)
                {
                    _obra.obr_id_dono = param.idContratada;
                }
                else
                {
                    throw new Exception("A obra deve possuir pelo menos uma empresa responsável associada como contratante ou contratada");
                }
            }

            if (idObra == 0)
            {
                string statusLicenca = LoginModel.VerificarLicenca(EmpresaModel.ObterRegistro(_obra.obr_id_dono).Token);
                if (statusLicenca != "ATIVA")
                {
                    throw new Exception("Não foi possível cadastrar a obra. A licença dessa empresa está " + statusLicenca);
                }
            }

            _obra.obr_dt_inicio = Convert.ToDateTime(param.dataInicio.ToString());
            _obra.obr_dt_previsao_fim = Convert.ToDateTime(param.dataPrevisaoFim.ToString());
            _obra.obr_ds_logradouro = param.logradouro;
            _obra.obr_ds_numero = param.numeroEndereco ?? 0;
            _obra.obr_ds_complemento = param.complemento;
            string cep = param.cep;
            cep = !string.IsNullOrEmpty(cep) ? cep.Replace("-", "").Replace(".", "") : cep;
            _obra.obr_ds_cep = cep;
            _obra.obr_id_municipio = param.idMunicipio;
            _obra.obr_ds_bairro = param.bairro;

            //if (!String.IsNullOrEmpty(param.foto.ToString()) && param.foto.filename != _obra.obr_ds_foto)
            //{
            //    _obra.obr_ds_foto = UtilsModel.SalvarImagem(param.foto, "ImagensObra");
            //}

            _obra.obr_nr_horas_semana = String.IsNullOrEmpty(param.qtdHrsSemana.ToString()) ? 8 : param.qtdHrsSemana;
            _obra.obr_nr_horas_sabado = String.IsNullOrEmpty(param.qtdHrsSabado.ToString()) ? 4 : param.qtdHrsSabado;
            _obra.obr_nr_horas_domingo = String.IsNullOrEmpty(param.qtdHrsDomingo.ToString()) ? 0 : param.qtdHrsDomingo;
            _obra.obr_ds_art = param.art;
            _obra.obr_nr_area_total = String.IsNullOrEmpty(param.areaTotalObra.ToString()) ? null : param.areaTotalObra;
            _obra.obr_nr_area_total_construida = String.IsNullOrEmpty(param.areaTotalObra.ToString()) ? null : param.areaTotalConstruida;
            if (!String.IsNullOrEmpty(Convert.ToString(param.dataFim)))
            {
                _obra.obr_dt_fim = Convert.ToDateTime(Convert.ToString(param.dataFim));
            }


            if (_obra.obr_id_obra > 0)
            {
                context.obra.Attach(_obra);
                context.Entry(_obra).State = EntityState.Modified;

            }
            else
            {
                if (PermissaoLicenca(_obra))
                {
                    _obra.obr_id_colaborador = idColaborador;
                    context.obra.Add(_obra);
                }
                else
                {
                    throw new Exception("A licença dessa empresa não permite que novas obras sejam adicionadas.");
                }
            }

            try
            {
                bool result = context.SaveChanges() > 0;

                // 2019-08-12 Imagem da contratada
                // A imagem é tratada depois q o registro é inserido no banco de dados                
                if (!String.IsNullOrEmpty(param.foto.ToString()))
                {
                    _obra.obr_ds_foto = UtilsModel.UploadImage(param.foto.ToString(), _obra.obr_id_obra.ToString() + "/logocontratada/");
                }
                else
                {
                    _obra.obr_ds_foto = null;
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            licenca licenca = context.empresa.FirstOrDefault(x => x.emp_id_empresa == _obra.obr_id_dono).licenca;


            SalvarListaColaboradores(param, context, _obra.obr_id_obra, idColaborador, licenca);
            SalvarListaEquipamentos(param, context, _obra.obr_id_obra);

            dynamic dynObj = new System.Dynamic.ExpandoObject();
            dynObj.idObra = _obra.obr_id_obra;
            dynObj.idObraColaborador = idColaborador;

            return ObterRegistro(dynObj);
        }

        public static bool PermissaoLicenca(obra _obra)
        {
            bool result = false;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            empresa emp = context.empresa.FirstOrDefault(x => x.emp_id_empresa == _obra.obr_id_dono);
            int numeroObrasEmpresa = context.obra.Where(x => x.obr_id_dono == emp.emp_id_empresa).Count();
            int numeroObrasLicenca = emp.licenca == null ? 0 : emp.licenca.lic_nr_qtd_obras ?? 0;

            result = numeroObrasEmpresa >= numeroObrasLicenca;
            return !result;
        }




        public static bool AtualizarConvidada(int idConvidada, int idObra, int tipoConvite)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            obra _obra = context.obra.Where(x => x.obr_id_obra == idObra).FirstOrDefault() ?? new obra();


            if (tipoConvite == 1)
            {
                _obra.obr_id_empresa_contratante = idConvidada;
            }
            else if (tipoConvite == 2)
            {
                _obra.obr_id_empresa_contratada = idConvidada;
            }
            else
            {
                throw new Exception("Não foi possível associar a empresa com a obra.");
            }

            if (_obra.obr_id_obra > 0)
            {
                context.obra.Attach(_obra);
                context.Entry(_obra).State = EntityState.Modified;
            }
            else
            {
                context.obra.Add(_obra);
            }

            bool result = context.SaveChanges() > 0;
            return result;
        }

        public static bool SalvarColaboradorLogado(dynamic param, LaudosPiscinasEntities context, int idObra, int idColaborador, licenca tipoLicenca)
        {
            bool result = true;

            obra _obra = context.obra.Find(idObra);

            obra_colaborador _obra_colaborador = context.obra_colaborador.Where(x => x.oco_id_obra == idObra && x.oco_id_colaborador == idColaborador).FirstOrDefault() ?? new obra_colaborador();
            _obra_colaborador.oco_id_obra = idObra;
            _obra_colaborador.oco_id_colaborador = idColaborador;
            _obra_colaborador.oco_st_contratante_contratada = param.contratanteContratada; // verificar se a contratante ou contratada estão nulos e preencher com base no valor que houver

            int contratanteContratada = param.contratanteContratada.ToString() == "t" ? 1 : param.contratanteContratada.ToString() == "d" ? 2 : 0;

            //possible error
            if (contratanteContratada == 0)
            {
                context.obra.Remove(_obra);
                context.SaveChanges();
                throw new Exception("Não foi possivel identificar se é uma obra contratante ou contratada.");
            }
            // end possible error

            grupo grupoUsuarioLogado = context.grupo.FirstOrDefault(gru => gru.gru_st_diretor == 1 && gru.gru_st_contratante == contratanteContratada && gru.gru_id_licenca == tipoLicenca.lic_id_licenca);
            if (grupoUsuarioLogado == null)
            {
                context.obra.Remove(_obra);
                context.SaveChanges();
                throw new Exception("Não foi possivel identificar um grupo para cadastrar a obra, contate o administrador.");
            }


            if (_obra_colaborador.oco_id_obra_colaborador == 0)
            {
                if (_obra_colaborador.oco_st_contratante_contratada == "t")
                {
                    _obra_colaborador.oco_id_cargo = Convert.ToInt32(ParametroModel.ObterValor("CargoDefaultDonoContratante")); // buscar no parametro o cargo default para dono da obra
                    _obra_colaborador.oco_id_grupo = grupoUsuarioLogado.gru_id_grupo;
                }
                else if (_obra_colaborador.oco_st_contratante_contratada == "d")
                {
                    _obra_colaborador.oco_id_cargo = Convert.ToInt32(ParametroModel.ObterValor("CargoDefaultDonoContratada")); // buscar no parametro o cargo default para dono da obra
                    _obra_colaborador.oco_id_grupo = grupoUsuarioLogado.gru_id_grupo;
                }
                else
                {
                    throw new Exception("O colaborador precisa ser associado a contratante ou a contratada");
                }


                //_obra_colaborador.oco_id_cargo = Convert.ToInt32(ParametroModel.ObterValor("CargoDefaultDono")); // buscar no parametro o cargo default para dono da obra
                //_obra_colaborador.oco_id_grupo = Convert.ToInt32(ParametroModel.ObterValor("GrupoDefaultDono")); // buscar no parametro o grupo default para dono da obra
                _obra_colaborador.oco_dt_contratacao = DateTime.Now.Date;
            }


            // mudar esse método - não precisa atualizar o colaborador logado caso ele já exista
            if (_obra_colaborador.oco_id_obra_colaborador > 0)
            {
                return true;
                //context.obra_colaborador.Attach(_obra_colaborador);
                //context.Entry(_obra_colaborador).State = EntityState.Modified;
            }
            else
            {
                context.obra_colaborador.Add(_obra_colaborador);
            }

            result = context.SaveChanges() > 0;
            return result;
        }


        internal static bool SalvarListaColaboradores(dynamic param, LaudosPiscinasEntities context, int idObra, int idColaborador, licenca tipoLicenca)
        {
            bool result = SalvarColaboradorLogado(param, context, idObra, idColaborador, tipoLicenca);

            int idCriador = context.obra.Find(idObra).obr_id_colaborador ?? int.MinValue;

            //remover os coleboradores
            foreach (var item in param.listaColaboradoresRemovidos)
            {
                int idObraColaborador = item.idObraColaborador ?? 0;

                if (context.obra_colaborador.ToList().Any(x => x.oco_id_obra_colaborador == idObraColaborador && x.oco_id_colaborador != idCriador))
                {
                    context.obra_colaborador.Where(x => x.oco_id_obra_colaborador == idObraColaborador && x.oco_id_colaborador != idCriador).ToList().ForEach(y => context.obra_colaborador.Remove(y));
                }
            }
            try
            {
                result = context.SaveChanges() > 0;
            }
            catch (Exception)
            {
                throw new Exception("Não é possível excluir o colaborador, ele está associado a outro registro do sistema.");
            }

            //salvar e atualizar o objeto dos outros colaboradores
            foreach (var item in param.listaColaboradores)
            {
                item.idObra = idObra;
                item.contratanteContratada = param.contratanteContratada;

                var colaborador = new ColaboradorViewModel(item);

                ColaboradorViewModel colaboradorSalvo = ColaboradorModel.Salvar(colaborador, true);
            }

            return result;
        }


        internal static bool SalvarListaEquipamentos(dynamic param, LaudosPiscinasEntities context, int idObra)
        {
            bool result = true;

            foreach (var item in param.listaEquipamentosRemovidos)
            {
                int idObraEquipamento = item.idObraEquipamento ?? 0;

                if (context.obra_equipamento.ToList().Any(x => x.oeq_id_obra_equipamento == idObraEquipamento))
                {
                    context.obra_equipamento.Where(x => x.oeq_id_obra_equipamento == idObraEquipamento).ToList().ForEach(y => context.obra_equipamento.Remove(y));
                }
            }
            try
            {
                result = context.SaveChanges() > 0;
            }
            catch (Exception)
            {
                throw new Exception("Não é possível excluir o equipamento porque está associado a uma tarefa.");
            }


            foreach (var item in param.listaEquipamentos)
            {
                item.idObra = idObra;
                var equipamento = new EquipamentoViewModel(item);
                EquipamentosModel.Salvar(equipamento);
            }



            return result;
        }


        internal static bool Deletar(dynamic param)
        {
            int idObra = (int)param.idObra;

            ObraViewModel obra = new ObraViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            bool result = false;

            try
            {
                // as obras não serão excluidas só os relacionamentos, já deixei pronto para quando mudar.
                //context.historico_tarefa_colaborador.Where(x => x.obra_colaborador.oco_id_obra == idObra).ToList().ForEach(y => context.historico_tarefa_colaborador.Remove(y));
                //context.historico_tarefa_equipamento.Where(x => x.obra_equipamento.oeq_id_obra == idObra).ToList().ForEach(y => context.historico_tarefa_equipamento.Remove(y));
                //context.historico_tarefa_rdo.Where(x => x.tarefa.tar_id_obra == idObra).ToList().ForEach(y => context.historico_tarefa_rdo.Remove(y));
                context.rdo.Where(x => x.rdo_id_obra == idObra).ToList().ForEach(y => context.rdo.Remove(y));
                context.obra_tarefa_colaborador.Where(x => x.tarefa.etapa.eta_id_obra == idObra).ToList().ForEach(y => context.obra_tarefa_colaborador.Remove(y));
                context.obra_tarefa_equipamento.Where(x => x.tarefa.etapa.eta_id_obra == idObra).ToList().ForEach(y => context.obra_tarefa_equipamento.Remove(y));
                context.tarefa.Where(x => x.etapa.eta_id_obra == idObra).ToList().ForEach(y => context.tarefa.Remove(y));
                context.obra_colaborador.Where(x => x.oco_id_obra == idObra).ToList().ForEach(y => context.obra_colaborador.Remove(y));
                context.obra_equipamento.Where(x => x.oeq_id_obra == idObra).ToList().ForEach(y => context.obra_equipamento.Remove(y));
                context.obra.Where(x => x.obr_id_obra == idObra).ToList().ForEach(y => context.obra.Remove(y));


                result = context.SaveChanges() > 0;
            }
            catch (Exception)
            {
                return false;
            }

            return result;
        }

        internal static bool VerificarConvite(dynamic param)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            string convite = param.idObra;
            obra _obra = context.obra.FirstOrDefault(obr => obr.obr_cd_convite == convite);
            if (_obra == null)
            {
                throw new Exception("Este convite já foi utilizado ou não existe.");
            }
            return true;
        }

        internal static bool Convidar(dynamic param)
        {
            int idObra = (int)param.idObra;
            string emailConvidada = param.emailConvidada;
            int idGrupoConvite = param.idGrupoConvite ?? 0;
            string contratante = "";
            string contratada = "";

            ObraViewModel obra = new ObraViewModel();
            EmpresaViewModel empresa = new EmpresaViewModel();
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            obra _obra = context.obra.Where(x => x.obr_id_obra == idObra).FirstOrDefault();


            if (_obra.obr_id_empresa_contratada != null && _obra.obr_id_empresa_contratante != null)
            {
                throw new Exception("Já existe empresa contratante e contratada para essa obra.");
            }

            if (_obra.obr_id_empresa_contratada != null && _obra.obr_id_empresa_contratante == null)
            {
                contratante = "CONTRATANTE";
                contratada = "";
            }
            if (_obra.obr_id_empresa_contratante != null && _obra.obr_id_empresa_contratada == null)
            {
                contratada = "CONTRATADA";
                contratante = "";
            }

            _obra.obr_cd_convite = Guid.NewGuid().ToString();
            context.obra.Attach(_obra);
            context.Entry(_obra).State = EntityState.Modified;
            context.SaveChanges();

            string baseUrl = ParametroModel.ObterValor("BaseUrl");
            string url = "/convidada?o=" + _obra.obr_cd_convite;


            url = baseUrl + url;
            List<string> emails = new List<string>();

            emails.Add(emailConvidada);



            bool returnValue = MailClass.Enviar("RDO App", "Convite para o RDO App", string.Format("<p>Prezado Usuario(a):<br/ > <br/ >Este é um convite para ser {0} na obra <strong>{1}</strong>. <br/> Para aceitar acesse o link: {2} </p><p> Atenciosamente,</p>", contratante == "" ? contratada : contratante, _obra.obr_ds_obra, url), emails, "~/Assets/images/logo.png");


            return returnValue;
        }

        internal static ObraViewModel ObterRegistro(dynamic param)
        {
            int idObra = (int)param.idObra;
            int idObraColaborador = (int)param.idObraColaborador;

            ObraViewModel obra = new ObraViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            obra resultado = context.obra.FirstOrDefault(obr => obr.obr_id_obra == idObra);
            obra_colaborador _obra_colaborador = context.obra_colaborador.FirstOrDefault(obr => obr.oco_id_obra_colaborador == idObraColaborador) ?? new obra_colaborador();

            obra.IdObra = resultado.obr_id_obra;
            obra.Descricao = resultado.obr_ds_obra;

            obra.idContratante = resultado.obr_id_empresa_contratante ?? 0;
            obra.idContratada = resultado.obr_id_empresa_contratada ?? 0;
            obra.DataInicio = String.Format("{0:dd/MM/yyyy}", Convert.ToString(resultado.obr_dt_inicio).Substring(0, 10));
            obra.DataPrevisaoFim = resultado.obr_dt_previsao_fim == null ? "" : String.Format("{0:dd/MM/yyyy}", Convert.ToString(resultado.obr_dt_previsao_fim).Substring(0, 10));
            obra.Logradouro = resultado.obr_ds_logradouro;
            obra.NumeroEndereco = resultado.obr_ds_numero;
            obra.Complemento = resultado.obr_ds_complemento;
            obra.Cep = resultado.obr_ds_cep;
            obra.IdMunicipio = resultado.obr_id_municipio;
            obra.IdUf = resultado.municipio.mun_id_uf;
            obra.Bairro = resultado.obr_ds_bairro;
            obra.DescricaoFoto = resultado.obr_ds_foto;
            obra.QtdHrsSemana = resultado.obr_nr_horas_semana ?? 8;
            obra.QtdHrsSabado = resultado.obr_nr_horas_sabado ?? 4;
            obra.QtdHrsDomingo = resultado.obr_nr_horas_domingo ?? 0;
            obra.Art = resultado.obr_ds_art;
            obra.AreaTotalObra = resultado.obr_nr_area_total ?? 0;
            obra.AreaTotalConstruida = resultado.obr_nr_area_total_construida ?? 0;
            obra.ContratanteContratada = _obra_colaborador.oco_st_contratante_contratada;
            obra.ObraFinalizada = (String.IsNullOrEmpty(resultado.obr_dt_fim.ToString()) || (resultado.obr_dt_fim == DateTime.MinValue));

            if (resultado.obr_dt_fim != null)
            {
                obra.DataFim = resultado.obr_dt_fim == null ? "" : String.Format("{0:dd/MM/yyyy}", Convert.ToString(resultado.obr_dt_fim).Substring(0, 10));
            }

            obra.listaColaboradores = new List<ColaboradorViewModel>();
            obra.listaEquipamentos = new List<EquipamentoViewModel>();



            List<obra_colaborador> listaObraColaborador = context.obra_colaborador.Where(oco => oco.oco_id_obra == idObra).ToList();
            List<obra_equipamento> listaObraEquipamento = context.obra_equipamento.Where(oeq => oeq.oeq_id_obra == idObra).ToList();

            listaObraColaborador.ForEach(x =>
                 obra.listaColaboradores.Add(
                new ColaboradorViewModel
                {
                    Nome = x.colaborador.col_nm_colaborador,
                    IdColaborador = x.oco_id_colaborador,
                    TelefonePrincipal = string.IsNullOrEmpty(x.colaborador.col_ds_telefone_principal) ? "" : Int64.Parse(x.colaborador.col_ds_telefone_principal).ToString("## #####-####"),
                    DescricaoCargo = x.cargo.car_ds_cargo,
                    Email = x.colaborador.col_ds_email,
                    DescricaoGrupo = x.grupo.gru_nm_nome,
                    Bairro = x.colaborador.col_ds_logradouro,
                    Cargo = x.oco_id_cargo,
                    Cep = x.colaborador.col_ds_cep,
                    Complemento = x.colaborador.col_ds_complemento,
                    Cpf = x.colaborador.col_nr_cpf,
                    Crea = x.colaborador.col_ds_crea,
                    DataContratacao = x.oco_dt_contratacao == null ? "" : String.Format("{0:dd/MM/yyyy}", Convert.ToString(x.oco_dt_contratacao).Substring(0, 10)),
                    IdMunicipio = x.colaborador.col_id_municipio ?? 0,
                    DataNascimento = x.colaborador.col_dt_nascimento == null ? "" : String.Format("{0:dd/MM/yyyy}", Convert.ToString(x.colaborador.col_dt_nascimento).Substring(0, 10)),
                    Grupo = x.oco_id_grupo,
                    Logradouro = x.colaborador.col_ds_logradouro,
                    Uf = x.colaborador.municipio == null ? 0 : x.colaborador.municipio.mun_id_uf,
                    NumeroEndereco = x.colaborador.col_ds_numero,
                    Sexo = x.colaborador.col_ds_sexo,
                    Senha = Seguranca.DecryptTripleDES(x.colaborador.col_ds_senha),
                    TelefoneSecundario = String.IsNullOrEmpty(x.colaborador.col_ds_telefone_secundario) ? "" : Int64.Parse(x.colaborador.col_ds_telefone_secundario).ToString("## #####-####"),
                    IdObraColaborador = x.oco_id_obra_colaborador,
                    Excluivel = x.oco_id_colaborador != x.obra.obr_id_colaborador,
                    Editavel = false
                })
            );


            listaObraEquipamento.ForEach(x =>
                 obra.listaEquipamentos.Add(
                new EquipamentoViewModel
                {
                    Descricao = x.equipamento.equ_ds_equipamento,
                    Modelo = x.equipamento.equ_ds_modelo,
                    Marca = x.equipamento.equ_ds_marca,
                    FabricanteFornecedor = x.oeq_ds_fabricante_fornecedor,
                    DataAquisicao = x.oeq_dt_aquisicao == null ? "" : String.Format("{0:dd/MM/yyyy}", Convert.ToString(x.oeq_dt_aquisicao).Substring(0, 10)),
                    Id = x.equipamento.equ_id_equipamento,
                    IdObra = x.oeq_id_obra,
                    Contato = x.oeq_ds_contato,
                    Telefone = String.IsNullOrEmpty(x.oeq_ds_telefone) ? "" : Int64.Parse(Regex.Replace(x.oeq_ds_telefone, "[^0-9]", "")).ToString("## #####-####"),
                    TipoAquisicao = x.oeq_tp_aquisicao,
                    TipoEquipamento = x.equipamento.tipo_equipamento.teq_nm_tipo_equipamento,
                    IdObraEquipamento = x.oeq_id_obra_equipamento,
                    DescricaoTipoAquisicao = x.oeq_tp_aquisicao == "S" ? "Não Informado" : (x.oeq_tp_aquisicao == "C" ? "Compra" : (x.oeq_tp_aquisicao == "A" ? "Aluguel" : ""))

                })
            );


            return obra;
        }

    }


    public class ObraViewModel
    {
        public int idColaborador { get; set; }
        public int IdObra { get; set; }
        public string Descricao { get; set; }
        public string CidadeEstado { get; set; }
        public int idDono { get; set; }
        public int idContratante { get; set; }
        public int idContratada { get; set; }
        public string DescricaoContratante { get; set; }
        public string DescricaoContratada { get; set; }
        public string DataInicio { get; set; }
        public string DataConclusao { get; set; }
        public string DataPrevisaoFim { get; set; }
        public string DataFim { get; set; }

        public string Logradouro { get; set; }
        public string NumeroEndereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public int IdMunicipio { get; set; }
        public int IdUf { get; set; }
        public bool ObraFinalizada { get; set; }
        public string DescricaoFoto { get; set; }


        public int QtdHrsSemana { get; set; }
        public int QtdHrsSabado { get; set; }
        public int QtdHrsDomingo { get; set; }

        public string Art { get; set; }
        public int AreaTotalObra { get; set; }
        public int AreaTotalConstruida { get; set; }
        public int DiasDecorridos { get; set; }
        /// <summary>
        /// ClasseStatusCss
        /// </summary>
        public string ClasseStatusCss { get; set; }
        /// <summary>
        /// ProgressoPorcentagem
        /// </summary>
        public int ProgressoPorcentagem { get; set; }

        public ColaboradorViewModel colaboradorObj { get; set; }
        public EquipamentoViewModel equipamentoObj { get; set; }
        public string ContratanteContratada { get; set; }
        public string StatusBasicaGratuita { get; set; }

        public bool Marcado { get; set; }

        //public List<ColaboradorViewModel> listaColaboradores { get; set; }
        public virtual ICollection<ColaboradorViewModel> listaColaboradores { get; set; }
        public virtual ICollection<EquipamentoViewModel> listaEquipamentos { get; set; }
        public virtual ICollection<ColaboradorViewModel> listaColaboradoresRemovidos { get; set; }
        public virtual ICollection<EquipamentoViewModel> listaEquipamentosRemovidos { get; set; }


        public ObraViewModel()
        {
            //this.listaColaboradores = new List<ColaboradorViewModel>();
            this.listaColaboradores = new HashSet<ColaboradorViewModel>();
            this.listaColaboradoresRemovidos = new HashSet<ColaboradorViewModel>();
            this.listaEquipamentosRemovidos = new HashSet<EquipamentoViewModel>();
            this.listaEquipamentos = new HashSet<EquipamentoViewModel>();
            this.colaboradorObj = new ColaboradorViewModel();
            this.equipamentoObj = new EquipamentoViewModel();
        }


    }
}