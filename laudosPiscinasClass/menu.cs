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
    
    public partial class menu
    {
        public menu()
        {
            this.grupo = new HashSet<grupo>();
            this.menu_pagina = new HashSet<menu_pagina>();
        }
    
        public int men_id_menu { get; set; }
        public string men_nm_titulo { get; set; }
        public string men_ds_alias { get; set; }
        public int men_st_status { get; set; }
    
        public virtual ICollection<grupo> grupo { get; set; }
        public virtual ICollection<menu_pagina> menu_pagina { get; set; }
    }
}
