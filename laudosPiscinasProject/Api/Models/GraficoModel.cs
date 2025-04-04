using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Linq;

namespace laudosPiscinasProject.Api.Models
{
    public class GraficoModel
    {
        internal static List<RdoGeradoViewModel> ContarRdosGerados(int ColaboradorId, dynamic param)
        {
            string periodoIni = param.periodoIni.Value ?? "";
            string periodoFim = param.periodoFim.Value ?? "";

            if (string.IsNullOrWhiteSpace(periodoIni) || string.IsNullOrWhiteSpace(periodoFim))
            {
                throw new Exception("O período não foi fornecido.");
            }

            //string select = @"select obra.obr_ds_obra as obr_ds_obra, (select count(rdo.rdo_id_rdo) as gerado from rdo
            //    inner join status_rdo on status_rdo.str_id_status = rdo.rdo_id_status
            //    where rdo.rdo_id_obra = obra.obr_id_obra AND status_rdo.str_ds_status = 'GERADO'
            //    AND rdo.rdo_id_colaborador = {0} AND rdo.rdo_dt_rdo BETWEEN '{1}' AND '{2}')GERADO,
            //    (select count(rdo.rdo_id_rdo) as assinado_contratante from rdo inner join status_rdo on status_rdo.str_id_status = rdo.rdo_id_status
            //    where rdo.rdo_id_obra = obra.obr_id_obra AND status_rdo.str_ds_status = 'ASSINADO CONTRATANTE'
            //    AND rdo.rdo_id_colaborador = {0} AND rdo.rdo_dt_rdo BETWEEN '{1}' AND '{2}') ASSINADO_CONTRATANTE,
            //    (select count(rdo.rdo_id_rdo) as assinado_contratada from rdo inner join status_rdo on status_rdo.str_id_status = rdo.rdo_id_status
            //    where rdo.rdo_id_obra = obra.obr_id_obra AND status_rdo.str_ds_status = 'ASSINADO CONTRATADA' AND rdo.rdo_id_colaborador = {0} AND rdo.rdo_dt_rdo BETWEEN '{1}' AND '{2}') ASSINADO_CONTRATADA,
            //    (datediff(obra.obr_dt_previsao_fim, obra.obr_dt_inicio) - (select count(rdo.rdo_id_rdo) from rdo
            //    where rdo.rdo_id_obra = obra.obr_id_obra) - (select Abater_Final_Semana (obra.obr_dt_inicio,
            //    datediff(obra.obr_dt_previsao_fim,obra.obr_dt_inicio),obra.obr_nr_horas_sabado,obra.obr_nr_horas_domingo)))a_gerar from obra where obra.obr_id_colaborador = {0} and obra.obr_dt_fim is null";

            string select = @"SELECT o.obr_ds_obra AS obr_ds_obra,
  (SELECT count(rdo.rdo_id_rdo) AS gerado
   FROM rdo
   INNER JOIN status_rdo ON status_rdo.str_id_status = rdo.rdo_id_status
   WHERE rdo.rdo_id_obra = o.obr_id_obra
     AND status_rdo.str_ds_status = 'GERADO'
     AND rdo.rdo_dt_rdo BETWEEN o.obr_dt_inicio AND '{2}')GERADO,
  (SELECT count(rdo.rdo_id_rdo) AS assinado_contratante
   FROM rdo
   INNER JOIN status_rdo ON status_rdo.str_id_status = rdo.rdo_id_status
   WHERE rdo.rdo_id_obra = o.obr_id_obra
     AND status_rdo.str_ds_status = 'ASSINADO CONTRATANTE'
     AND rdo.rdo_dt_rdo BETWEEN o.obr_dt_inicio AND '{2}') ASSINADO_CONTRATANTE,
  (SELECT count(rdo.rdo_id_rdo) AS assinado_contratada
   FROM rdo
   INNER JOIN status_rdo ON status_rdo.str_id_status = rdo.rdo_id_status
   WHERE rdo.rdo_id_obra = o.obr_id_obra
     AND status_rdo.str_ds_status = 'ASSINADO CONTRATADA'
     AND rdo.rdo_dt_rdo BETWEEN o.obr_dt_inicio AND '{2}') ASSINADO_CONTRATADA,
       (datediff(o.obr_dt_previsao_fim, o.obr_dt_inicio) -
          (SELECT count(rdo.rdo_id_rdo)
           FROM rdo
           WHERE rdo.rdo_id_obra = o.obr_id_obra))a_gerar
FROM obra o
INNER JOIN obra_colaborador oc on o.obr_id_obra = oc.oco_id_obra
WHERE oc.oco_id_colaborador = {0};";





            var query = CreateQuery(select, ColaboradorId, DateTime.Parse(periodoIni).ToString("yyyy-MM-dd"), DateTime.Parse(periodoFim).ToString("yyyy-MM-dd"));

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            var result = context.Database.SqlQuery<RdoGeradoViewModel>(query);

            return result.ToList();
        }

