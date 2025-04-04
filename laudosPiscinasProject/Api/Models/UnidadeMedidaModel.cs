using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace laudosPiscinasProject.Api.Models
{
    public class UnidadeMedidaModel
    {
        public static List<UnidadeMedidaViewModel> Lista()
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            IQueryable<unidade_de_medida> query = context.Set<unidade_de_medida>();

            List<UnidadeMedidaViewModel> Lista = new List<UnidadeMedidaViewModel>();
            query.ToList().ForEach(uni => Lista.Add(new UnidadeMedidaViewModel
            {
                Id = uni.unm_id_unidade,
                Nome = uni.unm_ds_unidade

            }));

            return Lista.OrderBy(x => x.Id).ToList();
        }

        public static UnidadeMedidaViewModel Get(int id)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            IQueryable<unidade_de_medida> query = context.Set<unidade_de_medida>();
            UnidadeMedidaViewModel unidade = new UnidadeMedidaViewModel();
            var consulta = query.Where(x => x.unm_id_unidade == id).FirstOrDefault();
            if (consulta != null)
            {
                unidade = new UnidadeMedidaViewModel
                {
                    Id = consulta.unm_id_unidade,
                    Nome = consulta.unm_ds_unidade
                };
            }
            return unidade;
        }

        public class UnidadeMedidaViewModel
        {
            public long Id { get; set; }
            public string Nome { get; set; }

        }


    }
}