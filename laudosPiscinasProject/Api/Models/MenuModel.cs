using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace laudosPiscinasProject.Api.Models
{
    public class MenuModel
    {
        public int Id { get; set; }
        public string Menu { get; set; }
        public List<PaginaViewModel> ListaPagina { get; set; }

        public static MenuViewModel ObterMenu(dynamic param)
        {
            //string titulo = param.titulo;
            int idMenu = (int)param.idMenu;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            List<MenuViewModel> listaMenu = new List<MenuViewModel>();

            MenuViewModel objMenu = new MenuViewModel();
            objMenu.ListaPagina = new List<PaginaViewModel>();
            objMenu.ListaPaginasRemovidas = new List<PaginaViewModel>();
            menu _menu = context.menu.FirstOrDefault(x => x.men_id_menu == idMenu);

            objMenu.Titulo = _menu.men_nm_titulo;
            objMenu.Id = _menu.men_id_menu;
            objMenu.Alias = _menu.men_ds_alias;
            _menu.menu_pagina.OrderBy(x => x.mpa_vl_ordem).ToList().ForEach(x => objMenu.ListaPagina.Add(new PaginaViewModel
            {
                Titulo = x.pagina.pag_nm_titulo,
                Id = (int)x.pagina.pag_id_pagina,
                Alias = x.pagina.pag_ds_alias,
                Caminho = x.pagina.pag_ds_url

            }));


            return objMenu;


            //context.menu.Where(u => u.men_nm_titulo.ToLower().Contains(titulo)).ToList().ForEach(
            //    menu => listaMenu.Add(new MenuViewModel()
            //    {
            //        Id = menu.men_id_menu,
            //        Titulo = menu.men_nm_titulo,
            //        Descricao = menu.men_ds_alias,
            //        Status = menu.men_st_status
            //    }));


        }

        public static bool Salvar(dynamic param)
        {

            int idMenu = param.id ?? 0;
            string titulo = param.titulo;
            string alias = param.alias;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            menu _menu = context.menu.FirstOrDefault(x => x.men_id_menu == idMenu) ?? new menu();

            _menu.men_nm_titulo = titulo;
            _menu.men_ds_alias = alias;
            _menu.men_st_status = 1;

            if (idMenu <= 0)
            {
                context.menu.Add(_menu);
            }
            else
            {
                context.menu.Attach(_menu);
                context.Entry(_menu).State = EntityState.Modified;
            }

            bool result = context.SaveChanges() > 0;

            result = SalvarListaPaginasMenu(param, context, (int)_menu.men_id_menu);


            return result;
        }

        internal static bool Excluir(dynamic param)
        {
            try
            {
                int id = Convert.ToInt32(param.id);

                LaudosPiscinasEntities context = new LaudosPiscinasEntities();
                menu m = context.menu.Find(id);
                context.menu.Remove(m);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                throw new Exception("A página não pode ser excluída. Existem registros dependentes.");
            }
        }

        internal static bool SalvarListaPaginasMenu(dynamic param, LaudosPiscinasEntities context, int idMenu)
        {
            bool result = true;
            List<menu_pagina> paginasAdicionadas = new List<menu_pagina>();

            context.menu_pagina.Where(mpa => mpa.menu.men_id_menu == idMenu).ToList().ForEach(m =>
            {
                context.menu_pagina.Remove(m);
            });
            context.SaveChanges();

            foreach (var item in param.listaPagina)
            {
                int idPagina = (int)item.id;
                menu_pagina mpa = context.menu_pagina.FirstOrDefault(x => x.mpa_id_pagina == idPagina && x.mpa_id_menu == idMenu) ?? new menu_pagina();
                mpa.mpa_id_menu = idMenu;
                mpa.mpa_id_pagina = idPagina;


                if (mpa.mpa_id_menu_pagina > 0)
                {
                    context.menu_pagina.Attach(mpa);
                    context.Entry(mpa).State = EntityState.Modified;
                }
                else
                {
                    context.menu_pagina.Add(mpa);
                }
                paginasAdicionadas.Add(mpa);
            }

            result = context.SaveChanges() > 0;

            return result;
        }

        public static menu ObterRegistro(long idMenu)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            menu objMenu = new menu();

            objMenu = context.menu.Where(x => x.men_id_menu == idMenu).FirstOrDefault();

            return objMenu;
        }

        public static List<MenuViewModel> ObterMenus(dynamic param)
        {
            //string titulo = param.titulo;
            string titulo = "";

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            List<MenuViewModel> listaMenu = new List<MenuViewModel>();

            context.menu.Where(u => u.men_nm_titulo.ToLower().Contains(titulo)).ToList().ForEach(
                menu => listaMenu.Add(new MenuViewModel()
                {
                    Id = menu.men_id_menu,
                    Titulo = menu.men_nm_titulo,
                    Descricao = menu.men_ds_alias,
                    Status = menu.men_st_status
                }));

            return listaMenu;
        }

        public static bool Adicionar(dynamic param)
        {
            try
            {
                string titulo = param.titulo;


                LaudosPiscinasEntities context = new LaudosPiscinasEntities();
                menu instancia = new menu();
                instancia.men_nm_titulo = titulo;
                instancia.men_ds_alias = titulo;
                instancia.men_st_status = 1;


                context.menu.Add(instancia);
                return context.SaveChanges() > 0;
            }
            catch (Exception)
            {
                throw new Exception("Não foi possivel adicionar seu usuario.");
            }
        }
    }
}
