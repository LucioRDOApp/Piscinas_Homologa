using Microsoft.Reporting.WebForms;
using LaudosPiscinasClass;
using laudosPiscinasProject.Api.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace laudosPiscinasProject.Api.Models
{
    public class RelatorioEfetivoModel
    {
        public static byte[] Efetivo(RelatorioEfetivoViewModel filter)
        {
            var lista = Lista(filter);
            var detalhesObra = GetDetalhesObra(filter);
            var quantidadePorStatus = GetQuantidadePorStatus(filter);

            DataTable dt = GetDataTable(lista);

            string mappath = HttpContext.Current.Server.MapPath("~/Api/Contents/Reports/RelatorioEfetivoDiario.rdlc");
            LocalReport ReportViewer = new LocalReport();
            ReportViewer.ReportPath = mappath;
            ReportViewer.EnableExternalImages = true; // para permitir adicionar a imagem da logomarca dinamicamente

            ReportViewer.DataSources.Add(new ReportDataSource("DataSet1", lista));

            string basePath = ConfigurationManager.AppSettings["basePath"];
            basePath = basePath.Remove(basePath.Length - 1);

            ReportViewer.SetParameters(new ReportParameter("obra", detalhesObra.obr_ds_obra));
            ReportViewer.SetParameters(new ReportParameter("qtdPresente", quantidadePorStatus.FirstOrDefault(l=>l.est_ds_efetivo_status.Equals("Presente"))?.quantidade ?? "0"));
            ReportViewer.SetParameters(new ReportParameter("qtdAusente", quantidadePorStatus.FirstOrDefault(l => l.est_ds_efetivo_status.Equals("Ausente"))?.quantidade ?? "0"));
            ReportViewer.SetParameters(new ReportParameter("qtdEmFalta", quantidadePorStatus.FirstOrDefault(l => l.est_ds_efetivo_status.Equals("Em falta"))?.quantidade ?? "0"));
            ReportViewer.SetParameters(new ReportParameter("qtdDeFolga", quantidadePorStatus.FirstOrDefault(l => l.est_ds_efetivo_status.Equals("De folga"))?.quantidade ?? "0"));

            string periodo = string.Format("De {0} Até {1}", ((DateTime)filter.DataInicial).ToString("dd/MM/yyyy"), ((DateTime)filter.DataFinal).ToString("dd/MM/yyyy"));
            ReportViewer.SetParameters(new ReportParameter("periodo", periodo));


            bool licencaLiberaLogo = detalhesObra.lic_st_permite_logo_rdo;
            string logoContratada = detalhesObra.obr_ds_foto;

            if (!licencaLiberaLogo || string.IsNullOrEmpty(logoContratada))
            {
                logoContratada = basePath + "/Assets/images/logo.jpg";
                ReportViewer.SetParameters(new ReportParameter("logoContratada", new Uri(HttpContext.Current.Server.MapPath(logoContratada)).AbsoluteUri)); //adiciona logomarca 
            }
            else
            {
                ReportViewer.SetParameters(new ReportParameter("logoContratada", new Uri(HttpContext.Current.Server.MapPath(basePath + logoContratada)).AbsoluteUri)); //adiciona logomarca 
            }

            if (filter.TipoRelatorio == TipoRelatorio.Excel)
            {
                return ReportViewer.Render("Excel");
            }
            return ReportViewer.Render("PDF");
        }

        internal static DataTable GetDataTable(List<EfetivoViewModel> lista)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("efe_id_efetivo");
            dt.Columns.Add("oco_id_obra_colaborador");
            dt.Columns.Add("efe_data");
            dt.Columns.Add("col_nm_colaborador");
            dt.Columns.Add("car_ds_cargo");
            dt.Columns.Add("est_id_efetivo_status");
            dt.Columns.Add("est_ds_efetivo_status");
            dt.Columns.Add("est_ds_color");
            dt.Columns.Add("oco_id_obra");

            lista.ForEach(l =>
                dt.Rows.Add(l.efe_id_efetivo, l.oco_id_obra_colaborador, l.efe_data.ToShortDateString(), l.col_nm_colaborador, l.car_ds_cargo, l.est_id_efetivo_status, l.est_ds_efetivo_status, l.est_ds_color, l.oco_id_obra)
            );
            dt.AcceptChanges();
            return dt;
        }

        internal static List<EfetivoViewModel> Lista(RelatorioEfetivoViewModel filter)
        {
            string select = @"select efe.efe_id_efetivo, 
oco.oco_id_obra_colaborador, 
efe.efe_data, 
col.col_nm_colaborador, 
car.car_ds_cargo, 
est.est_id_efetivo_status, 
est.est_ds_efetivo_status, 
est.est_ds_color, 
oco.oco_id_obra,
o.obr_ds_obra
from obra_colaborador oco 
inner join colaborador col on col.col_id_colaborador = oco.oco_id_colaborador
inner join cargo car on car.car_id_cargo = oco.oco_id_cargo
inner join efetivo efe on efe.efe_id_obra_colaborador = oco.oco_id_obra_colaborador
inner join efetivo_status est on est.est_id_efetivo_status = efe.efe_id_efetivo_status 
inner join obra o on o.obr_id_obra = oco_id_obra 
where o.obr_id_obra = {0} 
and efe.efe_data >= '{1}' and efe.efe_data <= '{2}'; ";

            var query = string.Format(select, filter.IdObra, ((DateTime)filter.DataInicial).ToString("yyyy-MM-dd"), ((DateTime)filter.DataFinal).ToString("yyyy-MM-dd"));
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            var result = context.Database.SqlQuery<EfetivoViewModel>(query);

            return result.ToList();
        }

        internal static DetalhesObra GetDetalhesObra(RelatorioEfetivoViewModel filter)
        {
            string select = @"select o.obr_id_obra, o.obr_ds_obra, o.obr_ds_foto, l.lic_st_permite_logo_rdo from obra o 
inner join empresa e on e.emp_id_empresa = o.obr_id_empresa_contratada
inner join licenca l on l.lic_id_licenca = e.emp_id_licenca
where o.obr_id_obra = {0}; ";

            var query = string.Format(select, filter.IdObra);
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            var result = context.Database.SqlQuery<DetalhesObra>(query).FirstOrDefault();

            return result;
        }
        internal static List<QuantidadeStatus> GetQuantidadePorStatus(RelatorioEfetivoViewModel filter)
        {
            string select = @"select est.est_ds_efetivo_status, count(*) as quantidade
from obra_colaborador oco 
inner join colaborador col on col.col_id_colaborador = oco.oco_id_colaborador
inner join cargo car on car.car_id_cargo = oco.oco_id_cargo
inner join efetivo efe on efe.efe_id_obra_colaborador = oco.oco_id_obra_colaborador
inner join efetivo_status est on est.est_id_efetivo_status = efe.efe_id_efetivo_status 
inner join obra o on o.obr_id_obra = oco_id_obra 
where o.obr_id_obra = {0} 
and efe.efe_data >= '{1}' and efe.efe_data <= '{2}'
group by est.est_ds_efetivo_status; ";

            var query = string.Format(select, filter.IdObra, ((DateTime)filter.DataInicial).ToString("yyyy-MM-dd"), ((DateTime)filter.DataFinal).ToString("yyyy-MM-dd"));
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            var result = context.Database.SqlQuery<QuantidadeStatus>(query);

            return result.ToList();
        }
    }
}

public class QuantidadeStatus
{
    public string est_ds_efetivo_status { get; set; }
    public string quantidade { get; set; }
}

public class DetalhesObra
{
    public int? obr_id_obra { get; set; }
    public string obr_ds_obra { get; set; }
    public string obr_ds_foto { get; set; }
    public bool lic_st_permite_logo_rdo { get; set; }
}

public class RelatorioEfetivoViewModel
{
    public DateTime? DataInicial { get; set; }
    public DateTime? DataFinal { get; set; }
    public int? IdObra { get; set; }
    public TipoRelatorio? TipoRelatorio { get; set; }
    public RelatorioEfetivoViewModel() { }
    public RelatorioEfetivoViewModel(dynamic param)
    {
        if (param != null)
        {
            DataInicial = param.dataInicial;
            DataFinal = param.dataPrevisaoFinalObra;
            IdObra = param.idObra;
            TipoRelatorio = param.tipoRelatorio;
        }
    }
}