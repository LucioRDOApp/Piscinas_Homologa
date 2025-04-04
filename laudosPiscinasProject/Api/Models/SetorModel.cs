using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;

namespace laudosPiscinasProject.Api.Models
{
    public class SetorModel
    {
        public static List<SetorViewModel> Lista()
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            IQueryable<setor> query = context.Set<setor>();

            List<SetorViewModel> Lista = new List<SetorViewModel>();
            //SetorViewModel s = new SetorViewModel();
            //s.Id = 0;
            //s.Nome = "Selecione um Setor";
            //Lista.Add(s);
            query.ToList().ForEach(set => Lista.Add(new SetorViewModel
            {
                Id = set.set_id_setor,
                Nome = set.set_ds_setor

            }));

            return Lista.OrderBy(x => x.Id).ToList();
        }

        public static bool Remove(dynamic param)
        {
            try
            {
                int? idSetor = param.idSetor;
                LaudosPiscinasEntities context = new LaudosPiscinasEntities();
                string codigoSetor = param.codigoSetor;
                context.setor.Remove(context.setor.FirstOrDefault(set => set.set_id_setor == idSetor || set.set_id_setor_loja == codigoSetor));
                return context.SaveChanges() > 0;
            }
            catch
            {
                throw new Exception("Não foi possível remover o Setor. Existem registros dependentes");
            }
        }

        public static bool Atualizar(dynamic param)
        {
            try
            {
                int? idSetor = param.idSetor;
                string codigoSetor = param.codigoSetor;
                string descricao = param.descricao;
                LaudosPiscinasEntities context = new LaudosPiscinasEntities();
                setor objSetor = context.setor.FirstOrDefault(set => set.set_id_setor == idSetor || set.set_id_setor_loja == codigoSetor) ?? new setor();

                objSetor.set_ds_setor = descricao;

                if (!string.IsNullOrEmpty(codigoSetor))
                    objSetor.set_id_setor_loja = codigoSetor;

                if (objSetor.set_id_setor > 0)
                {
                    context.setor.Attach(objSetor);
                    context.Entry(objSetor).State = EntityState.Modified;
                }
                else
                {
                    context.setor.Add(objSetor);
                }

                return context.SaveChanges() > 0;

            }
            catch
            {
                throw new Exception("Não foi possível atualizar o Setor, tente novamente");
            }
        }
        
    }

    public class SetorViewModel
    {
        public long Id { get; set; }
        public string Nome { get; set; }

    }
}