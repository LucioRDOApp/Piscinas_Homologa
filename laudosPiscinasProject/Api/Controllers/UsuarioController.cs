using LaudosPiscinasClass;
using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace laudosPiscinasProject.Api.Controllers
{
    public class UsuarioController : ApiController
    {
        [HttpGet]
        public string IsApiOnline()
        {
            return "Api is online " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
        }

        [HttpPost]
        public List<usuario> ObterUsuarios([FromBody] dynamic param)
        {
            return UsuarioModel.ObterUsuarios(param);
        }

        [HttpPost]
        public bool Excluir([FromBody] dynamic param)
        {
            return UsuarioModel.Excluir(param);
        }


        [HttpPost]
        public bool Adicionar([FromBody] dynamic param)
        {
            return UsuarioModel.Adicionar(param);
        }

        [HttpPost]
        public LoginViewModel Login([FromBody] dynamic param)
        {
            return UsuarioModel.Login(param);
        }

        [HttpPost]
        public LoginViewModel LoginUser([FromBody] dynamic param)
        {
            return UsuarioModel.LoginUser(param);

        }
        [HttpPost]
        public LoginViewModel LoginObra([FromBody] dynamic param)
        {
            return UsuarioModel.LoginObra(param);
        }


    }
}
