using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace laudosPiscinasProject.Api.Models
{
    public class LicencaModel
    {
        public static List<LicencaViewModel> Lista()
        {
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            List<LicencaViewModel> Lista = new List<LicencaViewModel>();
            context.licenca.ToList().ForEach(lic => Lista.Add(new LicencaViewModel
            {
                Id = lic.lic_id_licenca,
                Nome = lic.lic_ds_licenca
            }));

            return Lista.OrderBy(x => x.Nome).ToList();
        }

        public static bool Remove(dynamic param)
        {
            try
            {
                int? idLicenca = param.idLicenca;
                LaudosPiscinasEntities context = new LaudosPiscinasEntities();
                string codigoLoja = param.codigoLoja;
                context.licenca.Remove(context.licenca.FirstOrDefault(lic => lic.lic_id_licenca == idLicenca || lic.lic_id_licenca_loja == codigoLoja));
                return context.SaveChanges() > 0;
            }
            catch (Exception)
            {
                throw new Exception("Não foi possível remover a licença. Existem registros dependentes.");
            }
        }

        public static bool Atualizar(dynamic param)
        {
            try
            {
                int? idLicenca = param.idLicenca;

                string codigoLoja = param.codigoLoja;
                string tipoLicenca = param.tipoLicenca;
                int numUsuarios = Convert.ToInt32(param.numUsuarios);
                int numObras = Convert.ToInt32(param.numObras);
                int numFotos = Convert.ToInt32(param.numFotos);
                int numTarefas = Convert.ToInt32(param.numTarefas);
                int utilizarLogo = Convert.ToInt32(param.utilizarLogo);

                LaudosPiscinasEntities context = new LaudosPiscinasEntities();
                licenca objLicensa = context.licenca.FirstOrDefault(li => li.lic_id_licenca == idLicenca || li.lic_id_licenca_loja == codigoLoja) ?? new licenca();

                if (!string.IsNullOrEmpty(codigoLoja))
                    objLicensa.lic_id_licenca_loja = codigoLoja;

                objLicensa.lic_ds_licenca = tipoLicenca;
                objLicensa.lic_nr_qtd_obras = numObras;
                objLicensa.lic_nr_qtd_usuarios = numUsuarios;
                objLicensa.lic_qtd_imagens_tarefas = numFotos;
                objLicensa.lic_qtd_tarefas_obra = numTarefas;
                objLicensa.lic_st_permite_logo_rdo = utilizarLogo == 0 ? false : true;

                if (objLicensa.lic_id_licenca > 0)
                {
                    context.licenca.Attach(objLicensa);
                    context.Entry(objLicensa).State = EntityState.Modified;
                }
                else
                {
                    context.licenca.Add(objLicensa);
                }

                return context.SaveChanges() > 0;
            }
            catch(Exception)
            {
                throw new Exception("Não foi possível atualizar a licença, tente novamente.");
            }
        }
    }

    public class LicencaViewModel
    {
        public long Id { get; set; }
        public string Nome { get; set; }
    }
}