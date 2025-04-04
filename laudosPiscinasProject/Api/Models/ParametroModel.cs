using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Linq;

namespace laudosPiscinasProject.Api.Models
{
    public class ParametroModel
    {
        public static List<parametro> ObterParametros(dynamic param)
        {
            string descricao = param.descricao;

            LaudosPiscinasEntities context = new LaudosPiscinasEntities();
            context.Configuration.LazyLoadingEnabled = false;

            return context.parametro.Where(u => u.par_ds_parametro.ToLower().Contains(descricao)).ToList();
        }

        public static string ObterValor(string alias)
        {
            try
            {
                LaudosPiscinasEntities bd = new LaudosPiscinasEntities();
                return bd.parametro.FirstOrDefault(p => p.par_ds_parametro.Contains(alias)).par_vl_parametro;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}