        internal static ICollection<StatusTarefaGraficoViewModel> ContarStatusTarefa(int ColaboradorId, dynamic param)
        {
            string periodoIni = param.periodoIni ?? "";
            string periodoFim = param.periodoFim ?? "";

            if (string.IsNullOrWhiteSpace(periodoIni) || string.IsNullOrWhiteSpace(periodoFim))
            {
                throw new Exception("O período não foi fornecido.");
            }

            string select = @"SELECT obra.obr_ds_obra,
  (select count(distinct t.tar_nr_agrupador) from tarefa t
	 inner join etapa e on e.eta_id_etapa = t.tar_id_etapa
	 where e.eta_id_obra = obra.obr_id_obra
	 and t.tar_dt_ultima_atualizacao = (select max(t2.tar_dt_ultima_atualizacao)
			 from tarefa t2 where t2.tar_nr_agrupador = t.tar_nr_agrupador)
	 and t.tar_id_status = 1) AS PLANEJADA,
  (select count(distinct t.tar_nr_agrupador) from tarefa t
	 inner join etapa e on e.eta_id_etapa = t.tar_id_etapa
	 where e.eta_id_obra = obra.obr_id_obra
	 and t.tar_dt_ultima_atualizacao = (select max(t2.tar_dt_ultima_atualizacao)
			 from tarefa t2 where t2.tar_nr_agrupador = t.tar_nr_agrupador)
	 and t.tar_id_status = 2) AS EM_EXECUCAO,
  (select count(distinct t.tar_nr_agrupador) from tarefa t
	 inner join etapa e on e.eta_id_etapa = t.tar_id_etapa
	 where e.eta_id_obra = obra.obr_id_obra
	 and t.tar_dt_ultima_atualizacao = (select max(t2.tar_dt_ultima_atualizacao)
			 from tarefa t2 where t2.tar_nr_agrupador = t.tar_nr_agrupador)
	 and t.tar_id_status = 3) AS FINALIZADA,
  (select count(distinct t.tar_nr_agrupador) from tarefa t
	 inner join etapa e on e.eta_id_etapa = t.tar_id_etapa
	 where e.eta_id_obra = obra.obr_id_obra
	 and t.tar_dt_ultima_atualizacao = (select max(t2.tar_dt_ultima_atualizacao)
			 from tarefa t2 where t2.tar_nr_agrupador = t.tar_nr_agrupador)
	 and t.tar_id_status = 4) AS PAUSADA,
  (select count(distinct t.tar_nr_agrupador) from tarefa t
	 inner join etapa e on e.eta_id_etapa = t.tar_id_etapa
	 where e.eta_id_obra = obra.obr_id_obra
	 and t.tar_dt_ultima_atualizacao = (select max(t2.tar_dt_ultima_atualizacao)
			 from tarefa t2 where t2.tar_nr_agrupador = t.tar_nr_agrupador)
	 and t.tar_id_status = 5) AS CANCELADA
FROM obra
INNER JOIN obra_colaborador oc on obra.obr_id_obra = oc.oco_id_obra
WHERE oc.oco_id_colaborador = {0};";

            LaudosPiscinasEntities LaudosPiscinasEntities = new LaudosPiscinasEntities();

            string query = CreateQuery(select, ColaboradorId, DateTime.Parse(periodoIni).ToString("yyyy-MM-dd"), DateTime.Parse(periodoFim).ToString("yyyy-MM-dd"));
            var result = LaudosPiscinasEntities.Database.SqlQuery<StatusTarefaGraficoViewModel>(query);

            return result.ToList();
        }

