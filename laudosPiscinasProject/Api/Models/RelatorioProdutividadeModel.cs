using Microsoft.Reporting.WebForms;
using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using static laudosPiscinasProject.Api.Models.TarefaModel;

namespace laudosPiscinasProject.Api.Models
{
    public class RelatorioProdutividadeModel
    {
        public static byte[] GerarRelatorio(dynamic param)
        {

            int idObra = param.idObra ?? 0;

            if (param.dataInicial != "")
            {
                Convert.ToDateTime(param.dataInicial.ToString());
            }
            if (param.dataFinal != "")
            {
                Convert.ToDateTime(param.dataFinal.ToString());
            }
            int statusTarefa = Convert.ToInt32(param.statusTarefa.ToString());

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var dt = context.Database.SqlQuery<Teste>(@"
                select tar.tar_id_tarefa, tar.tar_ds_tarefa, car.car_id_cargo, car.car_ds_cargo, stt.stt_id_status, stt.stt_ds_status,
                    htr.his_nr_horas_trabalhadas
                from obra obr 
                join tarefa tar on tar.tar_id_obra = obr.obr_id_obra
                join historico_tarefa_rdo htr on htr.his_id_tarefa = tar.tar_id_tarefa
                join historico_tarefa_colaborador htc on htc.htc_id_historico_tarefa_rdo = htr.his_id_historico_tarefa_rdo
                join status_tarefa stt on htr.his_id_status = stt.stt_id_status
                join obra_colaborador obc on obc.oco_id_obra_colaborador = htc.htc_id_obra_colaborador
                join cargo car on obc.oco_id_cargo = car.car_id_cargo
                where obr.obr_id_obra = " + idObra + " group by tar.tar_id_tarefa, tar.tar_ds_tarefa, car.car_id_cargo, car.car_ds_cargo, stt.stt_id_status, stt.stt_ds_status")
                .ToList();
            
            string mappath = HttpContext.Current.Server.MapPath("~/Api/Contents/Reports/RelatorioProdutividade.rdlc");
            LocalReport ReportViewer = new LocalReport();
            ReportViewer.ReportPath = mappath;

            ReportViewer.DataSources.Add(new ReportDataSource("DataSet1", dt));
            CultureInfo cultureInfo = new CultureInfo("pt-BR");
            String dataAtual = DateTime.Now.ToString(cultureInfo);

            ReportViewer.SetParameters(new ReportParameter("dataAtual", dataAtual));
            return  ReportViewer.Render("Pdf");
        }

        public class Teste
        {
            public string tar_id_tarefa { get; set; }
            public string tar_ds_tarefa { get; set; }
            public string car_id_cargo { get; set; }
            public string car_ds_cargo { get; set; }
            public string stt_id_status { get; set; }
            public string stt_ds_status { get; set; }
            public string his_nr_horas_trabalhadas { get; set; }

        }
    }
}