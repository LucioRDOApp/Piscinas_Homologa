//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LaudosPiscinasClass
{
    using System;
    using System.Collections.Generic;
    
    public partial class imagem
    {
        public imagem()
        {
            this.rdo_imagem = new HashSet<rdo_imagem>();
        }
    
        public int ima_id_imagem { get; set; }
        public string ima_ds_caminho { get; set; }
        public Nullable<int> ima_id_historico_tarefa_rdo { get; set; }
        public int ima_id_tarefa { get; set; }
        public System.DateTime ima_dt_imagem { get; set; }
    
        public virtual tarefa tarefa { get; set; }
        public virtual ICollection<rdo_imagem> rdo_imagem { get; set; }
    }
}
