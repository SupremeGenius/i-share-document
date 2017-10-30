using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ISDProject.Tools
{
    public class Tools
    {
        public static String GetFileSizeString(int valeur)
        {
            String[] unit = { "octets", "K", "M", "G", "T" };
            String str = "";
            int[] detail = new int[5];
            int i = 0;
            do
            {
                detail[i] = valeur % 1024;
                valeur /= 1024;
                i++;
            } while (valeur >= 1024 && i < 5);
            detail[i] = valeur;
            for (int k = detail.Length - 1; k >= 0; k--)
            {
                if (detail[k] != 0)
                {
                    str += (detail[k] + " " + unit[k] + " ");
                }
            }
            return str;
        }

        public static int GetFileSize(String url)
        {
            FileInfo f = new FileInfo(url);
            long size = -1;
            try
            {
                size = f.Length;
            }
            catch (Exception e)
            {
                // log.Error("Erreur lors de la lecture du fichier ", e);
                return -1;
            }
            return (int)size;
        }

        public static String GetFileNameFromURL(String url)
        {
            FileInfo ife = new FileInfo(url);
            return ife.Name;
        }

        public static void CreateDirectory(String url)
        {
            if (!Directory.Exists(url))
            {
                System.IO.Directory.CreateDirectory(url);
            }
        }

        public static void DeleteFile(String url)
        {
            if (System.IO.File.Exists(url))
            {
                try
                {
                    System.IO.File.Delete(url);
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }

        public static void SendMessage(String toMail, String userName, String message, String link, String file)
        {
            System.IO.StreamReader mailTemplate = new System.IO.StreamReader(System.Web.HttpContext.Current.Server.MapPath("../Content/config/template-mail.xml"));
            string body = mailTemplate.ReadToEnd();
            mailTemplate.Close();
            //
            DateTime now = DateTime.Now;
            body = body.Replace("[file]", file);
            body = body.Replace("[user]", userName);
            body = body.Replace("[link]", link);
            body = body.Replace("[message]", message);
            body = body.Replace("[year]", now.Year.ToString());

            String objet = userName + " shared "+file+" with you !";
            // Command line argument must the the SMTP host.
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("isharedocument@gmail.com", "isharedocument1");

            MailMessage mm = new MailMessage("donotreply@domain.com", toMail, objet, body);
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.IsBodyHtml = true;

            client.Send(mm);
        }
    }
}