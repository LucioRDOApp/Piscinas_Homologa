using System.Collections.Generic;

namespace laudosPiscinasProject.Api.Models
{
    public class LoginViewModel
    {
        public string Token { get; set; }
        public UsuarioViewModel Usuario { get; set; }
        public List<RouteViewModel> Routes { get; set; }
        public MenuViewModel Menu { get; set; }
        public ObraViewModel Obra { get; set; }
        public ObraColaboradorViewModel ObraColaborador { get; set; }
        public LoginViewModel()
        {
            Routes = new List<RouteViewModel>();
        }
    }
}