        internal static ICollection<ComentarioViewModel> ContarComentario(int ColaboradorId, dynamic param)
        {
            string periodoIni = param.periodoIni ?? "";
            string periodoFim = param.periodoFim ?? "";

            if (string.IsNullOrWhiteSpace(periodoIni) || string.IsNullOrWhiteSpace(periodoFim))
            {
                throw new Exception("O período não foi fornecido.");
            }

            string select = @"SELECT obra.obr_ds_obra,
  (SELECT count(rdo.rdo_tp_comentario_geracao)
   FROM rdo
   WHERE rdo.rdo_id_obra = obra.obr_id_obra
     AND rdo.rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}'
     AND rdo.rdo_tp_comentario_geracao = 'P'
     AND rdo.rdo_tp_comentario_assinatura = 't') AS POSITIVO_CONTRATANTE,
  (SELECT count(rdo.rdo_tp_comentario_geracao)
   FROM rdo
   WHERE rdo.rdo_id_obra = obra.obr_id_obra
     AND rdo.rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}'
     AND rdo.rdo_tp_comentario_geracao = 'P'
     AND rdo.rdo_tp_comentario_assinatura = 'd') AS POSITIVO_CONTRATADA,
  (SELECT count(rdo.rdo_tp_comentario_geracao)
   FROM rdo
   WHERE rdo.rdo_id_obra = obra.obr_id_obra
     AND rdo.rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}'
     AND rdo.rdo_tp_comentario_geracao = 'N'
     AND rdo.rdo_tp_comentario_assinatura = 't') AS NEGATIVO_CONTRATANTE,
  (SELECT count(rdo.rdo_tp_comentario_geracao)
   FROM rdo
   WHERE rdo.rdo_id_obra = obra.obr_id_obra
     AND rdo.rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}'
     AND rdo.rdo_tp_comentario_geracao = 'N'
     AND rdo.rdo_tp_comentario_assinatura = 'd') AS NEGATIVO_CONTRATADA
