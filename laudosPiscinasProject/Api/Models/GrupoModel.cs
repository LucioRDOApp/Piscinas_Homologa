using System.Collections.Generic;
using System.Linq;
using System;
using System.Data;
using LaudosPiscinasClass;

namespace laudosPiscinasProject.Api.Models
{
    public class GrupoModel
    {
        public int Id { get; set; }
        public List<PaginaViewModel> paginas { get; set; }

        internal static GrupoViewModel ObterRegistro(dynamic param)
        {
            int idGrupo = (int)param;

            GrupoViewModel grupo = new GrupoViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var resultado = context.grupo.FirstOrDefault(gru => gru.gru_id_grupo == idGrupo);

            grupo.Id = (int)resultado.gru_id_grupo;
            grupo.Nome = resultado.gru_nm_nome;
            grupo.IdMenu = resultado.gru_id_menu;
            grupo.StatusDiretor = resultado.gru_st_diretor ?? 0;
            grupo.IdLicenca = resultado.gru_id_licenca ?? 0;
            grupo.StatusContratante = resultado.gru_st_contratante ?? -1;

            return grupo;
        }

        public static object CarregaListaGrupoPaginaAcao(dynamic param)
        {
            int idGrupo = param == null ? 0 : (int)param;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var finshResult = new List<GrupoPaginaAcaoViewModel>();
            List<acao> acoes = context.acao.ToList();

            context.pagina.ToList().ForEach(x =>
                finshResult.Add(new GrupoPaginaAcaoViewModel
                {
                    objPagina = PaginaModel.ObterRegistro(x.pag_id_pagina),
                    IdPagina = x.pag_id_pagina,
                    //ListaAcao = PaginaModel.ObterRegistro(x.pag_id_pagina).ListaAcao
                    //ListaAcao = new List<PaginaAcaoViewModel>()
                }
            ));

            foreach (GrupoPaginaAcaoViewModel gpa in finshResult)
            {
                gpa.objPagina.ListaAcao = new List<PaginaAcaoViewModel>();

                context.acao.Where(ac => ac.pagina_acao.Any(pa => pa.paa_id_pagina == gpa.IdPagina)).ToList().ForEach(x =>
                    gpa.objPagina.ListaAcao.Add(new PaginaAcaoViewModel
                    {
                        Id = (int)x.aca_id_acao,
                        Acao = x.aca_ds_acao,
                        Marcado = idGrupo == 0 ? false : ObterGrupoPaginaAcao(idGrupo, ObterPaginaAcao(gpa.IdPagina, x.aca_id_acao).paa_id_pagina_acao).gpa_id_grupo_pagina_acao > 0
                    }
                ));
            }

            return finshResult;
        }

        public static List<GrupoViewModel> Lista(dynamic param)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();



            IQueryable<grupo> query = context.Set<grupo>();

            if (param.nome != null)
            {
                string nome = param.nome.ToString();
                if (!string.IsNullOrEmpty(nome))
                {
                    query = query.Where(gru => gru.gru_nm_nome.ToLower().Contains(nome));
                }
            }

            if (param.idGrupo != null && (int)param.idGrupo > 0)
            {
                int idGrupo = (int)param.idGrupo;
                string nomeGrupo = query.FirstOrDefault(grp => grp.gru_id_grupo == idGrupo).gru_nm_nome;
                if (nomeGrupo.ToLower().Contains("contratante"))
                {
                    query = query.Where(gru => gru.gru_nm_nome.ToLower().Contains("contratante"));
                }
                else if (nomeGrupo.ToLower().Contains("contratada"))
                {
                    query = query.Where(gru => gru.gru_nm_nome.ToLower().Contains("contratante"));
                }
                else if (nomeGrupo.ToLower().Contains("admin"))
                {
                    query = context.Set<grupo>();
                }
                else
                {
                    query = query.Where(gru => gru.gru_nm_nome.ToLower().Contains("default"));
                }


                if (nomeGrupo.ToLower().Contains("basica"))
                {
                    query = query.Where(gru => gru.gru_nm_nome.ToLower().Contains("basica"));
                }
                else if (nomeGrupo.ToLower().Contains("gratuita"))
                {
                    query = query.Where(gru => gru.gru_nm_nome.ToLower().Contains("gratuita"));
                }
                else if (nomeGrupo.ToLower().Contains("admin"))
                {
                    query = context.Set<grupo>();
                }
                else
                {
                    query = query.Where(gru => gru.gru_nm_nome.ToLower().Contains("default"));
                }
            }



            List<GrupoViewModel> Lista = new List<GrupoViewModel>();
            query.ToList().ForEach(gru => Lista.Add(new GrupoViewModel
            {
                Nome = gru.gru_nm_nome,
                Id = (int)gru.gru_id_grupo,
                IdMenu = gru.gru_id_menu,
                NomeMenu = MenuModel.ObterRegistro(gru.gru_id_menu).men_nm_titulo
            }));

