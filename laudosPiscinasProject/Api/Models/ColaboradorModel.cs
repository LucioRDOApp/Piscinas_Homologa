using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace laudosPiscinasProject.Api.Models
{
    public class ColaboradorModel
    {
        public static List<ColaboradorViewModel> Lista(dynamic param)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            int idObra = param.idObra ?? 0;
            if (idObra <= 0)
            {
                throw new Exception("Não é possivel listar colaboradores sem possuir uma obra selecionada.");
            }

            obra _obra = context.obra.Find(idObra);

            string contratanteContratada = param.contratanteContratada ?? 0;
            if (string.IsNullOrEmpty(contratanteContratada))
            {
                throw new Exception("Não é possivel listar colaboradores, Contratante ou contratada está indefinido.");
            }


            //obra _obra = context.obra.Find(idObra);


            List<colaborador> ListaColaboradores = new List<colaborador>();
            List<obra_colaborador> ListaObraColaborador = context.Set<obra_colaborador>()?.ToList();

            ListaObraColaborador = ListaObraColaborador.Where(oco => oco.oco_id_obra == idObra)?.ToList();
            ListaObraColaborador.ForEach(oco => ListaColaboradores.Add(oco.colaborador));

            if (param != null)
            {
                string nome = param.nome.ToString();
                string cpf = param.cpf.ToString();
                string sexo = param.sexo.ToString();
                string email = param.email.ToString();
                string crea = param.crea.ToString();
                int idUF = param.uf ?? 0;
                int idMunicipio = param.idMunicipio ?? 0;


                if (!string.IsNullOrEmpty(nome))
                {
                    ListaColaboradores = ListaColaboradores.Where(col => col.col_nm_colaborador.ToLower().Contains(nome.ToLower()))?.ToList();
                }
                if (!string.IsNullOrEmpty(cpf))
                {
                    cpf = !string.IsNullOrEmpty(cpf) ? cpf.Replace(".", "").Replace("-", "") : cpf;
                    ListaColaboradores = ListaColaboradores.Where(col => col.col_nr_cpf == cpf)?.ToList();
                }
                if (sexo != "S")
                {
                    ListaColaboradores = ListaColaboradores.Where(col => col.col_ds_sexo == sexo)?.ToList();
                }
                if (!string.IsNullOrEmpty(email))
                {
                    ListaColaboradores = ListaColaboradores.Where(col => col.col_ds_email.ToLower().Contains(email.ToLower()))?.ToList();
                }
                if (!string.IsNullOrEmpty(crea))
                {
                    ListaColaboradores = ListaColaboradores.Where(col => col.col_ds_crea != null).ToList();
                    ListaColaboradores = ListaColaboradores.Where(col => col.col_ds_crea.ToLower().Contains(crea.ToLower()))?.ToList();
                }

                if (idUF > 0)
                {
                    ListaColaboradores = ListaColaboradores.Where(col => col.municipio?.mun_id_uf == idUF)?.ToList();
                }
                if (idMunicipio > 0)
                {
                    ListaColaboradores = ListaColaboradores.Where(col => col.col_id_municipio == idMunicipio)?.ToList();
                }


            }

            List<ColaboradorViewModel> Lista = new List<ColaboradorViewModel>();                

            ListaColaboradores.ForEach(col => Lista.Add(new ColaboradorViewModel
            {
                Nome = col.col_nm_colaborador,
                IdColaborador = col.col_id_colaborador,
                Cpf = col.col_nr_cpf,
                Email = col.col_ds_email,
                Senha = Seguranca.DecryptTripleDES(col.col_ds_senha),
                TelefonePrincipal = String.IsNullOrEmpty(col.col_ds_telefone_principal) ? "" : col.col_ds_telefone_principal.Length == 11 ? Int64.Parse(col.col_ds_telefone_principal).ToString("(##) #####-####") : Int64.Parse(col.col_ds_telefone_principal).ToString("(##) ####-####"),
                TelefoneSecundario = String.IsNullOrEmpty(col.col_ds_telefone_secundario) ? "" : col.col_ds_telefone_secundario.Length == 11 ? Int64.Parse(col.col_ds_telefone_secundario).ToString("(##) #####-####") : Int64.Parse(col.col_ds_telefone_secundario).ToString("(##) ####-####"),
                Crea = col.col_ds_crea,
                Login = col.col_ds_login,
                Sexo = col.col_ds_sexo,
                //Foto = col.col_ds_foto,
                DescricaoFoto = col.col_ds_foto,
                //FotoAssinatura = col.col_ds_assinatura,
                DescricaoAssinatura = col.col_ds_assinatura,
                DataContratacao = Convert.ToString(ListaObraColaborador.FirstOrDefault(x => x.oco_id_colaborador == col.col_id_colaborador && x.oco_id_obra == idObra).oco_dt_contratacao),
                Grupo = ListaObraColaborador.FirstOrDefault(x => x.oco_id_colaborador == col.col_id_colaborador && x.oco_id_obra == idObra).oco_id_grupo,
                Cargo = ListaObraColaborador.FirstOrDefault(x => x.oco_id_colaborador == col.col_id_colaborador && x.oco_id_obra == idObra).oco_id_cargo,
                DescricaoCargo = ListaObraColaborador.FirstOrDefault(x => x.oco_id_colaborador == col.col_id_colaborador && x.oco_id_obra == idObra).cargo.car_ds_cargo,
                ContratanteContratada = ListaObraColaborador.FirstOrDefault(x => x.oco_id_colaborador == col.col_id_colaborador && x.oco_id_obra == idObra).oco_st_contratante_contratada,
                DescricaoGrupo = ListaObraColaborador.FirstOrDefault(x => x.oco_id_colaborador == col.col_id_colaborador && x.oco_id_obra == idObra).grupo.gru_nm_nome,
                IdObraColaborador = ListaObraColaborador.FirstOrDefault(x => x.oco_id_colaborador == col.col_id_colaborador && x.oco_id_obra == idObra).oco_id_obra_colaborador,
                Excluivel = VerificaExcluivel(ListaObraColaborador, col, _obra, idObra, contratanteContratada),
                Editavel = VerificaEditavel(ListaObraColaborador, col, _obra, idObra, contratanteContratada)
            }));



            int cargo = param.cargo ?? 0;
            int grupo = param.grupo ?? 0;
            string dataContratacao = Convert.ToString(param.dataContratacao);

            if (cargo > 0)
            {
                Lista = Lista.Where(x => x.Cargo == cargo)?.ToList();
            }
            if (grupo > 0)
            {
                Lista = Lista.Where(x => x.Grupo == grupo)?.ToList();
            }
            if (!string.IsNullOrEmpty(dataContratacao))
            {
                Lista = Lista.Where(x => x.DataContratacao.Length > 0 && x.DataContratacao.Replace("/", "").Substring(0, 8) == dataContratacao)?.ToList();
            }


            string orderby = param.orderby ?? "";
            string orderbydescending = param.orderbydescending ?? "";

            if (!string.IsNullOrEmpty(orderby))
            {
                return Lista.OrderBy(x => x.Nome)?.ToList();
            }
            if (!string.IsNullOrEmpty(orderbydescending))
            {
                return Lista.OrderByDescending(x => x.Nome)?.ToList();
            }

            return Lista;
        }

        internal static List<ColaboradorViewModel> ObterColaboradorNome(ColaboradorViewModel filter)
        {
            var list = new List<ColaboradorViewModel>();
            if (!string.IsNullOrEmpty(filter.Nome))
            {
                using (var context = new LaudosPiscinasEntities())
                {
                    var _list = context.colaborador.Where(c => c.col_nm_colaborador.Contains(filter.Nome))?.ToList();

                    if (_list.Count > 0)
                    {
                        foreach (var item in _list)
                        {
                            list.Add(new ColaboradorViewModel(item));
                        }
                    }
                }
            }
            return list;
            //string nome = param.nome ?? string.Empty;

            //ColaboradorViewModel colaborador = new ColaboradorViewModel();

            //LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            //var resultado = context.colaborador.FirstOrDefault(col => col.col_nm_colaborador.Contains(nome));

            //if (resultado == null)
            //{
            //    return colaborador;
            //}

            //colaborador.IdColaborador = resultado.col_id_colaborador;
            //colaborador.Nome = resultado.col_nm_colaborador;
            //colaborador.Cpf = resultado.col_nr_cpf;
            //colaborador.Sexo = resultado.col_ds_sexo;
            //colaborador.DataNascimento = resultado.col_dt_nascimento == null ? "" : String.Format("{0:dd/MM/yyyy}", resultado.col_dt_nascimento.ToString().Substring(0, 10));
            //colaborador.TelefonePrincipal = resultado.col_ds_telefone_principal;
            //colaborador.TelefoneSecundario = resultado.col_ds_telefone_secundario;
            //colaborador.Email = resultado.col_ds_email;
            //colaborador.Crea = resultado.col_ds_crea;
            ////colaborador.Cargo = resultado.c
            //colaborador.Logradouro = resultado.col_ds_logradouro;
            //colaborador.NumeroEndereco = resultado.col_ds_numero;
            //colaborador.Complemento = resultado.col_ds_complemento;
            //colaborador.Bairro = resultado.col_ds_bairro;
            //colaborador.IdMunicipio = resultado.col_id_municipio ?? 0;
            //colaborador.Uf = resultado.municipio == null ? 0 : resultado.municipio.mun_id_uf;
            //colaborador.Cep = resultado.col_ds_cep;
            ////colaborador.Foto = resultado.col_ds_foto;
            //colaborador.DescricaoFoto = resultado.col_ds_foto;
            ////colaborador.FotoAssinatura = resultado.col_ds_assinatura;
            //colaborador.DescricaoAssinatura = resultado.col_ds_assinatura;
            //colaborador.Senha = Seguranca.DecryptTripleDES(resultado.col_ds_senha);

            //return colaborador;
        }

        public static bool VerificaExcluivel(List<obra_colaborador> ListaObraColaborador, colaborador col, obra _obra, int idObra, string contratanteContratada)
        {
            string colContratanteContratada = ListaObraColaborador.FirstOrDefault(x => x.oco_id_colaborador == col.col_id_colaborador && x.oco_id_obra == idObra).oco_st_contratante_contratada;
            return (colContratanteContratada == contratanteContratada || colContratanteContratada == null) && _obra.obr_id_colaborador != col.col_id_colaborador;
        }

        public static bool VerificaEditavel(List<obra_colaborador> ListaObraColaborador, colaborador col, obra _obra, int idObra, string contratanteContratada)
        {
            string colContratanteContratada = ListaObraColaborador.FirstOrDefault(x => x.oco_id_colaborador == col.col_id_colaborador && x.oco_id_obra == idObra).oco_st_contratante_contratada;
            return (colContratanteContratada == contratanteContratada || colContratanteContratada == null);
        }

        public static int AddColaboradorServico(dynamic param)
        {
            if (string.IsNullOrEmpty(param.nome.ToString()))
            {
                throw new Exception("O nome do cliente deve ser preenchido");
            }
            if (string.IsNullOrEmpty(param.cpf.ToString()))
            {
                throw new Exception("O cpf do cliente deve ser preenchido");
            }
            if (string.IsNullOrEmpty(param.email.ToString()))
            {
                throw new Exception("O e-mail do cliente deve ser preenchido");
            }
            if (string.IsNullOrEmpty(param.telefonePrincipal.ToString()))
            {
                throw new Exception("O telefone principal do cliente deve ser preenchido");
            }


            int idColaborador = param.idColaborador ?? 0;
            string cpf = param.cpf;
            cpf = !string.IsNullOrEmpty(cpf) ? cpf.Replace(".", "").Replace("-", "") : cpf;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            colaborador _colaborador = context.colaborador.Where(x => x.col_id_colaborador == idColaborador).FirstOrDefault() ?? new colaborador();

            if (!string.IsNullOrEmpty(cpf) && context.colaborador.Any(x => x.col_nr_cpf == cpf))
            {
                _colaborador = context.colaborador.FirstOrDefault(x => x.col_nr_cpf == cpf);

                if (_colaborador.col_st_admin == true)
                {
                    throw new Exception("O CPF do cliente já esta associado a um usuário administrador.");
                }
            }

            _colaborador.col_nm_colaborador = param.nome;
            _colaborador.col_nr_cpf = cpf;
            string telefonePrincipal = param.telefonePrincipal;
            telefonePrincipal = !string.IsNullOrEmpty(telefonePrincipal) ? telefonePrincipal.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : telefonePrincipal;
            _colaborador.col_ds_telefone_principal = telefonePrincipal;
            _colaborador.col_ds_email = param.email;
            _colaborador.col_ds_senha = Seguranca.EncryptTripleDES(Convert.ToString(param.senha));

            if (param.sexo != null && (param.sexo.ToString().ToUpper() == "M" || param.sexo.ToString().ToUpper() == "F"))
            {
                _colaborador.col_ds_sexo = param.sexo;
            }

            if (param.nascimento != null)
            {
                DateTimeFormatInfo br = new CultureInfo("pt-BR", false).DateTimeFormat;
                string nascimento = param.nascimento;
                _colaborador.col_dt_nascimento = Convert.ToDateTime(nascimento, br);
            }

            if (param.logradouro != null)
            {
                string logradouro = param.logradouro;
                _colaborador.col_ds_logradouro = logradouro.Length > 50 ? logradouro.Substring(0, 50) : logradouro;
            }

            if (param.municipio != null)
            {
                string municipio = param.municipio;
                municipio mun = context.municipio.Where(m => m.mun_ds_municipio.ToLower() == municipio.ToLower()).FirstOrDefault();
                if (mun != null)
                {
                    _colaborador.col_id_municipio = mun.mun_id_municipio;
                }
            }

            if (param.cep != null)
            {
                string cep = param.cep;
                _colaborador.col_ds_cep = cep.Replace("-", string.Empty);
            }

            if (param.bairro != null)
            {
                string bairro = param.bairro;
                _colaborador.col_ds_bairro = bairro.Length > 50 ? bairro.Substring(0, 50) : bairro;
            }
            if (param.complemento != null)
            {
                string complemento = param.complemento;
                _colaborador.col_ds_complemento = complemento.Length > 50 ? complemento.Substring(0, 50) : complemento;
            }
            if (param.numero != null)
            {
                string numero = param.numero;
                _colaborador.col_ds_numero = numero.Length > 5 ? numero.Substring(0, 5) : numero;
            }


            if (_colaborador.col_id_colaborador > 0)
            {
                context.colaborador.Attach(_colaborador);
                context.Entry(_colaborador).State = EntityState.Modified;
            }
            else
            {
                context.colaborador.Add(_colaborador);
            }

            bool result;
            result = context.SaveChanges() > 0;

            return _colaborador.col_id_colaborador;
        }

        public static ColaboradorViewModel Salvar(ColaboradorViewModel param, bool substituir)
        {
            if (string.IsNullOrEmpty(param.Nome.ToString()))
            {
                throw new Exception("O nome do colaborador deve ser preenchido");
            }
            //if (string.IsNullOrEmpty(param.cpf.ToString()))
            //{
            //    throw new Exception("O cpf do colaborador deve ser preenchido");
            //}
            //if (string.IsNullOrEmpty(param.email.ToString()))
            //{
            //    throw new Exception("O e-mail do colaborador deve ser preenchido");
            //}
            //if (string.IsNullOrEmpty(param.telefonePrincipal.ToString()))
            //{
            //    throw new Exception("O telefone principal do colaborador deve ser preenchido");
            //}

            if (string.IsNullOrEmpty(param.Cargo.ToString()) || param.Cargo.ToString() == "0")
            {
                throw new Exception("O cargo do colaborador deve ser preenchido");
            }
            if (string.IsNullOrEmpty(param.Grupo.ToString()) || param.Grupo.ToString() == "0")
            {
                throw new Exception("O perfil do colaborador deve ser preenchido");
            }


            int idObra = param.IdObra ?? 0;
            int idColaborador = param.IdColaborador;

            string cpf = param.Cpf;
            cpf = !string.IsNullOrEmpty(cpf) ? cpf.Replace(".", "").Replace("-", "") : cpf;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            colaborador _colaborador = context.colaborador.Where(x => x.col_id_colaborador == idColaborador).FirstOrDefault() ?? new colaborador();

            if (!string.IsNullOrEmpty(cpf) && context.colaborador.Any(x => x.col_nr_cpf == cpf))
            {
                _colaborador = context.colaborador.FirstOrDefault(x => x.col_nr_cpf == cpf);
                if (!substituir)
                {
                    return CriarObraColaborador(idObra, _colaborador, param);
                }
            }

            _colaborador.col_nm_colaborador = param.Nome;
            _colaborador.col_nr_cpf = cpf;
            _colaborador.col_ds_sexo = param.Sexo;
            if (param.DataNascimento != null && param.DataNascimento != "__/__/____")
            {
                _colaborador.col_dt_nascimento = String.IsNullOrEmpty(param.DataNascimento.ToString()) ? default(DateTime?) : Convert.ToDateTime(Convert.ToString(param.DataNascimento));
            }
            string telefonePrincipal = param.TelefonePrincipal;
            telefonePrincipal = !string.IsNullOrEmpty(telefonePrincipal) ? telefonePrincipal.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("_", "") : telefonePrincipal;
            _colaborador.col_ds_telefone_principal = telefonePrincipal;
            string telefoneSecundario = param.TelefoneSecundario;
            telefoneSecundario = !string.IsNullOrEmpty(telefoneSecundario) ? telefoneSecundario.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("_", "") : telefoneSecundario;
            _colaborador.col_ds_telefone_secundario = telefoneSecundario;
            _colaborador.col_ds_email = param.Email;
            _colaborador.col_ds_crea = param.Crea;
            _colaborador.col_ds_logradouro = param.Logradouro;
            _colaborador.col_ds_numero = param.NumeroEndereco;
            _colaborador.col_ds_complemento = param.Complemento;
            _colaborador.col_ds_bairro = param.Bairro;
            if (param.IdMunicipio > 0)
            {
                _colaborador.col_id_municipio = param.IdMunicipio;
            }

            string cep = param.Cep;
            cep = !string.IsNullOrEmpty(cep) ? cep.Replace("-", "").Replace(".", "") : cep;
            _colaborador.col_ds_cep = cep;
            _colaborador.col_ds_senha = !string.IsNullOrEmpty(param.Senha) ? Seguranca.EncryptTripleDES(Convert.ToString(param.Senha)) : Seguranca.EncryptTripleDES("123456");

            if (_colaborador.col_id_colaborador > 0)
            {
                context.colaborador.Attach(_colaborador);
                context.Entry(_colaborador).State = EntityState.Modified;
            }
            else
            {
                context.colaborador.Add(_colaborador);
            }

            bool result;

            try
            {
                result = context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return CriarObraColaborador(idObra, _colaborador, param);
        }

        public static ColaboradorViewModel CriarObraColaborador(int idObra, colaborador _colaborador, ColaboradorViewModel param)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            obra_colaborador _obra_colaborador = context.obra_colaborador.Where(x => x.oco_id_colaborador == _colaborador.col_id_colaborador && x.oco_id_obra == idObra).FirstOrDefault() ?? new obra_colaborador();

            _obra_colaborador.oco_id_colaborador = _colaborador.col_id_colaborador;

            //_obra_colaborador.oco_st_dono = param.stDono;
            _obra_colaborador.oco_id_obra = idObra;
            string dataContratacao = param.DataContratacao == null ? "" : param.DataContratacao.ToString();
            if (!String.IsNullOrEmpty(dataContratacao) && dataContratacao != "__/__/____")
            {
                _obra_colaborador.oco_dt_contratacao = Convert.ToDateTime(Convert.ToString(param.DataContratacao));
            }
            _obra_colaborador.oco_id_cargo = param.Cargo;
            _obra_colaborador.oco_id_grupo = param.Grupo;
            _obra_colaborador.oco_id_colaborador = _colaborador.col_id_colaborador;

            grupo _grupo = context.grupo.Find(param.Grupo);
            if (_grupo.gru_st_contratante == 1)
            {
                _obra_colaborador.oco_st_contratante_contratada = "t";
            }
            else if (_grupo.gru_st_contratante == 2)
            {
                _obra_colaborador.oco_st_contratante_contratada = "d";
            }

            if (_obra_colaborador.oco_id_obra_colaborador > 0)
            {
                context.obra_colaborador.Attach(_obra_colaborador);
                context.Entry(_obra_colaborador).State = EntityState.Modified;
            }
            else
            {
                context.obra_colaborador.Add(_obra_colaborador);
            }

            context.SaveChanges();

            dynamic MyDynamic = new System.Dynamic.ExpandoObject();
            MyDynamic.id = _colaborador.col_id_colaborador;
            MyDynamic.idObra = _obra_colaborador.oco_id_obra;
            ColaboradorViewModel cvm = ObterRegistro(MyDynamic);

            return cvm;
        }

        public static bool AtualizarSenha(dynamic param)
        {
            int idColaborador = (int)param.Id;
            string senha = Convert.ToString(param.senha);

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            colaborador _colaborador = context.colaborador.Where(x => x.col_id_colaborador == idColaborador).FirstOrDefault() ?? new colaborador();
            _colaborador.col_ds_senha = Seguranca.EncryptTripleDES(senha);


            if (_colaborador.col_id_colaborador > 0)
            {
                context.colaborador.Attach(_colaborador);
                context.Entry(_colaborador).State = EntityState.Modified;
            }
            else
            {
                context.colaborador.Add(_colaborador);
            }

            bool result = context.SaveChanges() > 0;
            return result;
        }

        public static string RecuperarSenha(dynamic param)
        {
            string email = Convert.ToString(param.email);
            string cpf = param.cpf;
            cpf = !string.IsNullOrEmpty(cpf) ? cpf.Replace(".", "").Replace("-", "") : cpf;


            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            colaborador _colaborador = context.colaborador.Where(x => x.col_ds_email == email && x.col_nr_cpf == cpf).FirstOrDefault() ?? new colaborador();

            if (_colaborador.col_id_colaborador > 0)
            {
                List<string> emails = new List<string>();
                string senha = Seguranca.DecryptTripleDES(_colaborador.col_ds_senha);
                emails.Add(_colaborador.col_ds_email);

                try
                {
                    MailClass.Enviar("RDO App", "RDO App - Recuperar Senha", string.Format("<p>Prezado (a) usuário (a) <strong>{0}</strong>, <br><br> Sua senha atual é  <strong>{1}</strong><br> Caso não tenha feito essa solicitação favor escrever um email para <a href='mailto:suporte@rdoapp.com.br'>suporte@rdoapp.com.br</a> com o título “NÃO SOLICITEI SENHA”.</p><p> Atenciosamente,   </p>", _colaborador.col_nm_colaborador, senha), emails, "~/Assets/images/logo.png");
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            else
            {
                if (cpf != "" || email != "")
                {

                    throw new Exception("CPF e/ou E-mail inválidos.");

                }
                if (email == "")
                {

                    throw new Exception("Email Cadastrado: Preenchimento Obrigatório");

                }
                else if (cpf == "" || cpf == "00000000000")
                {
                    throw new Exception("CPF: Preenchimento Obrigatório");
                }
            }

            bool result = context.SaveChanges() > 0;
            return email;
        }

        public static bool AssociarColaboradorTarefa(int idObra, int idColaborador, int idTarefa)
        {

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            colaborador _colaborador = context.colaborador.Where(x => x.col_id_colaborador == idColaborador).FirstOrDefault() ?? new colaborador();
            obra_colaborador _obra_colaborador = context.obra_colaborador.Where(x => x.oco_id_colaborador == idColaborador && x.oco_id_obra == idObra).FirstOrDefault() ?? new obra_colaborador();
            obra_tarefa_colaborador _obra_tarefa_colaborador = context.obra_tarefa_colaborador.Where(x => x.otc_id_obra_colaborador == _obra_colaborador.oco_id_obra_colaborador && x.otc_id_tarefa == idTarefa).FirstOrDefault() ?? new obra_tarefa_colaborador();

            _obra_tarefa_colaborador.otc_id_tarefa = idTarefa;
            _obra_tarefa_colaborador.otc_id_obra_colaborador = _obra_colaborador.oco_id_obra_colaborador;


            if (_obra_tarefa_colaborador.otc_id_obra_tarefa_colaborador > 0)
            {
                context.obra_tarefa_colaborador.Attach(_obra_tarefa_colaborador);
                context.Entry(_obra_tarefa_colaborador).State = EntityState.Modified;
            }
            else
            {
                context.obra_tarefa_colaborador.Add(_obra_tarefa_colaborador);
            }

            bool result = context.SaveChanges() > 0;
            return result;
        }

        internal static bool Deletar(dynamic param)
        {
            int idColaborador = (int)param.idColaborador;
            int idObra = (int)param.idObra;
            //int idTarefa = (int)param.idTarefa;

            ColaboradorViewModel colaborador = new ColaboradorViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            try
            {
                // os colaboradores não serão excluidos só os relacionamentos, já deixei pronto para quando mudar.

                context.obra_colaborador.Where(x => x.oco_id_colaborador == idColaborador && x.oco_id_obra == idObra).ToList().ForEach(y => context.obra_colaborador.Remove(y));
                //context.obra_tarefa_colaborador.Where(x => x.ote_id_tarefa == idTarefa).ToList().ForEach(y => context.obra_tarefa_colaborador.Remove(y));
                //context.colaborador.Where(x => x.col_id_colaborador == idColaborador).ToList().ForEach(y => context.colaborador.Remove(y));


                return context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return false;
            }
        }

        internal static ColaboradorViewModel ObterColaboradorCPF(dynamic param)
        {
            string cpf = param.cpf ?? string.Empty;

            ColaboradorViewModel colaborador = new ColaboradorViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var resultado = context.colaborador.FirstOrDefault(col => col.col_nr_cpf == cpf);

            if (resultado == null)
            {
                return colaborador;
            }

            colaborador.IdColaborador = resultado.col_id_colaborador;
            colaborador.Nome = resultado.col_nm_colaborador;
            colaborador.Cpf = resultado.col_nr_cpf;
            colaborador.Sexo = resultado.col_ds_sexo;
            colaborador.DataNascimento = resultado.col_dt_nascimento == null ? "" : String.Format("{0:dd/MM/yyyy}", resultado.col_dt_nascimento.ToString().Substring(0, 10));
            colaborador.TelefonePrincipal = resultado.col_ds_telefone_principal;
            colaborador.TelefoneSecundario = resultado.col_ds_telefone_secundario;
            colaborador.Email = resultado.col_ds_email;
            colaborador.Crea = resultado.col_ds_crea;
            //colaborador.Cargo = resultado.c
            colaborador.Logradouro = resultado.col_ds_logradouro;
            colaborador.NumeroEndereco = resultado.col_ds_numero;
            colaborador.Complemento = resultado.col_ds_complemento;
            colaborador.Bairro = resultado.col_ds_bairro;
            colaborador.IdMunicipio = resultado.col_id_municipio ?? 0;
            colaborador.Uf = resultado.municipio == null ? 0 : resultado.municipio.mun_id_uf;
            colaborador.Cep = resultado.col_ds_cep;
            //colaborador.Foto = resultado.col_ds_foto;
            colaborador.DescricaoFoto = resultado.col_ds_foto;
            //colaborador.FotoAssinatura = resultado.col_ds_assinatura;
            colaborador.DescricaoAssinatura = resultado.col_ds_assinatura;
            colaborador.Senha = Seguranca.DecryptTripleDES(resultado.col_ds_senha);

            return colaborador;
        }

        internal static ColaboradorViewModel ObterRegistro(dynamic param)
        {
            int idColaborador = param.id ?? 0;
            int idObra = param.idObra ?? 0;

            ColaboradorViewModel colaborador = new ColaboradorViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var resultado = context.colaborador.FirstOrDefault(col => col.col_id_colaborador == idColaborador);

            colaborador.IdColaborador = resultado.col_id_colaborador;
            colaborador.Nome = resultado.col_nm_colaborador;
            colaborador.Cpf = resultado.col_nr_cpf;
            colaborador.Sexo = resultado.col_ds_sexo;
            colaborador.DataNascimento = resultado.col_dt_nascimento == null ? "" : String.Format("{0:dd/MM/yyyy}", resultado.col_dt_nascimento.ToString().Substring(0, 10));
            colaborador.TelefonePrincipal = resultado.col_ds_telefone_principal;
            colaborador.TelefoneSecundario = resultado.col_ds_telefone_secundario;
            colaborador.Email = resultado.col_ds_email;
            colaborador.Crea = resultado.col_ds_crea;
            colaborador.Logradouro = resultado.col_ds_logradouro;
            colaborador.NumeroEndereco = resultado.col_ds_numero;
            colaborador.Complemento = resultado.col_ds_complemento;
            colaborador.Bairro = resultado.col_ds_bairro;
            colaborador.IdMunicipio = resultado.col_id_municipio ?? 0;
            colaborador.Uf = resultado.municipio == null ? 0 : resultado.municipio.mun_id_uf;
            colaborador.Cep = resultado.col_ds_cep;
            //colaborador.Foto = resultado.col_ds_foto;
            colaborador.DescricaoFoto = resultado.col_ds_foto;
            //colaborador.FotoAssinatura = resultado.col_ds_assinatura;
            colaborador.DescricaoAssinatura = resultado.col_ds_assinatura;
            colaborador.Senha = Seguranca.DecryptTripleDES(resultado.col_ds_senha);

            obra_colaborador colaboradorObra = context.obra_colaborador.FirstOrDefault(oco => oco.oco_id_colaborador == idColaborador && oco.oco_id_obra == idObra) ?? new obra_colaborador();

            colaborador.Grupo = colaboradorObra.oco_id_grupo;
            colaborador.Cargo = colaboradorObra.oco_id_cargo;
            colaborador.DataContratacao = colaboradorObra.oco_dt_contratacao == null ? "" : String.Format("{0:dd/MM/yyyy}", Convert.ToString(colaboradorObra.oco_dt_contratacao).Substring(0, 10));
            colaborador.IdObraColaborador = colaboradorObra.oco_id_obra_colaborador;
            colaborador.ContratanteContratada = colaboradorObra.oco_st_contratante_contratada;
            colaborador.DescricaoGrupo = colaboradorObra.grupo.gru_nm_nome;

            return colaborador;
        }

        internal static ColaboradorViewModel ObterColaboradorDoSistema(int idColaborador)
        {
            ColaboradorViewModel colaborador = new ColaboradorViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var resultado = context.colaborador.FirstOrDefault(col => col.col_id_colaborador == idColaborador);

            colaborador.IdColaborador = resultado.col_id_colaborador;
            colaborador.Nome = resultado.col_nm_colaborador;
            colaborador.Cpf = resultado.col_nr_cpf;
            colaborador.Sexo = resultado.col_ds_sexo;
            colaborador.DataNascimento = resultado.col_dt_nascimento == null ? "" : String.Format("{0:dd/MM/yyyy}", resultado.col_dt_nascimento.ToString().Substring(0, 10));
            colaborador.TelefonePrincipal = resultado.col_ds_telefone_principal;
            colaborador.TelefoneSecundario = resultado.col_ds_telefone_secundario;
            colaborador.Email = resultado.col_ds_email;
            colaborador.Crea = resultado.col_ds_crea;
            colaborador.Logradouro = resultado.col_ds_logradouro;
            colaborador.NumeroEndereco = resultado.col_ds_numero;
            colaborador.Complemento = resultado.col_ds_complemento;
            colaborador.Bairro = resultado.col_ds_bairro;
            colaborador.IdMunicipio = resultado.col_id_municipio ?? 0;
            colaborador.Uf = resultado.municipio == null ? 0 : resultado.municipio.mun_id_uf;
            colaborador.Cep = resultado.col_ds_cep;
            //colaborador.Foto = resultado.col_ds_foto;
            colaborador.DescricaoFoto = resultado.col_ds_foto;
            //colaborador.FotoAssinatura = resultado.col_ds_assinatura;
            colaborador.DescricaoAssinatura = resultado.col_ds_assinatura;
            colaborador.Senha = Seguranca.DecryptTripleDES(resultado.col_ds_senha);
            
            return colaborador;
        }

        internal static ColaboradorViewModel ObterRegistro(int idObra, int idColaborador)
        {
            ColaboradorViewModel colaborador = new ColaboradorViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            colaborador resultado = context.colaborador.FirstOrDefault(col => col.col_id_colaborador == idColaborador);

            colaborador.IdColaborador = resultado.col_id_colaborador;
            colaborador.Nome = resultado.col_nm_colaborador;
            colaborador.Cpf = resultado.col_nr_cpf;
            colaborador.Sexo = resultado.col_ds_sexo;
            colaborador.DataNascimento = resultado.col_dt_nascimento == null ? "" : String.Format("{0:dd/MM/yyyy}", Convert.ToString(resultado.col_dt_nascimento));
            colaborador.TelefonePrincipal = string.IsNullOrEmpty(resultado.col_ds_telefone_principal) ? "" : resultado.col_ds_telefone_principal.Length == 11 ? Int64.Parse(resultado.col_ds_telefone_principal).ToString("(##) #####-####") : Int64.Parse(resultado.col_ds_telefone_principal).ToString("(##) ####-####");
            colaborador.TelefoneSecundario = string.IsNullOrEmpty(resultado.col_ds_telefone_secundario) ? "" : resultado.col_ds_telefone_secundario.Length == 11 ? Int64.Parse(resultado.col_ds_telefone_secundario).ToString("(##) #####-####") : Int64.Parse(resultado.col_ds_telefone_secundario).ToString("(##) ####-####");
            colaborador.Email = resultado.col_ds_email;
            colaborador.Crea = resultado.col_ds_crea;
            colaborador.Logradouro = resultado.col_ds_logradouro;
            colaborador.NumeroEndereco = resultado.col_ds_numero;
            colaborador.Complemento = resultado.col_ds_complemento;
            colaborador.Bairro = resultado.col_ds_bairro;
            colaborador.IdMunicipio = resultado.col_id_municipio ?? 0;
            colaborador.Uf = resultado.municipio == null ? 0 : resultado.municipio.mun_id_uf;
            colaborador.Cep = resultado.col_ds_cep;
            //colaborador.Foto = resultado.col_ds_foto;
            colaborador.DescricaoFoto = resultado.col_ds_foto;
            //colaborador.FotoAssinatura = resultado.col_ds_assinatura;
            colaborador.DescricaoAssinatura = resultado.col_ds_assinatura;
            colaborador.Senha = Seguranca.DecryptTripleDES(resultado.col_ds_senha);

            obra_colaborador colaboradorObra = context.obra_colaborador.FirstOrDefault(oco => oco.oco_id_colaborador == idColaborador && oco.oco_id_obra == idObra);

            colaborador.Grupo = colaboradorObra.oco_id_grupo;
            colaborador.Cargo = colaboradorObra.oco_id_cargo;
            colaborador.DataContratacao = colaboradorObra.oco_dt_contratacao == null ? "" : String.Format("{0:dd/MM/yyyy}", Convert.ToString(colaboradorObra.oco_dt_contratacao));
            colaborador.IdObraColaborador = colaboradorObra.oco_id_obra_colaborador;
            colaborador.DescricaoCargo = colaboradorObra.cargo.car_ds_cargo;
            colaborador.DescricaoGrupo = colaboradorObra.grupo.gru_nm_nome;
            colaborador.ContratanteContratada = colaboradorObra.oco_st_contratante_contratada;

            return colaborador;
        }

        public static List<ColaboradorViewModel> ObterColaboradorPorPerfil(int idPerfil)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            IQueryable<colaborador> colaboradores = context.colaborador;
            if (idPerfil > 0)
            {
                colaboradores = colaboradores.Where(col => col.obra_colaborador.Any(oco => oco.oco_id_grupo == idPerfil));
            }

            List<ColaboradorViewModel> Lista = new List<ColaboradorViewModel>();
            colaboradores.ToList().ForEach(oco => Lista.Add(new ColaboradorViewModel
            {
                Nome = oco.col_nm_colaborador,
                IdColaborador = oco.col_id_colaborador,
                Cpf = oco.col_nr_cpf,
                Email = oco.col_ds_email,
                Senha = Seguranca.DecryptTripleDES(oco.col_ds_senha),
                TelefonePrincipal = String.IsNullOrEmpty(oco.col_ds_telefone_principal) ? "" : oco.col_ds_telefone_principal.Length == 11 ? Int64.Parse(oco.col_ds_telefone_principal).ToString("(##) #####-####") : Int64.Parse(oco.col_ds_telefone_principal).ToString("(##) ####-####"),
                TelefoneSecundario = String.IsNullOrEmpty(oco.col_ds_telefone_secundario) ? "" : oco.col_ds_telefone_secundario.Length == 11 ? Int64.Parse(oco.col_ds_telefone_secundario).ToString("(##) #####-####") : Int64.Parse(oco.col_ds_telefone_secundario).ToString("(##) ####-####"),
                Crea = oco.col_ds_crea,
                Login = oco.col_ds_login,
                Sexo = oco.col_ds_sexo,
                DescricaoFoto = oco.col_ds_foto,
                DescricaoAssinatura = oco.col_ds_assinatura
            }));
            return Lista.OrderBy(col => col.Nome).ToList();
        }
    }
    public class ColaboradorViewModel
    {
        public int IdColaborador { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string TelefonePrincipal { get; set; }
        public string TelefoneSecundario { get; set; }
        public string Email { get; set; }
        public string Crea { get; set; }
        public string Senha { get; set; }
        public string ConfirmacaoSenha { get; set; }
        public string Login { get; set; }
        public string Foto { get; set; }
        public string FotoAssinatura { get; set; }
        public string DescricaoFoto { get; set; }
        public string DescricaoAssinatura { get; set; }
        public string Assinatura { get; set; }
        public string Sexo { get; set; }
        public string Cep { get; set; }
        public string DataNascimento { get; set; }

        public string Logradouro { get; set; }
        public string NumeroEndereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public int IdMunicipio { get; set; }
        public int Uf { get; set; }

        //dados do relacionamento obra colaborador
        public int Grupo { get; set; }
        public string DescricaoGrupo { get; set; }
        public int Cargo { get; set; }
        public string DescricaoCargo { get; set; }
        public string DataContratacao { get; set; }
        public int IdObraColaborador { get; set; }
        public int? IdObra { get; set; }
        public bool Marcado { get; set; }
        public string ContratanteContratada { get; set; }
        public bool Excluivel { get; set; }
        public bool Editavel { get; set; }
        public ColaboradorViewModel() { }
        public ColaboradorViewModel(dynamic param)
        {
            if (param != null)
            {
                IdColaborador = param.idColaborador ?? 0;
                Nome = param.nome;
                Cpf = param.cpf;
                TelefonePrincipal = param.telefonePrincipal;
                TelefoneSecundario = param.telefoneSecundario;
                Senha = param.senha;
                DataNascimento = param.dataNascimento;
                IdMunicipio = string.IsNullOrEmpty(Convert.ToString(param.idMunicipio)) ? 0 : param.idMunicipio;
                Uf = string.IsNullOrEmpty(Convert.ToString(param.uf)) ? 0 : param.uf;
                Grupo = param.grupo;
                DescricaoGrupo = param.descricaoGrupo;
                Cargo = param.cargo;
                DescricaoCargo = param.descricaoCargo;
                DataContratacao = param.dataContratacao;
                IdObraColaborador = param.idObraColaborador ?? 0;
                Marcado = param.marcado ?? false;
                Excluivel = param.excluivel;
                Editavel = param.editavel ?? false;
                IdObra = param.idObra;
                ContratanteContratada = param.contratanteContratada;
                Email = param.email;
                Logradouro = param.logradouro;
                Crea = param.crea;
                NumeroEndereco = param.numeroEndereco;
                Complemento = param.complemento;
                Bairro = param.bairro;
                Cep = param.cep;
            }
        }
        public ColaboradorViewModel(colaborador entity)
        {
            if (entity != null)
            {
                IdColaborador = entity.col_id_colaborador;
                Nome = entity.col_nm_colaborador;
            }
        }
    }
}