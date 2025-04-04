using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace laudosPiscinasProject.Api.Models
{
    public class DashboardModel
    {
        internal static object CarregarDashboards(dynamic param)
        {
            dynamic dynObj = new System.Dynamic.ExpandoObject();
            dynObj.pizzaData = CarregarDashboardPizza(param);
            dynObj.barraDataCategories = CarregarCategoriasDashboardBarra(param);
            dynObj.barraDataValues = CarregarValoresDashboardBarra(param);
            dynObj.pizzaEfetivoDataValues = CarregarValoresEfetivo(param);
            
            return dynObj;
        }


        internal static List<dynamic> CarregarDashboardPizza(dynamic param)
        {
            int idObra = (int)param.idObra;
            List<dynamic> lista = new List<dynamic>();
            

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            List<obra_colaborador> _obra_colaborador = context.obra_colaborador.Where(x => x.oco_id_obra == idObra).ToList();

            var cargosAgrupados = _obra_colaborador.GroupBy(c => c.cargo.car_ds_cargo).
                     Select(group =>
                         new
                         {
                             DescricaoCargo = group.Key,
                             QuantidadeCargo = group.Count()
                         });

            foreach (var item in cargosAgrupados)
            {
                dynamic dynObj = new System.Dynamic.ExpandoObject();
                dynObj.name = "<b>" + item.DescricaoCargo + ": " + item.QuantidadeCargo + "</b>";
                dynObj.y = item.QuantidadeCargo;
                lista.Add(dynObj);
            }

            return lista;
        }


        internal static List<dynamic> CarregarCategoriasDashboardBarra(dynamic param)
        {
            int idObra = (int)param.idObra;
            List<dynamic> lista = new List<dynamic>();


            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            List<tarefa> _tarefa = context.tarefa.Where(x => x.etapa.eta_id_obra == idObra).ToList();

            var satusAgrupados = _tarefa.GroupBy(s => s.status_tarefa.stt_ds_status).
                     Select(group =>
                         new
                         {
                             DescricaoSatus = group.Key,
                             QuantidadeSatus = group.Count()
                         }).OrderBy(x => x.QuantidadeSatus);

            satusAgrupados = satusAgrupados.GroupBy(test => test.DescricaoSatus).Select(grp => grp.First()).OrderBy(x => x.QuantidadeSatus);

            foreach (var item in satusAgrupados)
            {
                lista.Add(item.DescricaoSatus);
            }

            return lista;
        }


        internal static List<dynamic> CarregarValoresDashboardBarra(dynamic param)
        {
            int idObra = (int)param.idObra;
            List<dynamic> lista = new List<dynamic>();


            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            List<tarefa> _tarefa = context.tarefa.Where(x => x.etapa.eta_id_obra == idObra).ToList();


            var satusAgrupados = _tarefa.GroupBy(s => s.status_tarefa.stt_ds_status).
                     Select(group =>
                         new
                         {
                             DescricaoSatus = group.Key,
                             QuantidadeSatus = group.Count()
                         }).OrderBy(x => x.QuantidadeSatus);

            foreach (var item in satusAgrupados)
            {
                lista.Add(item.QuantidadeSatus);
            }

            return lista;
        }

        internal static ICollection<dynamic> CarregarValoresEfetivo(dynamic param)
        {
            string idObra = param.idObra ?? "";
            DateTime data = DateTime.Today.AddMonths(-1);

            if (string.IsNullOrWhiteSpace(idObra))
                throw new Exception("O id da obra é origatório.");

            string select = @"select est.est_ds_efetivo_status, count(*) as quantidade
from obra_colaborador oco 
inner join colaborador col on col.col_id_colaborador = oco.oco_id_colaborador
inner join cargo car on car.car_id_cargo = oco.oco_id_cargo
inner join efetivo efe on efe.efe_id_obra_colaborador = oco.oco_id_obra_colaborador
inner join efetivo_status est on est.est_id_efetivo_status = efe.efe_id_efetivo_status
where oco.oco_id_obra = {0}
and efe.efe_data >= '{1}'  
group by est.est_ds_efetivo_status
order by est_ds_efetivo_status; ";

            var query = string.Format(select, idObra, data.ToString("yyyy-MM-dd"));

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            var result = context.Database.SqlQuery<EfetivoViewModel>(query);

            List<dynamic> lista = new List<dynamic>();
            foreach (var item in result.ToList())
            {
                dynamic dynObj = new System.Dynamic.ExpandoObject();
                dynObj.name = "<b>" + item.est_ds_efetivo_status.ToUpper() + ": " + item.quantidade + "</b>";
                dynObj.y = item.quantidade;
                lista.Add(dynObj);
            }

            return lista;
        }

    }
}