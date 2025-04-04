using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace laudosPiscinasProject.Api.Models
{
    public class EfetivoModel
    {
        internal static ICollection<EfetivoViewModel> Lista(dynamic param)
        {
            string efe_data = param.efe_data ?? "";
            string oco_id_obra = param.oco_id_obra ?? "";
            string oco_id_cargo = param.oco_id_cargo ?? "";
            string col_nm_colaborador = param.col_nm_colaborador ?? "";
            string est_id_efetivo_status = param.est_id_efetivo_status ?? "";

            if (string.IsNullOrWhiteSpace(efe_data))
                throw new Exception("A data é obrigatória.");

            if (string.IsNullOrWhiteSpace(oco_id_obra))
                throw new Exception("O id da obra é origatório.");

            string select = @"select -1 as efe_id_efetivo, 
oco.oco_id_obra_colaborador, 
'{1}' as efe_data, 
col.col_nm_colaborador, 
car.car_ds_cargo, 
0 as est_id_efetivo_status, 
'' as est_ds_efetivo_status, 
'' as est_ds_color, 
oco.oco_id_obra 
from obra_colaborador oco 
inner join colaborador col on col.col_id_colaborador = oco.oco_id_colaborador
inner join cargo car on car.car_id_cargo = oco.oco_id_cargo
where oco.oco_id_obra = {0}
and not exists (select null from efetivo efe where efe.efe_id_obra_colaborador = oco.oco_id_obra_colaborador and efe.efe_data = '{1}') ";

            if (!string.IsNullOrWhiteSpace(oco_id_cargo) && Convert.ToInt32(oco_id_cargo) > 0)
                select += string.Format("and oco.oco_id_cargo = {0} ", oco_id_cargo);

            if (!string.IsNullOrWhiteSpace(col_nm_colaborador))
                select += string.Format("and col.col_nm_colaborador like '%{0}%' ", col_nm_colaborador);

            if (!string.IsNullOrWhiteSpace(est_id_efetivo_status) && Convert.ToInt32(est_id_efetivo_status) > 0)
                select += string.Format("and 0 = {0} ", est_id_efetivo_status);

            select = string.Format(select, oco_id_obra, DateTime.Parse(efe_data).ToString("yyyy-MM-dd"));
            select += " union ";

            select += @"select efe.efe_id_efetivo, 
oco.oco_id_obra_colaborador, 
efe.efe_data, 
col.col_nm_colaborador, 
car.car_ds_cargo, 
est.est_id_efetivo_status, 
est.est_ds_efetivo_status, 
est.est_ds_color, 
oco.oco_id_obra 
from obra_colaborador oco 
inner join colaborador col on col.col_id_colaborador = oco.oco_id_colaborador
inner join cargo car on car.car_id_cargo = oco.oco_id_cargo
inner join efetivo efe on efe.efe_id_obra_colaborador = oco.oco_id_obra_colaborador
inner join efetivo_status est on est.est_id_efetivo_status = efe.efe_id_efetivo_status
where oco.oco_id_obra = {0}
and efe.efe_data = '{1}' ";

            if (!string.IsNullOrWhiteSpace(oco_id_cargo) && Convert.ToInt32(oco_id_cargo) > 0)
                select += string.Format("and oco.oco_id_cargo = {0} ", oco_id_cargo);

            if (!string.IsNullOrWhiteSpace(col_nm_colaborador))
                select += string.Format("and col.col_nm_colaborador like '%{0}%' ", col_nm_colaborador);

            if (!string.IsNullOrWhiteSpace(est_id_efetivo_status) && Convert.ToInt32(est_id_efetivo_status) > 0)
                select += string.Format("and est.est_id_efetivo_status = {0} ", est_id_efetivo_status);

            select += @" order by col_nm_colaborador;";

            var query = string.Format(select, oco_id_obra, DateTime.Parse(efe_data).ToString("yyyy-MM-dd"));

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            var result = context.Database.SqlQuery<EfetivoViewModel>(query);

            return result.ToList();
        }

        internal static ICollection<EfetivoStatusViewModel> CarregarListaEfetivoStatus()
        {
            string query = @"select * from efetivo_status order by 2;";

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            var result = context.Database.SqlQuery<EfetivoStatusViewModel>(query);

            return result.ToList();
        }

        internal static EfetivoViewModel ChangeEfetivoStatus(EfetivoViewModel param)
        {

            string query = string.Format("update efetivo set efe_id_efetivo_status = {0} where efe_id_efetivo = {1};", param.est_id_efetivo_status, param.efe_id_efetivo);

            if (param.est_id_efetivo_status == 0)
                query = string.Format("delete from efetivo where efe_id_efetivo = {0};", param.efe_id_efetivo);
            else if (param.efe_id_efetivo < 0)
                query = string.Format(@"insert into efetivo (efe_id_obra, efe_id_obra_colaborador, efe_id_efetivo_status, efe_data) 
                    values ({0}, {1}, {2}, '{3}')", param.oco_id_obra, param.oco_id_obra_colaborador, param.est_id_efetivo_status, param.efe_data.ToString("yyyy-MM-dd"));

            using (var context = new LaudosPiscinasEntities())
            {
                context.Database.ExecuteSqlCommand(query);
            }

            return param;
        }

        internal static DateTime UltimaDataLancada(string oco_id_obra)
        {
            if (string.IsNullOrWhiteSpace(oco_id_obra))
                throw new Exception("O id da obra é origatório.");

            string select = @"select MAX(efe.efe_data)  as efe_data
from obra_colaborador oco 
inner join colaborador col on col.col_id_colaborador = oco.oco_id_colaborador
inner join cargo car on car.car_id_cargo = oco.oco_id_cargo
inner join efetivo efe on efe.efe_id_obra_colaborador = oco.oco_id_obra_colaborador
inner join efetivo_status est on est.est_id_efetivo_status = efe.efe_id_efetivo_status
where oco.oco_id_obra = {0} ";

            var query = string.Format(select, oco_id_obra);

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            var efe_data = context.Database.SqlQuery<DateTime?>(query).FirstOrDefault();
            var result = efe_data ?? DateTime.Now.Date;

            return result;
        }

        internal static bool CopiarEfetivo(dynamic param)
        {
            if (param.data_copia == null)
                return false;

            List<EfetivoViewModel> lista = Lista(param);
            foreach(var item in lista)
            {
                if (item.efe_id_efetivo > 0)
                {
                    item.efe_id_efetivo = -1;
                    string data = param.data_copia;

                    item.efe_data = Convert.ToDateTime(data);
                    var efetivo = ObterEfetivo(item);
                    if (efetivo == null)
                        ChangeEfetivoStatus(item);
                }
            }
            return true;
        }

        internal static EfetivoViewModel ObterEfetivo(EfetivoViewModel efetivo)
        {
            string select = @"select * from efetivo e where e.efe_id_obra = {0} and e.efe_id_obra_colaborador = {1} and e.efe_data = '{2}' ";

            var query = string.Format(select, efetivo.oco_id_obra, efetivo.oco_id_obra_colaborador, efetivo.efe_data.ToString("yyyy-MM-dd"));

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            var result = context.Database.SqlQuery<EfetivoViewModel>(query).FirstOrDefault();

            return result;
        }
    }
    public class EfetivoViewModel
    {
        public long? efe_id_efetivo { get; set; }
        public long oco_id_obra_colaborador { get; set; }
        public string efe_dataStr => efe_data.ToString("dd/MM/yyyy");
        public DateTime efe_data { get; set; }
        public string col_nm_colaborador { get; set; }
        public string car_ds_cargo { get; set; }
        public long? est_id_efetivo_status { get; set; }
        public string est_ds_efetivo_status { get; set; }
        public string est_ds_color { get; set; }
        public long oco_id_obra { get; set; }
        public string obr_ds_obra { get; set; }
        public long quantidade { get; set; }
    }
    public class EfetivoStatusViewModel
    {
        public long est_id_efetivo_status { get; set; }
        public string est_ds_efetivo_status { get; set; }
        public string est_ds_color { get; set; }
    }
}