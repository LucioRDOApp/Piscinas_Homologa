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
    
    public partial class unidade_de_medida
    {
        public unidade_de_medida()
        {
            this.tarefa = new HashSet<tarefa>();
        }
    
        public int unm_id_unidade { get; set; }
        public string unm_ds_unidade { get; set; }
        public string unm_ds_simbolo { get; set; }
    
        public virtual ICollection<tarefa> tarefa { get; set; }
    }
}
