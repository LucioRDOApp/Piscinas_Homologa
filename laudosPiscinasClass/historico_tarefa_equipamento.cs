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
    
    public partial class historico_tarefa_equipamento
    {
        public int hte_id_tarefa_equipamento { get; set; }
        public int hte_id_historico_tarefa_rdo { get; set; }
        public int hte_id_obra_equipamento { get; set; }
    
        public virtual historico_tarefa_rdo historico_tarefa_rdo { get; set; }
        public virtual obra_equipamento obra_equipamento { get; set; }
    }
}
