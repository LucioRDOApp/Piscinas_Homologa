using LaudosPiscinasClass;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace laudosPiscinasProject.Api.Models
{
    public class ImagemModel
    {
        internal static bool Salvar(dynamic param, LaudosPiscinasEntities context, string caminho)
        {
            bool result = true;

            return result;
        }

        //public static List<TarefaModel.ImageFile> ObterImagensTarefa(dynamic param)
        //{
        //    string basePath = ConfigurationManager.AppSettings["BasePath"];
        //    string PastaDeImagens = ConfigurationManager.AppSettings["PastaDeImagens"] ?? "~/Upload/Imagens/";
        //    PastaDeImagens = basePath == "/" ? PastaDeImagens : basePath + PastaDeImagens;

        //    long idTarefa = param ?? 0;
        //    LaudosPiscinasEntities context = new LaudosPiscinasEntities();

        //    DateTime DataComparacao = DateTime.Today.AddDays(-1);

        //    var lista = context.imagem.Where(x => x.ima_id_tarefa == idTarefa && x.ima_dt_imagem >= DataComparacao).ToList();

        //    var listaImagem = new List<TarefaModel.ImageFile>();

        //    lista.ForEach(x =>
        //        listaImagem.Add(new TarefaModel.ImageFile
        //        {
        //            id = x.ima_id_imagem,
        //            filename = x.ima_ds_caminho.Split('/')[x.ima_ds_caminho.Split('/').Length - 1],
        //            href = string.Concat(PastaDeImagens, x.ima_ds_caminho).Replace("~", ""), //Convert.ToBase64String(File.ReadAllBytes(string.Concat(PastaDeImagens, x.ima_ds_caminho))),
        //            filetype = TarefaModel.RetornaMimeType(x.ima_ds_caminho.Split('.')[1])
        //        })
        //    );

        //    return listaImagem;
        //}
        public static List<ImageFile> ObterImagensTarefa(dynamic param)
        {
            string basePath = ConfigurationManager.AppSettings["BasePath"];
            string PastaDeImagens = ConfigurationManager.AppSettings["PastaDeImagens"] ?? "~/Upload/Imagens/";
            PastaDeImagens = basePath == "/" ? PastaDeImagens : basePath + PastaDeImagens;

            long idTarefa = param ?? 0;
            LaudosPiscinasEntities context = new LaudosPiscinasEntities();

            DateTime DataComparacao = DateTime.Today.AddDays(-1);

            var lista = context.imagem.Where(x => x.ima_id_tarefa == idTarefa && x.ima_dt_imagem >= DataComparacao).ToList();

            var listaImagem = new List<ImageFile>();

            lista.ForEach(x =>
                listaImagem.Add(new ImageFile
                {
                    id = x.ima_id_imagem,
                    filename = x.ima_ds_caminho.Split('/')[x.ima_ds_caminho.Split('/').Length - 1],
                    href = string.Concat(PastaDeImagens, x.ima_ds_caminho).Replace("~", ""), //Convert.ToBase64String(File.ReadAllBytes(string.Concat(PastaDeImagens, x.ima_ds_caminho))),
                    filetype = TarefaModel.RetornaMimeType(x.ima_ds_caminho.Split('.')[1])
                })
            );

            return listaImagem;
        }

        public static List<ImageFile> ObterImagens(int idTarefa)
        {
            var imagemList = new List<ImageFile>();

            var tempPath = System.Configuration.ConfigurationManager.AppSettings["basePath"].ToString() == "/" ? "" : System.Configuration.ConfigurationManager.AppSettings["basePath"].ToString();


            using (var context = new LaudosPiscinasEntities())
            {
                var imagens = context.imagem.Where(i => i.ima_id_tarefa == idTarefa).ToList();

                imagens.ForEach(x =>
                {
                    imagemList.Add(new ImageFile
                    {
                        href = tempPath + x.ima_ds_caminho,
                        idRDO = x.ima_id_historico_tarefa_rdo
                    });
                });                
            }

            return imagemList;
        }

        public static List<ImagemViewModel> ObterImagensRdo(int IdRdo)
        {
            List<ImagemViewModel> imagens = new List<ImagemViewModel>();

            using (var context = new LaudosPiscinasEntities())
            {
                context.rdo_imagem.Where(i => i.rim_id_rdo == IdRdo).ToList().ForEach(im => imagens.Add(
                     new ImagemViewModel
                     {
                         idImagem = im.rim_id_imagem,
                         idTarefa = im.imagem.ima_id_tarefa,
                         agrupadorTarefa = im.imagem.tarefa.tar_nr_agrupador.ToString(),
                         idRdo = im.rim_id_rdo
                     }
                ));
            }

            return imagens;
        }

    }
    public class ImagemViewModel
    {
        public int idImagem { get; set; }
        public string dsCaminho { get; set; }
        public int? idHistoricoTarefaRDO { get; set; }
        public int idTarefa { get; set; }
        public string agrupadorTarefa { get; set; }
        public int idRdo { get; set; }
        public DateTime dtInclusao { get; set; }
        public ImagemViewModel() { }
        public ImagemViewModel(imagem entity)
        {
            if (entity != null)
            {
                idImagem = entity.ima_id_imagem;
                dsCaminho = entity.ima_ds_caminho;
                idHistoricoTarefaRDO = entity.ima_id_historico_tarefa_rdo;
                idTarefa = entity.ima_id_tarefa;
                dtInclusao = entity.ima_dt_imagem;
            }
        }
    }
}