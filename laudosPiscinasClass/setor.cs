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
    
    public partial class setor
    {
        public setor()
        {
            this.empresa = new HashSet<empresa>();
        }
    
        public int set_id_setor { get; set; }
        public string set_ds_setor { get; set; }
        public string set_id_setor_loja { get; set; }
    
        public virtual ICollection<empresa> empresa { get; set; }
    }
}
