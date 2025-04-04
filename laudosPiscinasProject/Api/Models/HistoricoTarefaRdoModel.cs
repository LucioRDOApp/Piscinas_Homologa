using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace laudosPiscinasProject.Api.Models
{
    public class HistoricoTarefaRdoModel
    {
        public static bool Salvar(rdo_tarefa htr)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            //historico_tarefa_rdo _htr = context.historico_tarefa_rdo.Where(x => x.his_id_historico_tarefa_rdo == htr.his_id_historico_tarefa_rdo).FirstOrDefault() ?? new historico_tarefa_rdo();

            //_htr.his_id_historico_tarefa_rdo = htr.his_id_historico_tarefa_rdo;
            //_htr.his_dt_data = htr.his_dt_data;
            //_htr.his_id_rdo = htr.his_id_rdo;
            //_htr.his_id_status = htr.his_id_status;
            //_htr.his_id_tarefa = htr.his_id_tarefa;
            //_htr.his_ds_foto = htr.his_ds_foto;
            //_htr.his_ds_comentario = htr.his_ds_comentario;



            //if (_htr.his_id_historico_tarefa_rdo > 0)
            //{
            //    context.historico_tarefa_rdo.Attach(_htr);
            //    context.Entry(_htr).State = EntityState.Modified;
            //}
            //else
            //{
            //    context.historico_tarefa_rdo.Add(_htr);
            //}

            bool result = context.SaveChanges() > 0;

            return result;
        }

        public static rdo_tarefa TarefaAtual(int idTarefa)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            rdo_tarefa h = null;// context.rdo_tarefa.Where(x => x.his_id_tarefa == idTarefa).OrderByDescending(x => x.his_dt_data).FirstOrDefault();

            return h;
        }

        public static rdo_tarefa ObterHistorico(int idTarefa, DateTime dataHistorico)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            rdo_tarefa h = null;// context.historico_tarefa_rdo.Where(x => x.his_dt_data == dataHistorico && x.his_id_tarefa == idTarefa).OrderByDescending(x => x.his_dt_data).FirstOrDefault();

            return h;
        }

    }
}