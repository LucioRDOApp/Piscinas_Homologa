using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;


namespace laudosPiscinasProject.Api.Controllers
{
    public class LoginController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage LoginUser([FromBody] dynamic param)
        {
            try
            {
                LoginViewModel model = LoginModel.LoginUser(param);
                return Request.CreateResponse(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        [HttpPost]
        public HttpResponseMessage LoginObra([FromBody] dynamic param)
        {
            try
            {
                LoginViewModel retorno = LoginModel.LoginObra(param);
                return Request.CreateResponse(HttpStatusCode.OK, retorno);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        public LoginViewModel AcessoConvidada([FromBody] dynamic param)
        {
            return LoginModel.AcessoConvidada(param);
        }

        public bool ServicoLoja([FromBody] dynamic param)
        {
            return LoginModel.ServicoLoja(param);
        }

        public bool VerificarLicenca([FromBody] dynamic param)
        {
            return LoginModel.VerificarLicenca(param);
        }

        public HttpResponseMessage RecuperarSenha(dynamic param)
        {
            try
            {
                LoginModel.Enviar();
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        [HttpPost]
        public PagedList GetHistoricoLogin(dynamic param)
        {
            return PagedList.create((int)param.page, 50, LoginModel.GetHistoricoLogin(param));
        }
    }
}