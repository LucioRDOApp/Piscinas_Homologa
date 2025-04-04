using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace laudosPiscinasProject.Api.Models
{
    public class StatusRdoModel
    {
        public static List<StatusRdoViewModel> Lista()
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            IQueryable<status_rdo> query = context.Set<status_rdo>();

            List<StatusRdoViewModel> Lista = new List<StatusRdoViewModel>();
            query.ToList().ForEach(str => Lista.Add(new StatusRdoViewModel
            {
                Id = str.str_id_status,
                Nome = str.str_ds_status

            }));

            //StatusRdoViewModel stv = new StatusRdoViewModel();
            //stv.Id = 0;
            //stv.Nome = "Selecione...";
            //Lista.Add(stv);

            return Lista.OrderBy(x => x.Id).ToList();
        }

        public class StatusRdoViewModel
        {
            public long Id { get; set; }
            public string Nome { get; set; }

        }

    }
}