            context.grupo.Where(x => x.gru_nm_nome.ToLower().Contains("Colaborador")).ToList().ForEach(gru => Lista.Add(new GrupoViewModel
            {
                Nome = gru.gru_nm_nome,
                Id = (int)gru.gru_id_grupo,
                IdMenu = gru.gru_id_menu,
                NomeMenu = MenuModel.ObterRegistro(gru.gru_id_menu).men_nm_titulo
            }));

            return Lista;
        }

        public static List<GrupoViewModel> ListaPeloGrupo(dynamic param)
        {

            string contratanteContratada = param.contratanteContratada.ToString();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            int idemp = Convert.ToInt32(param.idEmpresa);
            empresa emp = context.empresa.Find(idemp);

            IQueryable<grupo> query = context.Set<grupo>();

            if (param != null)
            {
                if (param.idEmpresa != null)
                {
                    query = query.Where(gru => gru.gru_id_licenca == emp.emp_id_licenca || gru.gru_id_licenca == null);
                }

                if (param.idLicenca != null)
                {
                    int idLicenca = Convert.ToInt32(param.idLicenca);
                    query = query.Where(gru => gru.gru_id_licenca == idLicenca || gru.gru_id_licenca == null);
                }


                if (!string.IsNullOrEmpty(contratanteContratada))
                {
                    if (contratanteContratada == "t")
                    {
                        query = query.Where(gru => gru.gru_st_contratante == 1 || gru.gru_st_contratante == null);
                    }
                    else if (contratanteContratada == "d")
                    {
                        query = query.Where(gru => gru.gru_st_contratante == 2 || gru.gru_st_contratante == null);
                    }
                    else
                    {
                        throw new Exception("Para o carregamento dos grupos é necessário saber se são grupos para uma contratante ou para uma contratada.");
                    }
                }
            }

            List<GrupoViewModel> Lista = new List<GrupoViewModel>();
            query.ToList().ForEach(gru => Lista.Add(new GrupoViewModel
            {
                Nome = gru.gru_nm_nome,
                Id = (int)gru.gru_id_grupo,
                IdMenu = gru.gru_id_menu,
                NomeMenu = MenuModel.ObterRegistro(gru.gru_id_menu).men_nm_titulo
            }));

            return Lista.OrderByDescending(x => x.Id).ToList();
        }



        internal static bool Deletar(dynamic param)
        {
            int idGrupo = (int)param.id;

            GrupoViewModel grupo = new GrupoViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            try
            {
                context.grupo_pagina_acao.Where(x => x.gpa_id_grupo == idGrupo).ToList().ForEach(y => context.grupo_pagina_acao.Remove(y));
                context.grupo.Where(x => x.gru_id_grupo == idGrupo).ToList().ForEach(y => context.grupo.Remove(y));


                return context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return false;
            }
        }

        public static bool Salvar(dynamic param)
        {
            if (string.IsNullOrEmpty(param.nome.ToString()))
            {
                throw new Exception("O titulo deve ser preenchido");
            }
            if (string.IsNullOrEmpty(param.idMenu.ToString()))
            {
                throw new Exception("O menu deve ser preenchido");
            }
            if (string.IsNullOrEmpty(param.idLicenca.ToString()))
            {
                throw new Exception("A licenca deve ser preenchida");
            }
            if (string.IsNullOrEmpty(param.statusDiretor.ToString()))
            {
                throw new Exception("O status diretor deve ser preenchido");
            }
            if (string.IsNullOrEmpty(param.statusContratante.ToString()))
            {
                throw new Exception("O status contratante deve ser preenchido");
            }

            int idGrupo = param.id ?? 0;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            grupo _grupo = context.grupo.Where(x => x.gru_id_grupo == idGrupo).FirstOrDefault() ?? new grupo();

            _grupo.gru_nm_nome = param.nome;
            _grupo.gru_id_menu = param.idMenu;
            _grupo.gru_id_licenca = Convert.ToInt32(param.idLicenca) > 0 ? param.idLicenca : null;
            _grupo.gru_st_diretor = param.statusDiretor > 0 ? param.statusDiretor : null;

            if (_grupo.gru_id_grupo > 0)
            {
                context.grupo.Attach(_grupo);
                context.Entry(_grupo).State = EntityState.Modified;
            }
            else
            {
                context.grupo.Add(_grupo);
            }

            context.SaveChanges();
            idGrupo = _grupo.gru_id_grupo;

            context.grupo_pagina_acao.Where(x => x.gpa_id_grupo == idGrupo).ToList().ForEach(y => context.grupo_pagina_acao.Remove(y));

            List<grupo_pagina_acao> lpa = new List<grupo_pagina_acao>();

            foreach (var item in param.listaGrupoPaginaAcao)
            {

                int idPagina = item.objPagina.id;
                int idAcao = 0;
                var listaAcoes = item.objPagina.listaAcao;

                foreach (dynamic objAcao in listaAcoes)
                {
                    if (objAcao.marcado == "true")
                    {
                        idAcao = objAcao.id;
                        if (!context.pagina_acao.Any(x => x.paa_id_pagina == idPagina && x.paa_id_acao == idAcao))
                        {
                            context.pagina_acao.Add(new pagina_acao() { paa_id_pagina = idPagina, paa_id_acao = idAcao });
                            context.SaveChanges();
                        }

                        pagina_acao paa = new pagina_acao();
                        paa = ObterPaginaAcao(idPagina, idAcao);
                        lpa.Add(new grupo_pagina_acao() { gpa_id_grupo = idGrupo, gpa_id_pagina_acao = paa.paa_id_pagina_acao });
                    }
                }

            }

            lpa.ForEach(x => context.grupo_pagina_acao.Add(x));
            return context.SaveChanges() > 0;
        }


