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
    
    public partial class obra_equipamento
    {
        public obra_equipamento()
        {
            this.historico_tarefa_equipamento = new HashSet<historico_tarefa_equipamento>();
            this.obra_tarefa_equipamento = new HashSet<obra_tarefa_equipamento>();
        }
    
        public int oeq_id_obra_equipamento { get; set; }
        public int oeq_id_obra { get; set; }
        public int oeq_id_equipamento { get; set; }
        public string oeq_tp_aquisicao { get; set; }
        public string oeq_ds_fabricante_fornecedor { get; set; }
        public Nullable<System.DateTime> oeq_dt_aquisicao { get; set; }
        public string oeq_ds_contato { get; set; }
        public string oeq_ds_telefone { get; set; }
    
        public virtual equipamento equipamento { get; set; }
        public virtual ICollection<historico_tarefa_equipamento> historico_tarefa_equipamento { get; set; }
        public virtual obra obra { get; set; }
        public virtual ICollection<obra_tarefa_equipamento> obra_tarefa_equipamento { get; set; }
    }
}
