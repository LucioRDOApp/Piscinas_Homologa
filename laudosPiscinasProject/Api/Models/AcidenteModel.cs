using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace laudosPiscinasProject.Api.Models
{
    public class AcidenteModel
    {
        public static List<AcidenteViewModel> Lista()
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            IQueryable<acidente> query = context.Set<acidente>();

            List<AcidenteViewModel> Lista = new List<AcidenteViewModel>();
            query.ToList().ForEach(aci => Lista.Add(new AcidenteViewModel
            {
                Descricao = aci.aci_ds_acidente,
                DataAcidente = aci.aci_dt_data_hora ?? DateTime.MinValue,
                IdAcidente = aci.aci_id_acidente,
                listaAcidenteColaboradores = CarregarColaboradoresAcidente(aci)

            }));

            return Lista.OrderBy(x => x.DataAcidente).ToList();
        }
        public static List<AcidenteColaboradorViewModel> CarregarColaboradoresAcidente(acidente aci)
        {
            List<AcidenteColaboradorViewModel> result = new List<AcidenteColaboradorViewModel>();
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            context.acidente_colaborador.Where(x => x.acc_id_acidente == aci.aci_id_acidente).ToList().ForEach(x => result.Add(new AcidenteColaboradorViewModel {
                IdAcidente = x.acc_id_acidente,
                Nome = x.obra_colaborador.colaborador.col_nm_colaborador,
                HouveAfastamento = x.acc_st_atastamento,
                IdAcidenteColaborador = x.acc_id_acidente_colaborador,
                IdColaborador = x.obra_colaborador.oco_id_colaborador,
                DescricaoCargo = x.obra_colaborador.cargo.car_ds_cargo,
                Email = x.obra_colaborador.colaborador.col_ds_email,
                IdObraColaborador = x.obra_colaborador.oco_id_colaborador,
                TelefonePrincipal = x.obra_colaborador.colaborador.col_ds_telefone_principal

            }));

            return result;
        }        
        internal static AcidenteViewModel ObterRegistro(int idAcidente)
        {
            AcidenteViewModel acidente = new AcidenteViewModel();

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            var resultado = context.acidente.FirstOrDefault(aci => aci.aci_id_acidente == idAcidente);

            acidente.DataAcidente = resultado.aci_dt_data_hora ?? DateTime.MinValue;
            acidente.Descricao = resultado.aci_ds_acidente;
            acidente.HouveAfastamento = resultado.aci_st_afastamento;
            acidente.IdAcidente = resultado.aci_id_acidente;
            acidente.listaAcidenteColaboradores = new List<AcidenteColaboradorViewModel>();


            List<acidente_colaborador> acidenteColaborador = context.acidente_colaborador.Where(acc => acc.acc_id_acidente == idAcidente).ToList();


            acidenteColaborador.ForEach(x => acidente.listaAcidenteColaboradores.Add(new AcidenteColaboradorViewModel() {
                DescricaoCargo = x.obra_colaborador.cargo.car_ds_cargo,
                Email = x.obra_colaborador.colaborador.col_ds_email,
                HouveAfastamento = x.acc_st_atastamento,
                IdAcidente = x.acc_id_acidente,
                IdAcidenteColaborador = x.acc_id_acidente_colaborador,
                IdColaborador = x.obra_colaborador.oco_id_colaborador,
                IdObraColaborador = x.obra_colaborador.oco_id_obra_colaborador,
                Nome = x.obra_colaborador.colaborador.col_nm_colaborador,
                TelefonePrincipal = x.obra_colaborador.colaborador.col_ds_telefone_principal

            }));
            

            return acidente;
        }
        public static AcidenteViewModel Salvar(dynamic param)
        {
            if (string.IsNullOrEmpty(param.Descricao.ToString()))
            {
                throw new Exception("O titulo deve ser preenchido");
            }

            int idTarefa = param.IdTarefa ?? 0;
            int idAcidente = param.IdAcidente ?? 0;
            //int idColaborador = param.idColaborador ?? 0;
            //int idObraColaborador = param.idObraColaborador ?? 0;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            acidente _acidente = context.acidente.Where(x => x.aci_id_acidente == idAcidente).FirstOrDefault() ?? new acidente();

            _acidente.aci_ds_acidente = param.Descricao;
            _acidente.aci_id_tarefa = idTarefa;
            _acidente.aci_dt_data_hora = Convert.ToDateTime(Convert.ToString(param.DataAcidente));
            _acidente.aci_st_afastamento = param.HouveAfastamento;

            if (_acidente.aci_id_acidente > 0)
            {
                context.acidente.Attach(_acidente);
                context.Entry(_acidente).State = EntityState.Modified;
            }
            else
            {
                context.acidente.Add(_acidente);
            }

            bool result = context.SaveChanges() > 0;

            AcidenteViewModel acidenteSalvo = new AcidenteViewModel() {
                DataAcidente = (DateTime)_acidente.aci_dt_data_hora,
                Descricao = _acidente.aci_ds_acidente,
                HouveAfastamento = _acidente.aci_st_afastamento,
                IdAcidente = _acidente.aci_id_acidente,
                listaAcidenteColaboradores = new List<AcidenteColaboradorViewModel>()

            };

            SalvarListaColaboradorAcidentes(param, context, _acidente.aci_id_acidente);

            return acidenteSalvo;
        }
        internal static bool SalvarListaColaboradorAcidentes(dynamic param, LaudosPiscinasEntities context, int idAcidente)
        {
            bool result = true;
            context.acidente_colaborador.Where(x => x.acc_id_acidente == idAcidente).ToList().ForEach(y => context.acidente_colaborador.Remove(y));
            result = context.SaveChanges() > 0;


            foreach (var item in param.listaAcidenteColaboradores)
            {
                item.IdAcidente = idAcidente;
                string houveAfastamento = item.HouveAfastamento ?? "n";
                int idObraColaborador = item.IdObraColaborador ?? 0;
                bool acidenteColaboradorSalvo = AcidenteModel.AssociarAcidenteColaborador(idAcidente, idObraColaborador, houveAfastamento);
            }

            
            return result;
        }
        internal static bool Deletar(int idAcidente)
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            bool result = false;
            try
            {
                context.acidente_colaborador.Where(x => x.acc_id_acidente == idAcidente).ToList().ForEach(y => context.acidente_colaborador.Remove(y));
                context.acidente.Where(x => x.aci_id_acidente == idAcidente).ToList().ForEach(y => context.acidente.Remove(y));
                result = context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result;
        }
        public static bool AssociarAcidenteColaborador(int idAcidente, int idObraColaborador, string houveAfastamento)
        {

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            obra_colaborador _obra_colaborador = context.obra_colaborador.Where(x => x.oco_id_obra_colaborador == idObraColaborador).FirstOrDefault() ?? new obra_colaborador();
            acidente_colaborador _acidente_colaborador = context.acidente_colaborador.Where(x => x.acc_id_acidente == idAcidente && x.acc_id_obra_colaborador == idObraColaborador).FirstOrDefault() ?? new acidente_colaborador();

            _acidente_colaborador.acc_id_obra_colaborador = idObraColaborador;
            _acidente_colaborador.acc_id_acidente = idAcidente;
            _acidente_colaborador.acc_st_atastamento = houveAfastamento;


            if (_acidente_colaborador.acc_id_acidente_colaborador > 0)
            {
                context.acidente_colaborador.Attach(_acidente_colaborador);
                context.Entry(_acidente_colaborador).State = EntityState.Modified;
            }
            else
            {
                context.acidente_colaborador.Add(_acidente_colaborador);
            }

            bool result = context.SaveChanges() > 0;
            //bool result = true;

            return result;
        }
    }
    public class AcidenteViewModel
    {
        public int? IdTarefa { get; set; }
        public int IdAcidente { get; set; }
        public DateTime DataAcidente { get; set; }
        public string Descricao { get; set; }
        public string HouveAfastamento { get; set; }
        public virtual ICollection<AcidenteColaboradorViewModel> listaAcidenteColaboradores { get; set; }
        public AcidenteViewModel()
        {
            this.listaAcidenteColaboradores = new HashSet<AcidenteColaboradorViewModel>();
        }
    }
    public class AcidenteColaboradorViewModel
    {
        public int IdAcidenteColaborador { get; set; }
        public int IdAcidente { get; set; }
        public int IdColaborador { get; set; }
        public int IdObraColaborador { get; set; }
        public string Nome { get; set; }
        public string HouveAfastamento { get; set; }
        public string DescricaoCargo { get; set; }
        public string Email { get; set; }
        public string TelefonePrincipal { get; set; }
    }
}