FROM obra
INNER JOIN obra_colaborador oc on obra.obr_id_obra = oc.oco_id_obra
WHERE oc.oco_id_colaborador = {0};";

            LaudosPiscinasEntities LaudosPiscinasEntities = new LaudosPiscinasEntities();

            string query = CreateQuery(select, ColaboradorId, DateTime.Parse(periodoIni).ToString("yyyy-MM-dd"), DateTime.Parse(periodoFim).ToString("yyyy-MM-dd"));
            var result = LaudosPiscinasEntities.Database.SqlQuery<ComentarioViewModel>(query);

            return result.ToList();
        }

        internal static ICollection<TarefaViewModel> ContarTarefa(int ColaboradorId, dynamic param)
        {

            string PeriodoIni = param.periodoIni ?? "";
            string PeriodoFim = param.periodoFim ?? "";

            if (string.IsNullOrWhiteSpace(PeriodoIni) || string.IsNullOrWhiteSpace(PeriodoFim))
            {
                throw new Exception("O período não foi fornecido.");
            }

            string select = @"SELECT obra.obr_ds_obra,
  (select count(tabela.tar_ds_tarefa) from (
	 select e.eta_id_obra, t.tar_nr_agrupador, t.tar_ds_tarefa, max(t.tar_id_status) as tar_id_status from tarefa t
	 inner join etapa e on e.eta_id_etapa = t.tar_id_etapa
	 where t.tar_dt_inicio <= now()
	 group by e.eta_id_obra, t.tar_nr_agrupador, t.tar_ds_tarefa
	 having max(t.tar_id_status) = 1
	 ) tabela where tabela.eta_id_obra = obra.obr_id_obra ) AS ATRASO_INICIO,
  (select count(distinct t.tar_nr_agrupador)
     from tarefa t
	 inner join etapa e on e.eta_id_etapa = t.tar_id_etapa
	 where e.eta_id_obra = obra.obr_id_obra 
     and t.tar_dt_previsao_fim <= now()
     and t.tar_dt_ultima_atualizacao = (select max(t2.tar_dt_ultima_atualizacao)
		 from tarefa t2 where t2.tar_nr_agrupador = t.tar_nr_agrupador)
	 and t.tar_id_status not in (3, 5) ) AS ATRASO_FIM,
  (select count(distinct t.tar_nr_agrupador) from tarefa t
	 inner join etapa e on e.eta_id_etapa = t.tar_id_etapa
	 where e.eta_id_obra = obra.obr_id_obra
	 and t.tar_dt_ultima_atualizacao = (select max(t2.tar_dt_ultima_atualizacao)
			 from tarefa t2 where t2.tar_nr_agrupador = t.tar_nr_agrupador)
	 and t.tar_id_status = 4) AS PAUSADAS,
  (select count(distinct t.tar_nr_agrupador) from tarefa t
	 inner join etapa e on e.eta_id_etapa = t.tar_id_etapa
	 where e.eta_id_obra = obra.obr_id_obra
	 and t.tar_dt_ultima_atualizacao = (select max(t2.tar_dt_ultima_atualizacao)
			 from tarefa t2 where t2.tar_nr_agrupador = t.tar_nr_agrupador)
	 and t.tar_id_status = 5) AS CANCELADAS
FROM obra
INNER JOIN obra_colaborador oc on obra.obr_id_obra = oc.oco_id_obra
WHERE oc.oco_id_colaborador = {0};";

            LaudosPiscinasEntities LaudosPiscinasEntities = new LaudosPiscinasEntities();

            string query = CreateQuery(select, ColaboradorId, DateTime.Parse(PeriodoIni).ToString("yyyy-MM-dd"), DateTime.Parse(PeriodoFim).ToString("yyyy-MM-dd"));
            var result = LaudosPiscinasEntities.Database.SqlQuery<TarefaViewModel>(query);

            return result.ToList();
        }

        internal static ICollection<DiaImprodutivoViewModel> ContarDiasImprodutivos(int ColaboradorId, dynamic param)
        {
            string periodoIni = param.periodoIni ?? "";
            string periodoFim = param.periodoFim ?? "";

            if (string.IsNullOrWhiteSpace(periodoIni) || string.IsNullOrWhiteSpace(periodoFim))
            {
                throw new Exception("O período não foi fornecido.");
            }

            string select = @"SELECT obra.obr_ds_obra,
  (SELECT count(rdo.rdo_id_rdo)
   FROM rdo
   INNER JOIN improdutividade ON rdo.rdo_id_improdutividade = improdutividade.imp_id_improdutividade
   WHERE improdutividade.imp_st_clima = 1
     AND obra.obr_id_obra = rdo.rdo_id_obra
     AND rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}') imp_clima,
  (SELECT count(rdo.rdo_id_rdo)
   FROM rdo
   INNER JOIN improdutividade ON rdo.rdo_id_improdutividade = improdutividade.imp_id_improdutividade
   WHERE improdutividade.imp_st_material = 1
     AND obra.obr_id_obra = rdo.rdo_id_obra
     AND rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}') imp_falta_material,

  (SELECT count(rdo.rdo_id_rdo)
   FROM rdo
   INNER JOIN improdutividade ON rdo.rdo_id_improdutividade = improdutividade.imp_id_improdutividade
   WHERE improdutividade.imp_st_paralizacao = 1
     AND obra.obr_id_obra = rdo.rdo_id_obra
     AND rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}') imp_paralizacao,
  (SELECT count(rdo.rdo_id_rdo)
   FROM rdo
   INNER JOIN improdutividade ON rdo.rdo_id_improdutividade = improdutividade.imp_id_improdutividade
   WHERE improdutividade.imp_st_acidentes = 1
     AND obra.obr_id_obra = rdo.rdo_id_obra
     AND rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}') imp_acidente,
  (SELECT count(rdo.rdo_id_rdo)
   FROM rdo
   INNER JOIN improdutividade ON rdo.rdo_id_improdutividade = improdutividade.imp_id_improdutividade
   WHERE improdutividade.imp_st_contratante = 1
     AND obra.obr_id_obra = rdo.rdo_id_obra
     AND rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}') imp_contratante,
  (SELECT count(rdo.rdo_id_rdo)
   FROM rdo
   INNER JOIN improdutividade ON rdo.rdo_id_improdutividade = improdutividade.imp_id_improdutividade
   WHERE improdutividade.imp_st_equipamento = 1
     AND obra.obr_id_obra = rdo.rdo_id_obra
     AND rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}') imp_equipamento,
  (SELECT count(rdo.rdo_id_rdo)
   FROM rdo
   INNER JOIN improdutividade ON rdo.rdo_id_improdutividade = improdutividade.imp_id_improdutividade
   WHERE improdutividade.imp_st_fornecedores = 1
     AND obra.obr_id_obra = rdo.rdo_id_obra
     AND rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}') imp_fornecedores,
  (SELECT count(rdo.rdo_id_rdo)
   FROM rdo
   INNER JOIN improdutividade ON rdo.rdo_id_improdutividade = improdutividade.imp_id_improdutividade
   WHERE improdutividade.imp_st_maodeobra = 1
     AND obra.obr_id_obra = rdo.rdo_id_obra
     AND rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}') imp_maodeobra,
  (SELECT count(rdo.rdo_id_rdo)
   FROM rdo
   INNER JOIN improdutividade ON rdo.rdo_id_improdutividade = improdutividade.imp_id_improdutividade
   WHERE improdutividade.imp_st_projeto = 1
     AND obra.obr_id_obra = rdo.rdo_id_obra
     AND rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}') imp_projeto,
  (SELECT count(rdo.rdo_id_rdo)
   FROM rdo
   INNER JOIN improdutividade ON rdo.rdo_id_improdutividade = improdutividade.imp_id_improdutividade
   WHERE improdutividade.imp_st_planejamento = 1
     AND obra.obr_id_obra = rdo.rdo_id_obra
     AND rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}') imp_planejamento
FROM obra
INNER JOIN obra_colaborador oc on obra.obr_id_obra = oc.oco_id_obra
WHERE oc.oco_id_colaborador = {0};";

            LaudosPiscinasEntities LaudosPiscinasEntities = new LaudosPiscinasEntities();

            string query = CreateQuery(select, ColaboradorId, DateTime.Parse(periodoIni).ToString("yyyy-MM-dd"), DateTime.Parse(periodoFim).ToString("yyyy-MM-dd"));
            var result = LaudosPiscinasEntities.Database.SqlQuery<DiaImprodutivoViewModel>(query);

            return result.ToList();

        }

        internal static List<RdoAtrasadoViewModel> ContarRdosAtrasados(int ColaboradorId, dynamic param)
        {
            string periodoIni = param.periodoIni.Value ?? "";
            string periodoFim = param.periodoFim.Value ?? "";

            if (string.IsNullOrEmpty(periodoIni) || string.IsNullOrEmpty(periodoFim))
            {
                throw new Exception("O período não foi fornecido.");
            }

            string select = @"SELECT obra.obr_ds_obra,
  (SELECT DATEDIFF(IFNULL(MAX(rdo.rdo_dt_rdo), obra.obr_dt_inicio), now())
   FROM rdo
   WHERE rdo.rdo_id_obra = obra.obr_id_obra
     AND rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}') AS DIAS_ATRASO,
  (SELECT COUNT(RDO_OBRA_COLABORADOR.RDO_ID_RDO) AS QUANT_RDO
   FROM
     (SELECT rdo.rdo_id_rdo,
             obra.obr_id_obra
      FROM obra_colaborador
      INNER JOIN rdo ON rdo.rdo_id_obra = obra_colaborador.oco_id_obra
      INNER JOIN obra ON obra.obr_id_obra = obra_colaborador.oco_id_obra
      AND obra.obr_id_obra = rdo.rdo_id_obra
      WHERE rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}'
        AND obra_colaborador.oco_st_contratante_contratada = 'd') AS RDO_OBRA_COLABORADOR
   LEFT OUTER JOIN assinatura_rdo ON assinatura_rdo.ass_id_rdo = RDO_OBRA_COLABORADOR.RDO_ID_RDO
   WHERE assinatura_rdo.ass_id_rdo IS NULL
     AND RDO_OBRA_COLABORADOR.obr_id_obra = obra.obr_id_obra) AS sem_ass_contratada,
  (SELECT COUNT(RDO_OBRA_COLABORADOR.RDO_ID_RDO) AS QUANT_RDO
   FROM
     (SELECT rdo.rdo_id_rdo,
             obra.obr_id_obra
      FROM obra_colaborador
      INNER JOIN rdo ON rdo.rdo_id_obra = obra_colaborador.oco_id_obra
      INNER JOIN obra ON obra.obr_id_obra = obra_colaborador.oco_id_obra
      AND obra.obr_id_obra = rdo.rdo_id_obra
      WHERE rdo_dt_rdo BETWEEN obra.obr_dt_inicio AND '{2}'
        AND obra_colaborador.oco_st_contratante_contratada = 't') AS RDO_OBRA_COLABORADOR
   LEFT OUTER JOIN assinatura_rdo ON assinatura_rdo.ass_id_rdo = RDO_OBRA_COLABORADOR.RDO_ID_RDO
   WHERE assinatura_rdo.ass_id_rdo IS NULL
     AND RDO_OBRA_COLABORADOR.obr_id_obra = obra.obr_id_obra) AS sem_ass_contratante
