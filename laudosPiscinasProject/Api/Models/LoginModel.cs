using LaudosPiscinasClass;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;

namespace laudosPiscinasProject.Api.Models
{
    public class LoginModel
    {

        public static LoginViewModel LoginUser(dynamic param)
        {
            try
            {
                string cpf = param.cpf;
                cpf = !string.IsNullOrEmpty(cpf) ? cpf.Replace(".", "").Replace("-", "") : cpf;

                string senha = Seguranca.EncryptTripleDES(Convert.ToString(param.senha));


                LaudosPiscinasEntities context = new LaudosPiscinasEntities();

                var Colaborador = context.colaborador.FirstOrDefault(u => u.col_nr_cpf == cpf && u.col_ds_senha == senha);

                if (Colaborador == null)
                {
                    throw new Exception("Usuário ou senha não existem.");
                }

                LoginViewModel data = new LoginViewModel();

                data.Routes = ObterRotasDefault(Colaborador);

                data.Menu = ObterMenuDefault(Colaborador);

                //mudar para colaborador viewmodel
                data.Usuario = new UsuarioViewModel
                {
                    Email = Colaborador.col_ds_email,
                    Id = (int)Colaborador.col_id_colaborador,
                    Senha = Seguranca.DecryptTripleDES(Colaborador.col_ds_senha),
                    NomeUsuario = Colaborador.col_nm_colaborador
                };

                var historico = new HistoricoLogin()
                {
                    col_id_colaborador = Colaborador.col_id_colaborador,
                    col_ds_email = Colaborador.col_ds_email,
                    col_nm_colaborador = Colaborador.col_nm_colaborador,
                    col_nr_cpf = Colaborador.col_nr_cpf,
                    data_login = DateTime.Now
                };
                InserirHistoricoLogin(historico);

                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        internal static void Enviar()
        {
            MailClass.Enviar("RDO App", "Recuperar Senha", "teste", new string[] { "nivaldooo@gmail.com" });
        }

        private static MenuViewModel ObterMenuDefault(colaborador colaborador)
        {
            MenuViewModel menu = new MenuViewModel();
            menu.ListaPagina = new List<PaginaViewModel>();

            if (colaborador.col_st_admin != null && colaborador.col_st_admin == true)
            {
                menu.ListaPagina.Add(new PaginaViewModel { Titulo = "Pagina", Caminho = "/pagina/index" });
                menu.ListaPagina.Add(new PaginaViewModel { Titulo = "Menu", Caminho = "/menu/index" });
                menu.ListaPagina.Add(new PaginaViewModel { Titulo = "Grupo", Caminho = "/grupo/index" });
                menu.ListaPagina.Add(new PaginaViewModel { Titulo = "Histórico de Acessos", Caminho = "/historicoacesso/index" });
            }

            return menu;
        }

        private static List<RouteViewModel> ObterRotasDefault(colaborador colaborador)
        {
            var ListaRotas = new List<RouteViewModel>();

            RouteViewModel rota = new RouteViewModel();
            rota.Name = "Escolher Obra";
            rota.Path = "/obra/escolher";
            rota.Permissions = new List<string>();
            rota.Permissions.Add("visualizar");
            ListaRotas.Add(rota);

            rota = new RouteViewModel();
            rota.Name = "Adicionar Obra";
            rota.Path = "/obra/cadastro";
            rota.Permissions = new List<string>();
            rota.Permissions.Add("visualizar");
            ListaRotas.Add(rota);

            rota = new RouteViewModel();
            rota.Name = "Alterar Senha";
            rota.Path = "/colaborador/alterarsenha";
            rota.Permissions = new List<string>();
            rota.Permissions.Add("visualizar");
            ListaRotas.Add(rota);

            rota = new RouteViewModel();
            rota.Name = "Convidada";
            rota.Path = "/convidada";
            rota.Permissions = new List<string>();
            rota.Permissions.Add("visualizar");
            ListaRotas.Add(rota);

            //todo: cadastrar no banco corretamente
            rota = new RouteViewModel();
            rota.Name = "Etapa";
            rota.Path = "/etapa/index";
            rota.Permissions = new List<string>();
            rota.Permissions.Add("visualizar");
            ListaRotas.Add(rota);

            rota = new RouteViewModel();
            rota.Name = "Etapa";
            rota.Path = "/etapa/cadastro";
            rota.Permissions = new List<string>();
            rota.Permissions.Add("visualizar");
            ListaRotas.Add(rota);

            rota = new RouteViewModel();
            rota.Name = "Gráfico";
            rota.Path = "/chart";
            rota.Permissions = new List<string>();
            rota.Permissions.Add("visualizar");
            ListaRotas.Add(rota);

            rota = new RouteViewModel();
            rota.Name = "Gráfico RDOs";
            rota.Path = "/chart/rdos";
            rota.Permissions = new List<string>();
            rota.Permissions.Add("visualizar");
            ListaRotas.Add(rota);

            rota = new RouteViewModel();
            rota.Name = "Gráfico RDOs Atrasado";
            rota.Path = "/chart/atrasado";
            rota.Permissions = new List<string>();
            rota.Permissions.Add("visualizar");
            ListaRotas.Add(rota);

            rota = new RouteViewModel();
            rota.Name = "Gráfico Dia Improdutivo";
            rota.Path = "/chart/diaimprodutivo";
            rota.Permissions = new List<string>();
            rota.Permissions.Add("visualizar");
            ListaRotas.Add(rota);

            rota = new RouteViewModel();
            rota.Name = "Gráfico Tarefas";
            rota.Path = "/chart/tarefa";
            rota.Permissions = new List<string>();
            rota.Permissions.Add("visualizar");
            ListaRotas.Add(rota);

            rota = new RouteViewModel();
            rota.Name = "Gráfico Comentários";
            rota.Path = "/chart/comentario";
            rota.Permissions = new List<string>();
            rota.Permissions.Add("visualizar");
            ListaRotas.Add(rota);

            rota = new RouteViewModel();
            rota.Name = "Dashboard Gráfico";
            rota.Path = "/dashboardGrafico";
            rota.Permissions = new List<string>();
            rota.Permissions.Add("visualizar");
            ListaRotas.Add(rota);

            ListaRotas.AddRange(ObterRotasAdmin(colaborador));

            return ListaRotas;
        }

        private static List<RouteViewModel> ObterRotasAdmin(colaborador colaborador)
        {
            var ListaRotas = new List<RouteViewModel>();

            if (colaborador.col_st_admin != null && colaborador.col_st_admin == true)
            {
                RouteViewModel rota = new RouteViewModel();
                rota.Name = "Pagina";
                rota.Path = "/pagina/index";
                rota.Permissions = new List<string>();
                rota.Permissions.AddRange("visualizar,editar,deletar,cadastrar".Split(','));
                ListaRotas.Add(rota);

                rota = new RouteViewModel();
                rota.Name = "Cadastro Pagina";
                rota.Path = "/pagina/cadastro";
                rota.Permissions = new List<string>();
                rota.Permissions.AddRange("visualizar,editar,deletar,cadastrar".Split(','));
                ListaRotas.Add(rota);

                rota = new RouteViewModel();
                rota.Name = "Grupo";
                rota.Path = "/grupo/index";
                rota.Permissions = new List<string>();
                rota.Permissions.AddRange("visualizar,editar,deletar,cadastrar".Split(','));
                ListaRotas.Add(rota);

                rota = new RouteViewModel();
                rota.Name = "Cadastro Grupo";
                rota.Path = "/grupo/cadastro";
                rota.Permissions = new List<string>();
                rota.Permissions.AddRange("visualizar,editar,deletar,cadastrar".Split(','));
                ListaRotas.Add(rota);

                rota = new RouteViewModel();
                rota.Name = "Menu";
                rota.Path = "/menu/index";
                rota.Permissions = new List<string>();
                rota.Permissions.AddRange("visualizar,editar,deletar,cadastrar".Split(','));
                ListaRotas.Add(rota);

                rota = new RouteViewModel();
                rota.Name = "Cadastro Menu";
                rota.Path = "/menu/cadastro";
                rota.Permissions = new List<string>();
                rota.Permissions.AddRange("visualizar,editar,deletar,cadastrar".Split(','));
                ListaRotas.Add(rota);

                rota = new RouteViewModel();
                rota.Name = "Histórico de Acessos";
                rota.Path = "/historicoacesso/index";
                rota.Permissions = new List<string>();
                rota.Permissions.AddRange("visualizar,editar,deletar,cadastrar".Split(','));
                ListaRotas.Add(rota);
            }

            return ListaRotas;
        }

        public static LoginViewModel LoginObra(dynamic param)
        {
            int idUsuario = param.idUsuario;
            int idObra = param.idObra;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            obra_colaborador objObraColaborador = context.obra_colaborador.FirstOrDefault(oc => oc.oco_id_obra == idObra && oc.oco_id_colaborador == idUsuario);

            LoginViewModel data = new LoginViewModel();

            data.Routes = ObterRotas((int)objObraColaborador.oco_id_grupo);

            data.Usuario = new UsuarioViewModel
            {
                Email = objObraColaborador.colaborador.col_ds_email,
                Id = objObraColaborador.colaborador.col_id_colaborador,
                Senha = Seguranca.DecryptTripleDES(objObraColaborador.colaborador.col_ds_senha),
                NomeUsuario = objObraColaborador.colaborador.col_nm_colaborador,
                IdGrupo = objObraColaborador.oco_id_grupo
            };


            data.ObraColaborador = new ObraColaboradorViewModel
            {
                IdObraColaborador = objObraColaborador.oco_id_obra_colaborador,
                NomeObra = objObraColaborador.obra.obr_ds_obra,
                NomeColaborador = objObraColaborador.colaborador.col_nm_colaborador,
                IdObra = objObraColaborador.oco_id_obra,
                IdColaborador = objObraColaborador.oco_id_colaborador,
                IdGrupo = objObraColaborador.oco_id_grupo,
                IdCargo = objObraColaborador.oco_id_cargo,
                ContratanteContratada = objObraColaborador.grupo == null ? "" : objObraColaborador.grupo.gru_nm_nome.ToLower().Contains("contratante") ? "t" : "d",
                DataContratacao = objObraColaborador.oco_dt_contratacao ?? DateTime.MinValue,
                TipoLicencaColaboradorGrupo = objObraColaborador.grupo == null ? "" : objObraColaborador.grupo.gru_nm_nome.ToLower().Contains("basica") ? "basica" : "gratuita",
                IdLicenca = objObraColaborador.grupo.gru_id_licenca ?? int.MinValue
            };

            data.Obra = new ObraViewModel
            {
                IdObra = objObraColaborador.oco_id_obra,
                Descricao = objObraColaborador.obra.obr_ds_obra,
                idDono = objObraColaborador.obra.obr_id_dono ?? 0,
                idContratante = objObraColaborador.obra.obr_id_empresa_contratante ?? 0,
                idContratada = objObraColaborador.obra.obr_id_empresa_contratada ?? 0,
                DataFim = objObraColaborador.obra.obr_dt_fim.ToString() ?? "",
                ObraFinalizada = objObraColaborador.obra.obr_dt_fim == null ? false : (objObraColaborador.obra.obr_dt_fim == DateTime.MinValue.Date ? false : true),
                idColaborador = objObraColaborador.obra.obr_id_colaborador ?? 0
            };

            string statusLicenca = VerificarLicenca(EmpresaModel.ObterRegistro(data.Obra.idDono).Token);

            if (statusLicenca != "ATIVA")
            {
                throw new Exception("Não foi possível realizar o login. A licença dessa empresa está " + statusLicenca);
            }


            data.Menu = ObterMenu((int)objObraColaborador.oco_id_grupo);

            var historico = new HistoricoLogin()
            {
                col_id_colaborador = objObraColaborador.colaborador.col_id_colaborador,
                col_ds_email = objObraColaborador.colaborador.col_ds_email,
                col_nm_colaborador = objObraColaborador.colaborador.col_nm_colaborador,
                col_nr_cpf = objObraColaborador.colaborador.col_nr_cpf,
                obr_id_obra = objObraColaborador.oco_id_obra,
                obr_ds_obra = objObraColaborador.obra.obr_ds_obra,
                data_login = DateTime.Now
            };
            InserirHistoricoLogin(historico);

            return data;
        }


        public static bool ServicoLoja(dynamic param)
        {
            if (param.token == null || string.IsNullOrEmpty(param.token.ToString()))
            {
                throw new Exception("O token não pode ser nulo.");
            }

            if (param.ramo == null || string.IsNullOrEmpty(param.ramo.ToString()))
            {
                throw new Exception("O ramo não pode ser nulo.");
            }

            if (param.setor == null || string.IsNullOrEmpty(param.setor.ToString()))
            {
                throw new Exception("O setor não pode ser nulo.");
            }

            string tipoLicenca = param.tipoLicenca.ToString().ToLower();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            licenca objLicensa = context.licenca.FirstOrDefault(li => li.lic_ds_licenca.ToLower() == tipoLicenca);
            if (objLicensa == null)
            {
                throw new Exception("A licenca selecionada não existe no banco de dados.");
            }

            // verifica se licenca ja existe
            if (param.cnpjEmpresa == null || string.IsNullOrEmpty(param.cnpjEmpresa.ToString()))
            {
                throw new Exception("O CNPJ da empresa deve ser preenchido.");
            }

            if (param.cpf == null || string.IsNullOrEmpty(param.cpf.ToString()))
            {
                throw new Exception("O CPF do cliente deve ser preenchido.");
            }

            string cpf = param.cpf;
            cpf = cpf.Replace(".", "").Replace("-", "");
            string cnpj = param.cnpjEmpresa;
            cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");

            empresa _empresa = context.empresa.Where(x => x.emp_nr_cnpj == cnpj).FirstOrDefault();

            if (_empresa != null)
            {
                if (_empresa.colaborador.col_nr_cpf != cpf)
                {
                    throw new Exception("A empresa selecionada já esta associada a outro cliente.");
                }
            }

            //colaborador

            //fazendo uso de objetos dinamicos para reaproveitar os métodos de salvar e ter eles em apenas um lugar
            dynamic dynObjColaborador = new System.Dynamic.ExpandoObject();
            dynObjColaborador.cpf = param.cpf;
            dynObjColaborador.nome = param.nomeColaborador;
            dynObjColaborador.email = param.emailColaborador;
            dynObjColaborador.telefonePrincipal = param.telefoneColaborador;
            dynObjColaborador.senha = param.senha;
            dynObjColaborador.idColaborador = 0;

            dynObjColaborador.sexo = param.sexo;
            dynObjColaborador.nascimento = param.nascimento;
            dynObjColaborador.logradouro = param.logradouro;
            dynObjColaborador.municipio = param.municipio;
            dynObjColaborador.cep = param.cep;
            dynObjColaborador.bairro = param.bairro;
            dynObjColaborador.complemento = param.complemento;
            dynObjColaborador.numero = param.numero;

            int idColaborador = ColaboradorModel.AddColaboradorServico(dynObjColaborador);

            if (!(idColaborador > 0))
            {
                return false;
            }
            string ramo = param.ramo;
            string setor = param.setor;
            dynamic dynObjEmpresa = new System.Dynamic.ExpandoObject();
            dynObjEmpresa.razaoSocial = param.razaoSocial;
            dynObjEmpresa.nomeFantasia = param.nomeFantasia;
            dynObjEmpresa.cnpj = param.cnpjEmpresa;
            dynObjEmpresa.idColaborador = idColaborador;
            dynObjEmpresa.idEmpresa = 0;
            dynObjEmpresa.idLicenca = objLicensa.lic_id_licenca;

            int idRamo;
            bool success = Int32.TryParse(ramo, out idRamo);

            if (success)
                dynObjEmpresa.idRamo = context.ramo.FirstOrDefault(ram => ram.ram_id_ramo == idRamo).ram_id_ramo;
            else
                dynObjEmpresa.idRamo = context.ramo.FirstOrDefault(ram => ram.ram_id_ramo_loja == ramo).ram_id_ramo;

            int idSetor;
            success = Int32.TryParse(setor, out idSetor);

            if (success)
                dynObjEmpresa.idSetor = context.setor.FirstOrDefault(set => set.set_id_setor == idSetor).set_id_setor;
            else
                dynObjEmpresa.idSetor = context.setor.FirstOrDefault(set => set.set_id_setor_loja == setor).set_id_setor;

            dynObjEmpresa.token = param.token ?? null;

            bool result = EmpresaModel.AddEmpresaServico(dynObjEmpresa);

            return result;
        }

        public static string VerificarLicenca(string token)
        {
            if (ConfigurationManager.AppSettings["DisableAssinatura"] != null)
            {
                if (ConfigurationManager.AppSettings["DisableAssinatura"].ToString().ToLower() == "true")
                {
                    return "ATIVA";
                }
            }

            if (String.IsNullOrEmpty(token))
            {
                throw new Exception("Não foi possível realizar o login pois não há um token de licença associado a essa empresa. Favor verificar a condiçao da assinatura na loja.");
            }

            //modificar o prefixo do link para o valor do parametro BaseUrl
            var request = (HttpWebRequest)WebRequest.Create(String.Format(ConfigurationManager.AppSettings["WSAssinatura"], token));
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            dynamic obj = new StreamReader(response.GetResponseStream()).ReadToEnd();
            object obj1 = new StreamReader(response.GetResponseStream()).ReadToEnd();
            ServicoLojaObj rs = JsonConvert.DeserializeObject<ServicoLojaObj>(responseString);

            return rs.subscription.ToUpper() == "ATIVA" ? "ATIVA" : rs.subscription.ToString().ToLower();
        }

        public static LoginViewModel AcessoConvidada(dynamic param)
        {
            //colaborador
            if (param.idObra == null)
            {
                throw new Exception("Parametro da obra não foi definido");
            }
                       
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            string convite = param.idObra;
            obra _obra = context.obra.FirstOrDefault(obr => obr.obr_cd_convite == convite);
            if (_obra == null)
            {
                throw new Exception("Este convite já foi utilizado ou não existe.");
            }
            
            int idObra = Convert.ToInt32(_obra.obr_id_obra);
            int idLicenca = _obra.obra_colaborador.FirstOrDefault(col => col.grupo.licenca != null).grupo.licenca.lic_id_licenca;
            int tipoConvite = 0;

            if (_obra.empresa1 == null)
            {
                tipoConvite = 1;
            }
            else if (_obra.empresa2 == null)
            {
                tipoConvite = 2;
            }
            else
            {
                throw new Exception("A obra já possui as duas empresas");
            }

            grupo grupoUsuarioConvidado = context.grupo.FirstOrDefault(gru => gru.gru_st_contratante == tipoConvite && gru.gru_st_diretor == 1 && gru.gru_id_licenca == idLicenca);
            if (grupoUsuarioConvidado == null)
            {
                throw new Exception("O grupo não pode ser identificado.");
            }

            //dynamic dynObjColaborador = new System.Dynamic.ExpandoObject();
            //dynObjColaborador.cpf = param.cpf;
            //dynObjColaborador.nome = param.nomeColaborador;
            //dynObjColaborador.email = param.emailColaborador;
            //dynObjColaborador.telefonePrincipal = param.telefoneColaborador;
            //dynObjColaborador.senha = param.senha;


            //dynObjColaborador.sexo = param.sexo;
            //dynObjColaborador.dataNascimento = param.dataNascimento;
            //dynObjColaborador.telefoneSecundario = param.telefoneSecundario;
            //dynObjColaborador.crea = param.crea;
            //dynObjColaborador.logradouro = param.logradouro;
            //dynObjColaborador.numeroEndereco = param.numeroEndereco;
            //dynObjColaborador.complemento = param.complemento;
            //dynObjColaborador.bairro = param.bairro;
            //dynObjColaborador.idMunicipio = param.idMunicipio;
            //dynObjColaborador.cep = param.cep;

            //dynObjColaborador.cargo = context.cargo.FirstOrDefault().car_id_cargo;

            //dynObjColaborador.grupo = grupoUsuarioConvidado.gru_id_grupo;

            //dynObjColaborador.idObra = _obra.obr_id_obra;
            //dynObjColaborador.contratanteContratada = tipoConvite;
            //dynObjColaborador.dataContratacao = DateTime.Now.Date;
            //dynObjColaborador.idColaborador = 0;
            //dynObjColaborador.foto = null;
            //dynObjColaborador.fotoAssinatura = null;

            ColaboradorViewModel dynObjColaborador = new ColaboradorViewModel();
            dynObjColaborador.Cpf = param.cpf;
            dynObjColaborador.Nome = param.nomeColaborador;
            dynObjColaborador.Email = param.emailColaborador;
            dynObjColaborador.TelefonePrincipal = param.telefoneColaborador;
            dynObjColaborador.Senha = param.senha;


            dynObjColaborador.Sexo = param.sexo;
            dynObjColaborador.DataNascimento = param.dataNascimento;
            dynObjColaborador.TelefoneSecundario = param.telefoneSecundario;
            dynObjColaborador.Crea = param.crea;
            dynObjColaborador.Logradouro = param.logradouro;
            dynObjColaborador.NumeroEndereco = param.numeroEndereco;
            dynObjColaborador.Complemento = param.complemento;
            dynObjColaborador.Bairro = param.bairro;
            dynObjColaborador.IdMunicipio = param.idMunicipio;
            dynObjColaborador.Cep = param.cep;

            dynObjColaborador.Cargo = param.cargo;

            dynObjColaborador.Grupo = grupoUsuarioConvidado.gru_id_grupo;

            dynObjColaborador.IdObra = _obra.obr_id_obra;
            dynObjColaborador.ContratanteContratada = tipoConvite.ToString();
            dynObjColaborador.DataContratacao = DateTime.Now.Date.ToString();
            dynObjColaborador.IdColaborador = 0;
            dynObjColaborador.Foto = null;
            dynObjColaborador.FotoAssinatura = null;


            ColaboradorViewModel cvm = ColaboradorModel.Salvar(dynObjColaborador, false);


            dynamic dynObjEmpresa = new System.Dynamic.ExpandoObject();
            dynObjEmpresa.razaoSocial = param.razaoSocial;
            dynObjEmpresa.nomeFantasia = param.nomeFantasia;
            dynObjEmpresa.cnpj = param.cnpjEmpresa;
            dynObjEmpresa.idColaborador = Convert.ToInt32(cvm.IdColaborador);
            dynObjEmpresa.idEmpresa = 0;

            dynObjEmpresa.logradouro = param.logradouro;
            dynObjEmpresa.numeroEndereco = param.numeroEndereco;
            dynObjEmpresa.bairro = param.bairro;
            dynObjEmpresa.cep = param.cep;
            dynObjEmpresa.telefone = param.telefone;
            dynObjEmpresa.complemento = param.complemento;
            dynObjEmpresa.idLicenca = idLicenca;

            dynObjEmpresa.idMunicipio = null;
            dynObjEmpresa.idRamo = param.idRamo;
            dynObjEmpresa.idSetor = param.idSetor;

            EmpresaViewModel evm = EmpresaModel.Salvar(dynObjEmpresa, false);

            //obra
            dynamic dynObjObra = new System.Dynamic.ExpandoObject();
            dynObjObra.idObra = _obra.obr_id_obra;
            dynObjObra.idObraColaborador = cvm.IdObraColaborador;

            ObraViewModel ovm = ObraModel.ObterRegistro(dynObjObra);


            bool result =  ObraModel.AtualizarConvidada(evm.idEmpresa, ovm.IdObra, tipoConvite);

            context = new LaudosPiscinasEntities();
            _obra = context.obra.Find(_obra.obr_id_obra);

            _obra.obr_cd_convite = null;
            context.obra.Attach(_obra);
            context.Entry(_obra).State = EntityState.Modified;
            context.SaveChanges();



            return LoginUser(param);
        }

        private static List<RouteViewModel> ObterRotas(int idGrupo)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var ListaRotasTemporaria = context.grupo_pagina_acao.Where(gpa => gpa.gpa_id_grupo == idGrupo).ToList();

            var ListaRotas = new List<RouteViewModel>();

            foreach (var item in ListaRotasTemporaria)
            {
                if (!ListaRotas.Any(x => x.Path == item.pagina_acao.pagina.pag_ds_url))
                {
                    ListaRotas.Add(new RouteViewModel
                    {
                        Name = item.pagina_acao.pagina.pag_nm_titulo,
                        Path = item.pagina_acao.pagina.pag_ds_url,
                        Permissions = (from lp in ListaRotasTemporaria
                                       where lp.pagina_acao.pagina.pag_nm_titulo == item.pagina_acao.pagina.pag_nm_titulo
                                       select lp.pagina_acao.acao == null ? "" : lp.pagina_acao.acao.aca_ds_alias).ToList()

                    });
                }
            }


            RouteViewModel rotaConvidada = new RouteViewModel();
            rotaConvidada.Name = "Convidada";
            rotaConvidada.Path = "/convidada";
            rotaConvidada.Permissions = new List<string>();
            rotaConvidada.Permissions.Add("visualizar");
            ListaRotas.Add(rotaConvidada);

            RouteViewModel rota = new RouteViewModel();
            rota.Name = "Escolher Obra";
            rota.Path = "/obra/escolher";
            rota.Permissions = new List<string>();
            rota.Permissions.Add("visualizar");
            ListaRotas.Add(rota);

            //todo: colocar no banco de dados
            //rotaConvidada.Name = "RelatorioMedicao";
            //rotaConvidada.Path = "/relatoriomedicao";
            //rotaConvidada.Permissions = new List<string>();
            //rotaConvidada.Permissions.Add("visualizar");
            //ListaRotas.Add(rotaConvidada);

            return ListaRotas;
        }

        private static MenuViewModel ObterMenu(int idGrupo)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var Grupo = context.grupo.Find(idGrupo);

            var MenuCompleto = context.menu_pagina.Where(mp => mp.menu.men_id_menu == Grupo.gru_id_menu).ToList();

            var Menu = new MenuViewModel();
            Menu.Id = Grupo.gru_id_menu;
            Menu.Titulo = Grupo.menu.men_nm_titulo;
            Menu.ListaPagina = new List<PaginaViewModel>();

            MenuCompleto.Where(mt => mt.mpa_id_pagina_pai == null).OrderBy(x => x.mpa_vl_ordem).ToList().ForEach(mt =>
            {
                var PaginaPai = new PaginaViewModel();
                PaginaPai.Id = (int)mt.mpa_id_pagina;
                PaginaPai.Titulo = mt.pagina.pag_nm_titulo;
                PaginaPai.Caminho = mt.pagina.pag_ds_url;
                PaginaPai.CssClass = mt.mpa_ds_class;

                PaginaPai.Paginas = ObterPaginasFilho(PaginaPai.Id); // pq não funciona chamando do próprio MenuTemporário ??????

                Menu.ListaPagina.Add(PaginaPai);
            });

            return Menu;
        }

