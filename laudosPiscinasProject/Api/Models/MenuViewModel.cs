using System.Collections.Generic;

namespace laudosPiscinasProject.Api.Models
{
    public class MenuViewModel
    {
        public MenuViewModel()
        {
            this.Grupo = new HashSet<GrupoViewModel>();
            this.MenuPagina = new HashSet<MenuPaginaViewModel>();
        }

        public long Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Alias { get; set; }
        public long Status { get; set; }

        public virtual ICollection<GrupoViewModel> Grupo { get; set; }
        public virtual ICollection<MenuPaginaViewModel> MenuPagina { get; set; }
        public virtual ICollection<PaginaViewModel> ListaPagina { get; set; }
        public virtual ICollection<PaginaViewModel> ListaPaginasRemovidas { get; set; }
    }
}