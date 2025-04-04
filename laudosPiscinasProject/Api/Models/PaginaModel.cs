using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace laudosPiscinasProject.Api.Models
{
    public class PaginaModel
    {
        public static List<PaginaViewModel> Lista(dynamic param)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            IQueryable<pagina> query = context.Set<pagina>();

            if (param != null)
            {
                int idStatus = param.status ?? 0;
                if (idStatus > 0)
                {
                    if (idStatus >= 0)
                    {
                        query = query.Where(pag => pag.pag_st_status == idStatus);
                    }
                }
                string titulo = param.titulo.ToString();
                if (!string.IsNullOrEmpty(titulo))
                {
                    query = query.Where(pag => pag.pag_nm_titulo == titulo);
                }
            }

            List<PaginaViewModel> Lista = new List<PaginaViewModel>();
            query.ToList().ForEach(pag => Lista.Add(new PaginaViewModel
            {
                Caminho = pag.pag_ds_url,
                Id = (int)pag.pag_id_pagina,
                Titulo = pag.pag_nm_titulo,
                Alias = pag.pag_ds_alias,
                Status = (int)pag.pag_st_status
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

            return Lista;
        }

        internal static bool Deletar(dynamic param)
        {
            int idPagina = (int)param.id;

            PaginaViewModel pagina = new PaginaViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            try
            {
                context.pagina_acao.Where(x => x.paa_id_pagina == idPagina).ToList().ForEach(y => context.pagina_acao.Remove(y));
                context.pagina.Where(x => x.pag_id_pagina == idPagina).ToList().ForEach(y => context.pagina.Remove(y));


                return context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return false;
            }
        }

        internal static PaginaViewModel ObterRegistro(dynamic param)
        {
            int idPagina = (int)param;

            PaginaViewModel pagina = new PaginaViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var resultado = context.pagina.FirstOrDefault(pag => pag.pag_id_pagina == idPagina);

            pagina.Id = (int)resultado.pag_id_pagina;
            pagina.Alias = resultado.pag_ds_alias;
            pagina.Caminho = resultado.pag_ds_url;
            pagina.Status = (int)resultado.pag_st_status;
            pagina.Titulo = resultado.pag_nm_titulo;
            pagina.ListaAcao = new List<PaginaAcaoViewModel>();

            context.acao.ToList().ForEach(x =>
                pagina.ListaAcao.Add(
                new PaginaAcaoViewModel
                {
                    Acao = x.aca_ds_acao,
                    Id = (int)x.aca_id_acao,
                    Marcado = resultado.pagina_acao.ToList().FirstOrDefault(y => y.paa_id_acao == x.aca_id_acao) != null ? true : false
                })
            );

            return pagina;
        }

        public static bool Salvar(dynamic param)
        {
            if (string.IsNullOrEmpty(param.titulo.ToString()))
            {
                throw new Exception("O titulo deve ser preenchido");
            }

            int idPagina = param.id ?? 0;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            pagina _pagina = context.pagina.Where(x => x.pag_id_pagina == (long)idPagina).FirstOrDefault() ?? new pagina();

            _pagina.pag_ds_alias = param.alias;
            _pagina.pag_ds_url = param.caminho;
            _pagina.pag_nm_titulo = param.titulo;
            _pagina.pag_st_status = param.status;

            if (_pagina.pag_id_pagina > 0)
            {
                context.pagina.Attach(_pagina);
                context.Entry(_pagina).State = EntityState.Modified;
            }
            else
            {
                context.pagina.Add(_pagina);
            }

            bool result = context.SaveChanges() > 0;

            //removi essa forma de fazer pois você não pode remover todos os pagina_ação do banco depois inserir de novo, porque eles podem já estar sendo usados no grupo_pagina_ação
            //context.pagina_acao.Where(x => x.paa_id_pagina == idPagina).ToList().ForEach(y => context.pagina_acao.Remove(y)); 

            List<pagina_acao> lpa = new List<pagina_acao>();

            foreach (var item in param.listaAcao)
            {
                if ((bool)item.marcado)
                {
                    int idAcao = item.id;
                    bool jaExisteNoBanco = context.pagina_acao.Any(x => x.paa_id_pagina == idPagina && x.paa_id_acao == idAcao);

                    if (!jaExisteNoBanco)
                    {
                        context.pagina_acao.Add(new pagina_acao
                        {
                            pagina = _pagina,
                            paa_id_acao = item.id
                        });
                    }
                }
                else
                {
                    int idAcao = item.id;
                    context.pagina_acao.Where(x => x.paa_id_pagina == idPagina && x.paa_id_acao == idAcao).ToList().ForEach(y => {
                        y.grupo_pagina_acao.ToList().ForEach(gpa => {
                            context.grupo_pagina_acao.Remove(gpa);
                        });
                        context.pagina_acao.Remove(y);
                    });
                }
            }

            try
            {
                result = context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Você provavelmente está tentando remover uma pagina_acao que já está sendo usada por um grupo_pagina_acao " + ex.Message);
            }

            return result;
        }

        public static object CarregaListaAcao()
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var resultado = new List<PaginaAcaoViewModel>();

            context.acao.ToList().ForEach(x =>
                resultado.Add(new PaginaAcaoViewModel
                {
                    Id = (int)x.aca_id_acao,
                    Acao = x.aca_ds_acao,
                    Marcado = false
                }
            ));

            return resultado;
        }
    }

    public class PaginaViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Caminho { get; set; }
        public string Alias { get; set; }
        public int Status { get; set; }
        public string CssClass { get; set; }
        public List<PaginaViewModel> Paginas { get; set; }
        public List<PaginaAcaoViewModel> ListaAcao { get; set; }
    }
}