FROM obra
INNER JOIN obra_colaborador oc on obra.obr_id_obra = oc.oco_id_obra
WHERE oc.oco_id_colaborador = {0};";

            LaudosPiscinasEntities Context = new LaudosPiscinasEntities();

            string query = CreateQuery(select, ColaboradorId, DateTime.Parse(periodoIni).ToString("yyyy-MM-dd"), DateTime.Parse(periodoFim).ToString("yyyy-MM-dd"));
            var result = Context.Database.SqlQuery<RdoAtrasadoViewModel>(query);

            var rdosAtrasados = result.ToList();

            FormatRdoAtrasados(rdosAtrasados);

            return rdosAtrasados;
        }

        private static string CreateQuery(string query, int ColaboradorId, string periodoIni, string periodoFim)
        {
            return string.Format(query, ColaboradorId, periodoIni.ToString(), periodoFim.ToString());
        }

        private static string CreateQuery(string query, int ColaboradorId)
        {
            return string.Format(query, ColaboradorId);
        }

        private static void FormatRdoAtrasados(List<RdoAtrasadoViewModel> rdosAtrasados)
        {
            rdosAtrasados.ForEach(r =>
            {
                if (r.dias_atraso < 0)
                {
                    r.dias_atraso = Math.Abs((int)r.dias_atraso);
                }

                if (r.dias_atraso == null)
                {
                    r.dias_atraso = 0;
                }
            });
        }

        public class RdoAtrasadoViewModel
        {
            public string obr_ds_obra { get; set; }
            public Nullable<int> dias_atraso { get; set; }
            public int sem_ass_contratante { get; set; }
            public int sem_ass_contratada { get; set; }
        }

        public class DiaImprodutivoViewModel
        {
            public string obr_ds_obra { get; set; }
            public int imp_clima { get; set; }
            public int imp_falta_material { get; set; }
            public int imp_paralizacao { get; set; }
            public int imp_acidente { get; set; }
            public int imp_contratante { get; set; }
            public int imp_equipamento { get; set; }
            public int imp_fornecedores { get; set; }
            public int imp_maodeobra { get; set; }
            public int imp_projeto { get; set; }
            public int imp_planejamento { get; set; }
        }

        public class TarefaViewModel
        {
            public string obr_ds_obra { get; set; }
            public int atraso_inicio { get; set; }
            public int atraso_fim { get; set; }
            public int pausadas { get; set; }
            public int canceladas { get; set; }
        }

        public class StatusTarefaGraficoViewModel
        {
            public string obr_ds_obra { get; set; }
            public int planejada { get; set; }
            public int em_execucao { get; set; }
            public int finalizada { get; set; }
            public int pausada { get; set; }
            public int cancelada { get; set; }
        }

        public class ComentarioViewModel
        {
            public string obr_ds_obra { get; set; }
            public int positivo_contratante { get; set; }
            public int negativo_contratante { get; set; }

            public int positivo_contratada { get; set; }
            public int negativo_contratada { get; set; }
        }

        public class RdoGeradoViewModel
        {
            public string obr_ds_obra { get; set; }
            public int gerado { get; set; }
            public int assinado_contratante { get; set; }
            public int assinado_contratada { get; set; }
            public int a_gerar { get; set; }
        }
    }
}