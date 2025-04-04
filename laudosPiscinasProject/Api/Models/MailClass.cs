using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Web;

namespace laudosPiscinasProject.Api.Models
{
    public class MailClass
    {
        public static void EnviarAsync(string assunto, string mensagem, string destinatario, string ImagemAssinatura = "", string[] ArquivosAnexo = null)
        {
            MailClass.EnviarAsync(assunto, mensagem, new string[] { destinatario }, ImagemAssinatura, ArquivosAnexo, null, null);
        }

        public static void EnviarAsync(string assunto, string mensagem, string[] destinatarios, string ImagemAssinatura = "", string[] ArquivosAnexo = null, string[] CC = null, string[] CCO = null)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                MailClass.Enviar(assunto, mensagem, destinatarios, ImagemAssinatura, ArquivosAnexo, CC, CCO);
            }).Start();
        }

        public static bool Enviar(string nomeRemetente, string assunto, string mensagem, IEnumerable<string> destinatario, string ImagemAssinatura = "", string[] ArquivosAnexo = null)
        {
            return MailClass.Enviar(assunto, mensagem, destinatario, ImagemAssinatura, ArquivosAnexo, null, null);
        }

        private static string ConfigurationOr(string key, string def = "")
        {
            string conf = ConfigurationManager.AppSettings[key];
            if (conf == null)
            {
                return def;
            }
            return conf;
        }

        /// <summary>
        ///  Definir no Config -> AppSettings: EmailRemetente, NomeRemetente, SenhaRemetente, Host, Port, EnableSsl
        /// </summary>
        public static bool Enviar(string assunto, string mensagem, IEnumerable<string> destinatarios, string ImagemAssinatura = "", IEnumerable<string> ArquivosAnexo = null, IEnumerable<string> CC = null, IEnumerable<string> CCO = null)
        {
            using (SmtpClient smtpClient = new SmtpClient())
            {
                using (MailMessage message = new MailMessage())
                {
                    //NetworkCredential cretential = new NetworkCredential(from.Address, ConfigurationOr("SenhaRemetente"));
                    //smtp.Host = ConfigurationOr("Host", "smtp.gmail.com");
                    //smtp.Port = Convert.ToInt32(ConfigurationOr("Port", "587"));
                    //smtp.EnableSsl = ConfigurationOr("EnableSsl", "true").ToLower() == "true";
                    //smtp.UseDefaultCredentials = false;

                    message.From = new MailAddress(ConfigurationOr("EmailRemetente"), ConfigurationOr("NomeRemetente"));
                    message.To.Add(String.Join(",", destinatarios));
                    message.Subject = assunto;
                    message.SubjectEncoding = System.Text.Encoding.UTF8;
                    message.Body = mensagem;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.IsBodyHtml = true;
                    //smtp.Credentials = cretential;

                    if (!string.IsNullOrEmpty(ImagemAssinatura))
                    {
                        LinkedResource inline;
                        try
                        {
                            inline = new LinkedResource(HttpContext.Current.Server.MapPath(ImagemAssinatura), MediaTypeNames.Image.Jpeg);
                        }
                        catch
                        {
                            inline = new LinkedResource(AppDomain.CurrentDomain.BaseDirectory + "/imagens/logomenor.png", MediaTypeNames.Image.Jpeg);
                        }
                        inline.ContentId = Guid.NewGuid().ToString();
                        mensagem += "<br><img src='cid:" + inline.ContentId + "' />";
                        AlternateView avHtml = AlternateView.CreateAlternateViewFromString(mensagem, null, MediaTypeNames.Text.Html);
                        avHtml.LinkedResources.Add(inline);
                        message.AlternateViews.Add(avHtml);
                    }

                    if (CC != null)
                    {
                        message.CC.Add(String.Join(",", CC));
                    }

                    if (CCO != null)
                    {
                        message.Bcc.Add(String.Join(",", CCO));
                    }

                    if (ArquivosAnexo != null)
                    {
                        ArquivosAnexo.ToList().ForEach(x => message.Attachments.Add(new Attachment(x) { Name = x.Split('/')[x.Split('/').Length - 1] }));
                    }

                    try
                    {
                        smtpClient.Send(message);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        string s = ex.Message;
                        return false;
                    }
                }
            }
        }


    }
}