using LaudosPiscinasClass;
using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AngularWebApi.Api.Controllers
{
    public class MenuController : ApiController
    {
        [HttpPost]
        public List<menu> ObterMenu([FromBody] dynamic param)
        {
            //string titulo = param.titulo;
            string titulo = "";

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            context.Configuration.LazyLoadingEnabled = false;

            List<menu> listMenus = context.menu.Where(u => u.men_nm_titulo.ToLower().Contains(titulo)).ToList();

            return context.menu.Where(u => u.men_nm_titulo.ToLower().Contains(titulo)).ToList();
        }

        [HttpPost]
        public List<MenuViewModel> ObterMenus([FromBody] dynamic param)
        {
            return MenuModel.ObterMenus(param);
        }

        public List<pagina> ObterPaginas([FromBody] dynamic param)
        {
            List<pagina> returnList = new List<pagina>();
            
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            context.Configuration.LazyLoadingEnabled = false;
            returnList = context.pagina.ToList();

            return returnList;
        }

        [HttpPost]
        public List<pagina> ObterPaginasMenu([FromBody] dynamic param)
        {
            List<pagina> returnList = new List<pagina>();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            context.Configuration.LazyLoadingEnabled = false;
            returnList = context.pagina.ToList();

            return returnList;
        }

        [HttpPost]
        public MenuViewModel ObterDetalheMenu([FromBody] dynamic param)
        {
            return MenuModel.ObterMenu(param);
        }

        [HttpPost]
        public bool Excluir([FromBody] dynamic param)
        {
            return MenuModel.Excluir(param);
        }

        [HttpPost]
        public bool Salvar([FromBody] dynamic param)
        {
            return MenuModel.Salvar(param);
        }
    }
}
