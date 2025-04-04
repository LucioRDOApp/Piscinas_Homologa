using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using static laudosPiscinasProject.Api.Models.TarefaModel;

namespace laudosPiscinasProject.Api.Models
{
    public class UtilsModel
    {
        internal static string SalvarImagem(dynamic arquivo, string pastaDestino)
        {
            string PastaDeImagens = ConfigurationManager.AppSettings["PastaDeImagens"] ?? "~/Uploads/Imagens/";
            PastaDeImagens = System.Web.Hosting.HostingEnvironment.MapPath(PastaDeImagens);
            if (!Directory.Exists(PastaDeImagens)) { Directory.CreateDirectory(PastaDeImagens); }

            string PrimeiraParteNomeDoArquivo = string.Concat(pastaDestino, "/");
            if (!Directory.Exists(string.Concat(PastaDeImagens, PrimeiraParteNomeDoArquivo))) { Directory.CreateDirectory(string.Concat(PastaDeImagens, PrimeiraParteNomeDoArquivo)); }

            string result = "";

            if (arquivo != null)
            {
                var imageFile = new ImageFile
                {
                    filetype = arquivo.filetype,
                    filename = arquivo.filename,
                    filesize = arquivo.filesize,
                    base64 = arquivo.base64
                };

                result = imageFile.filename;
                string NomeDoArquivo = string.Concat(PrimeiraParteNomeDoArquivo, result);
                File.WriteAllBytes(string.Concat(PastaDeImagens, NomeDoArquivo), Convert.FromBase64String(imageFile.base64));
            }

            return result;
        }
        public static string UploadImage(string base64, string folder)
        {
            if (!base64.StartsWith("data:image"))
            {
                return base64.Substring(base64.ToUpper().IndexOf("/UPLOADS"));
            }

            base64 = base64.Replace("data:image/png;base64,", "").Replace("data:image/jpeg;base64,", "").Replace("data:image/bmp;base64,", "");
            byte[] Imagembytes = Convert.FromBase64String(base64);
            var caminhoRelativo = "";
            if (!string.IsNullOrEmpty(folder))
            {
                folder = "/Uploads/" + folder;
                if (Directory.Exists(HostingEnvironment.ApplicationPhysicalPath + folder))
                {
                    Directory.Delete(HostingEnvironment.ApplicationPhysicalPath + folder, true);
                }
            }
            Directory.CreateDirectory(HostingEnvironment.ApplicationPhysicalPath + folder);

            caminhoRelativo = folder + Guid.NewGuid().ToString("N").ToUpper() + ".jpeg";

            var caminhoAbsoluto = HostingEnvironment.ApplicationPhysicalPath + caminhoRelativo;
            using (MemoryStream ms = new MemoryStream(Imagembytes))
            {
                using (Bitmap bm2 = new Bitmap(ms))
                {
                    bm2.Save(caminhoAbsoluto);
                }
            }

            return caminhoRelativo;
        }
        public static string UploadImage(string base64)
        {
            if (!base64.StartsWith("data:image"))
            {
                return base64.Substring(base64.ToUpper().IndexOf("/UPLOADS"));
            }

            base64 = base64.Replace("data:image/png;base64,", "").Replace("data:image/jpeg;base64,", "");
            byte[] Imagembytes = Convert.FromBase64String(base64);
            var caminhoRelativo = "/uploads/" + DateTime.Now.ToString("ddMMyyyyHHmmssffffff") + ".jpeg";
            var caminhoAbsoluto = HostingEnvironment.ApplicationPhysicalPath + caminhoRelativo;
            using (MemoryStream ms = new MemoryStream(Imagembytes))
            {
                using (Bitmap bm2 = new Bitmap(ms))
                {
                    bm2.Save(caminhoAbsoluto);
                }
            }

            return caminhoRelativo;
        }
    }
}