        public static pagina_acao ObterPaginaAcao(long idPagina, long idAcao)
        {
            // mudar método para a classe PaginaAcao
            pagina_acao objPaginaAcao = new pagina_acao();
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            objPaginaAcao = context.pagina_acao.Where(x => x.paa_id_pagina == idPagina && x.paa_id_acao == idAcao).FirstOrDefault() ?? new pagina_acao();
            return objPaginaAcao;
        }


        public static grupo_pagina_acao ObterGrupoPaginaAcao(long idGrupo, long idPaginaAcao)
        {
            // mudar método para a classe PaginaAcao
            grupo_pagina_acao objGrupoPaginaAcao = new grupo_pagina_acao();
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            objGrupoPaginaAcao = context.grupo_pagina_acao.Where(x => x.gpa_id_grupo == idGrupo && x.gpa_id_pagina_acao == idPaginaAcao).FirstOrDefault() ?? new grupo_pagina_acao();
            return objGrupoPaginaAcao;
        }

        public static List<GrupoViewModel> ObterGrupos(dynamic param)
        {
            //pode morrrrrrer, excluir esse método
            string nome = param.nome;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            //context.Configuration.LazyLoadingEnabled = false;
            List<GrupoViewModel> ListaGrupo = new List<GrupoViewModel>();
            context.grupo.Where(u => u.gru_nm_nome.ToLower().Contains(nome)).ToList().ForEach(
                grupo => ListaGrupo.Add(new GrupoViewModel()
                {


                }));
            return ListaGrupo;
        }

        public static GrupoModel CarregarTodasAsPaginas(dynamic param)
        {
            //excluir método
            int id = param.id;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            GrupoModel grupo = new GrupoModel();
            grupo.Id = param.id;
            grupo.paginas = new List<PaginaViewModel>();

            grupo g = context.grupo.Find(id);

            List<pagina> paginas = context.pagina.Where(pag => pag.pag_st_status == 1 && pag.pagina_acao.Count > 0).ToList();
            paginas.ForEach(p =>
            {
                PaginaViewModel pagina = new PaginaViewModel();
                pagina.Id = (int)p.pag_id_pagina;
                pagina.Titulo = p.pag_nm_titulo;
                pagina.ListaAcao = new List<PaginaAcaoViewModel>();
                p.pagina_acao.ToList().ForEach(paa =>
                {
                    PaginaAcaoViewModel paginaAcao = new PaginaAcaoViewModel();
                    paginaAcao.Id = (int)paa.paa_id_pagina_acao;
                    paginaAcao.Acao = paa.acao.aca_ds_acao;
                    paginaAcao.Marcado = AcaoEstaDisponivelGrupoPagina(g, (int)paa.paa_id_acao, (int)p.pag_id_pagina);
                    pagina.ListaAcao.Add(paginaAcao);
                });

                grupo.paginas.Add(pagina);
            });

            return grupo;
        }

        private static bool AcaoEstaDisponivelGrupoPagina(grupo grupo, int idAcao, int idPag)
        {
            try
            {
                if (grupo == null)
                {
                    return false;
                }

                List<grupo_pagina_acao> ListGrupoPaginaAcao = grupo.grupo_pagina_acao.ToList();
                if (ListGrupoPaginaAcao.Where(gpa => gpa.pagina_acao.pagina.pag_id_pagina == idPag && gpa.pagina_acao.acao.aca_id_acao == idAcao && gpa.gpa_id_grupo == grupo.gru_id_grupo).Count() > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

    }

    public class GrupoViewModel
    {
        public GrupoViewModel()
        {
            this.GrupoPaginaAcao = new HashSet<GrupoPaginaAcaoViewModel>();
            this.Usuario = new HashSet<UsuarioViewModel>();
            this.ObraColaborador = new HashSet<ObraColaboradorViewModel>();
        }

        public long Id { get; set; }
        public string Nome { get; set; }
        public long IdMenu { get; set; }
        public string NomeMenu { get; set; }

        public int StatusDiretor { get; set; }
        public int StatusContratante { get; set; }
        public int IdLicenca { get; set; }

        public virtual MenuViewModel Menu { get; set; }
        public virtual ICollection<GrupoPaginaAcaoViewModel> GrupoPaginaAcao { get; set; }
        public virtual ICollection<UsuarioViewModel> Usuario { get; set; }
        public virtual ICollection<ObraColaboradorViewModel> ObraColaborador { get; set; }
    }

    public class GrupoPaginaAcaoViewModel
    {
        public long IdPagina { get; set; }
        public List<PaginaAcaoViewModel> ListaAcao { get; set; }
        public PaginaViewModel objPagina { get; set; }

    }
}