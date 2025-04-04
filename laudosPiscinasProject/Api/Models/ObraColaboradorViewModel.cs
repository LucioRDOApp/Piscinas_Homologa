using System;

namespace laudosPiscinasProject.Api.Models
{
    public class ObraColaboradorViewModel
    {
        public int IdObraColaborador { get; set; }
        public int IdObra { get; set; }
        public int IdColaborador { get; set; }
        public int IdCargo { get; set; }
        public int TelefoneSecundario { get; set; }
        public long IdGrupo { get; set; }
        public DateTime DataContratacao { get; set; }
        public string ContratanteContratada { get; set; }
        public string NomeObra { get; set; }
        public string NomeColaborador { get; set; }
        public string TipoLicencaColaboradorGrupo { get; set; }
        public int IdLicenca { get; set; }
    }
}