        private static List<PaginaViewModel> ObterPaginasFilho(int idMenuPaginaPai)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            List<PaginaViewModel> paginas = new List<PaginaViewModel>();
            context.menu_pagina.Where(mpa => mpa.mpa_id_pagina_pai == idMenuPaginaPai).ToList().ForEach(mpa =>
            {
                PaginaViewModel pagina = new PaginaViewModel();
                pagina.Id = (int)mpa.mpa_id_pagina;
                pagina.Titulo = mpa.pagina.pag_nm_titulo;
                pagina.Caminho = mpa.pagina.pag_ds_url;
                pagina.CssClass = mpa.mpa_ds_class;
                //pagina.Paginas = ObterPaginasFilho(pagina.Id); //habilita recursividade 
                paginas.Add(pagina);
            });
            return paginas;
        }

        public static void InserirHistoricoLogin(HistoricoLogin historico)
        {
            string insert = @"INSERT INTO historico_login
(col_id_colaborador,
col_nr_cpf,
col_nm_colaborador,
col_ds_email,
obr_id_obra,
obr_ds_obra,
data_login)
VALUES
(@col_id_colaborador,
 @col_nr_cpf,
 @col_nm_colaborador,
 @col_ds_email,
 @obr_id_obra,
 @obr_ds_obra,
 @data_login)";
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            context.Database.ExecuteSqlCommand(insert,
                new MySqlParameter("col_id_colaborador", historico.col_id_colaborador),
                new MySqlParameter("col_nr_cpf", historico.col_nr_cpf),
                new MySqlParameter("col_nm_colaborador", historico.col_nm_colaborador),
                new MySqlParameter("col_ds_email", historico.col_ds_email),
                new MySqlParameter("obr_id_obra", historico.obr_id_obra),
                new MySqlParameter("obr_ds_obra", historico.obr_ds_obra),
                new MySqlParameter("data_login", historico.data_login)
            );
        }
        public static List<HistoricoLogin> GetHistoricoLogin(dynamic filter)
        {
            string select = @"select * from historico_login where 1 = 1 ";

            string col_nr_cpf = (filter.col_nr_cpf != null ? filter.col_nr_cpf : string.Empty);
            string col_nm_colaborador = (filter.col_nm_colaborador != null ? filter.col_nm_colaborador : string.Empty);
            string col_ds_email = (filter.col_ds_email != null ? filter.col_ds_email : string.Empty);
            string data_login = (filter.data_login != null ? filter.data_login : null);

            if (!string.IsNullOrEmpty(col_nr_cpf))
                select += string.Format(" and col_nr_cpf = '{0}' ", col_nr_cpf);
            if (!string.IsNullOrEmpty(col_nm_colaborador))
                select += string.Format(" and col_nm_colaborador like '%{0}%' ", col_nm_colaborador);
            if (!string.IsNullOrEmpty(col_ds_email))
                select += string.Format(" and col_ds_email = '{0}' ", col_ds_email);
            if (!string.IsNullOrEmpty(data_login))
                select += string.Format(" and DATE(data_login) = '{0}' ", DateTime.Parse(data_login).ToString("yyyy-MM-dd"));

            select += " order by data_login desc ";
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            var result = context.Database.SqlQuery<HistoricoLogin>(select).ToList();

            return result;
        }
    }
    public class UsuarioViewModel
    {
        public UsuarioViewModel()
        {
            this.Grupo = new GrupoViewModel();
        }

        public long Id { get; set; }
        public string NomeUsuario { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public long IdGrupo { get; set; }
        public Nullable<long> Status { get; set; }
        public Nullable<long> StatusAlterarSenha { get; set; }

        public virtual GrupoViewModel Grupo { get; set; }
    }
    public class ServicoLojaObj
    {
        public string success { get; set; }
        public string subscription { get; set; }
        public string content_for_layout { get; set; }
        public string scripts_for_layout { get; set; }
        public string title_for_layout { get; set; }
    }
    public class HistoricoLogin
    {
        public long col_id_colaborador { get; set; }
        public string col_nr_cpf { get; set; }
        public string col_nm_colaborador { get; set; }
        public string col_ds_email { get; set; }
        public long? obr_id_obra { get; set; }
        public string obr_ds_obra { get; set; }
        public DateTime? data_login { get; set; }
    }
}