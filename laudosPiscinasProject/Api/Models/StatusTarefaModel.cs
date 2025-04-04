using LaudosPiscinasClass;
using System.Collections.Generic;
using System.Linq;

namespace laudosPiscinasProject.Api.Models
{
    public class StatusTarefaModel
    {
        public static List<StatusTarefaViewModel> Lista()
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            IQueryable<status_tarefa> query = context.Set<status_tarefa>();
            
            List<StatusTarefaViewModel> Lista = new List<StatusTarefaViewModel>();
            query.ToList().ForEach(str => Lista.Add(new StatusTarefaViewModel
            {
                Id = str.stt_id_status,
                Nome = str.stt_ds_status

            }));
            
            return Lista.OrderBy(x => x.Id).ToList();
        }
        public static List<StatusTarefaViewModel> ListaStatusTarefaPermitidos(dynamic param)
        {
            int statusAtual = param == null ? 0 : (int)param;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            List<status_tarefa> query = context.Set<status_tarefa>().ToList();

            if (statusAtual == 1)
            {
                query = query.Where(x => x.stt_id_status == 1 || x.stt_id_status == 2 || x.stt_id_status == 3 || x.stt_id_status == 4 || x.stt_id_status == 5).ToList();
            }
            else if (statusAtual == 2)
            {
                query = query.Where(x => x.stt_id_status == 2 || x.stt_id_status == 3 || x.stt_id_status == 4 || x.stt_id_status == 5).ToList();
            }
            else if (statusAtual == 3)
            {
                query = query.Where(x => x.stt_id_status == 3).ToList();
            }
            else if (statusAtual == 4)
            {
                query = query.Where(x => x.stt_id_status == 2 || x.stt_id_status == 3 || x.stt_id_status == 4 || x.stt_id_status == 5).ToList();
            }
            else if (statusAtual == 5)
            {
                query = query.Where(x => x.stt_id_status == 5).ToList();
            }
            else
            {
                query = query.Where(x => x.stt_id_status == 1 || x.stt_id_status == 2).ToList();
            }

            List<StatusTarefaViewModel> Lista = new List<StatusTarefaViewModel>();
            query.ToList().ForEach(str => Lista.Add(new StatusTarefaViewModel
            {
                Id = str.stt_id_status,
                Nome = str.stt_ds_status,
                CssClass = str.stt_id_status == 1 ? "bg-cinza" : (str.stt_id_status == 2 ? "bg-azul" : (str.stt_id_status == 3 ? "bg-verde" : (str.stt_id_status == 4 ? "bg-laranja" : (str.stt_id_status == 5 ? "bg-vermelho" : "bg-cinza"))))

            }));

            return Lista.OrderBy(x => x.Id).ToList();
        }
    }
    public class StatusTarefaViewModel
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string CssClass { get; set; }
    }
}