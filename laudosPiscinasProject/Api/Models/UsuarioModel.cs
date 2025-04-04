using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laudosPiscinasProject.Api.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public GrupoModel Grupo { get; set; }

        public static List<UsuarioViewModel> ObterUsuarios(dynamic param)
        {
            string email = param.email;
            string senha = param.senha;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var ListaUsuarios = new List<UsuarioViewModel>();
            context.usuario.Where(u => u.usu_ds_email.ToLower().Contains(email) && u.usu_ds_senha.ToLower().Contains(senha))
                .ToList().ForEach(
                    usu => ListaUsuarios.Add(new UsuarioViewModel
                    {
                        Email = usu.usu_ds_email,
                        Id = usu.usu_id_usuario,
                        IdGrupo = usu.usu_id_grupo,
                        Grupo = new GrupoViewModel
                        {
                            Id = usu.grupo.gru_id_grupo,
                            Nome = usu.grupo.gru_nm_nome,
                            IdMenu = usu.grupo.gru_id_menu
                        }
                    }
                ));

            return ListaUsuarios;
        }

        public static bool Excluir(dynamic param)
        {
            try
            {
                long id = (long)param.id;

                LaudosPiscinasEntities context = new LaudosPiscinasEntities();
                usuario instancia = context.usuario.Where(u => u.usu_id_usuario == id).FirstOrDefault();
                context.usuario.Remove(instancia);
                context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                throw new Exception("Não foi possivel excluir o usuario.");
            }
        }

        public static bool Adicionar(dynamic param)
        {
            try
            {
                string email = param.email;
                string senha = param.senha;

                LaudosPiscinasEntities context = new LaudosPiscinasEntities();
                usuario instancia = new usuario();
                instancia.usu_ds_email = email;
                instancia.usu_ds_senha = senha;
                instancia.usu_id_grupo = 1;
                instancia.usu_st_status = 1;
                instancia.usu_st_alterar_senha = 1;

                context.usuario.Add(instancia);
                return context.SaveChanges() > 0;
            }
            catch (Exception)
            {
                throw new Exception("Não foi possivel adicionar o usuario.");
            }
        }


        public static LoginViewModel LoginUser(dynamic param)
        {
            string cpf = param.cpf;
            string senha = Seguranca.EncryptTripleDES(Convert.ToString(param.senha));


            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var Colaborador = context.colaborador.FirstOrDefault(u => u.col_nr_cpf == cpf && u.col_ds_senha == senha);

            if (Colaborador == null)
            {
                throw new Exception("Usuario ou senha não existem.");
            }

            LoginViewModel data = new LoginViewModel();

            data.Routes = ObterRotasDefault(Colaborador.col_id_colaborador);


            //mudar para colaborador viewmodel
            data.Usuario = new UsuarioViewModel
            {
                Email = Colaborador.col_ds_email,
                Id = (int)Colaborador.col_id_colaborador,
                Senha = Seguranca.DecryptTripleDES(Colaborador.col_ds_senha),
                NomeUsuario = Colaborador.col_nm_colaborador
            };

            return data;
        }


        private static List<RouteViewModel> ObterRotasDefault(int idGrupo)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var ListaRotasTemporaria = context.grupo_pagina_acao.Where(gpa => gpa.gpa_id_grupo == idGrupo).ToList();

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

            return ListaRotas;
        }


        public static LoginViewModel LoginObra(dynamic param)
        {
            //string cpf = param.cpf;
            //string senha = param.senha;
            //int idObra = param.obra.idObra;
            //int idUsuario = param.user.usuario.id;

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
                NomeUsuario = objObraColaborador.colaborador.col_nm_colaborador
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
                ObraFinalizada = objObraColaborador.obra.obr_dt_fim == null ? false : (objObraColaborador.obra.obr_dt_fim == DateTime.MinValue.Date ? false : true)

            };


            data.Menu = ObterMenu((int)objObraColaborador.oco_id_grupo);

            return data;


        }




        public static LoginViewModel Login(dynamic param)
        {
            string email = "adm";
            string senha = "123";

            //string email = param.cpf;
            //string senha = param.senha;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var Usuario = context.usuario.FirstOrDefault(u => u.usu_ds_email == email && u.usu_ds_senha == senha);

            if (Usuario == null)
            {
                throw new Exception("Usuario ou senha não existem.");
            }

            LoginViewModel data = new LoginViewModel();

            data.Routes = ObterRotas((int)Usuario.usu_id_grupo);
            data.Routes = ObterRotasDefault((int)Usuario.usu_id_grupo);


            data.Usuario = new UsuarioViewModel
            {
                Email = Usuario.usu_ds_email,
                Id = (int)Usuario.usu_id_usuario,
                Senha = Usuario.usu_ds_senha
            };


            //remover daqui, a obra será setada na tela de escolher obra quando ela for criada
            data.Obra = new ObraViewModel
            {
                IdObra = 1,
                Descricao = "Casa de Nivaldo",
                idDono = 1

            };


            data.Menu = ObterMenu((int)Usuario.usu_id_grupo);

            return data;


        }

        //private static string CriarToken()
        //{
        //    string Token = string.Concat(Guid.NewGuid().ToString(), DateTime.Now).Replace("-", "").Replace(" ", "").Replace("/", "").Replace(":", "");
        //    Token = Convert.ToBase64String(new ASCIIEncoding().GetBytes(Token));
        //    return Token;
        //}

        //private static bool TokenValido(string Token)
        //{
        //    Token = "MmJmZGJhMDliNjhmNDc5MmJlNWEyNjNiODQ5MGNkODIyNjA3MjAxNzEwNTY0NQ==";
        //    string Token_ = new ASCIIEncoding().GetString(Convert.FromBase64String(Token));
        //    return true;
        //}

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

            MenuCompleto.Where(mt => mt.mpa_id_pagina_pai == null).ToList().ForEach(mt =>
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
    }

    //public class UsuarioViewModel
    //{
    //    public UsuarioViewModel()
    //    {
    //        this.Grupo = new GrupoViewModel();
    //    }

    //    public long Id { get; set; }
    //    public string NomeUsuario { get; set; }
    //    public string Email { get; set; }
    //    public string Senha { get; set; }
    //    public long IdGrupo { get; set; }
    //    public Nullable<long> Status { get; set; }
    //    public Nullable<long> StatusAlterarSenha { get; set; }

    //    public virtual GrupoViewModel Grupo { get; set; }

    //}
}