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
    
    public partial class status_rdo
    {
        public status_rdo()
        {
            this.laudo = new HashSet<laudo>();
            this.rdo = new HashSet<rdo>();
        }
    
        public int str_id_status { get; set; }
        public string str_ds_status { get; set; }
    
        public virtual ICollection<laudo> laudo { get; set; }
        public virtual ICollection<rdo> rdo { get; set; }
    }
}
