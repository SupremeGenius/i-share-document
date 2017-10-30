using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISDProject.Controllers
{
    public class FileController : Controller
    {
        /// <summary>
        /// Récupérer le document stocké dans le serveur
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public FileResult Download(string fileName)
        {
            FileResult res = null;
            byte[] fileBytes = GetFile(fileName);
            try
            {
                res = File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(fileName));
            }
            catch (Exception e)
            {

            }
            return res;
        }

        /// <summary>
        /// Convertit un fichier en tableau de byte
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public byte[] GetFile(string s)
        {
            byte[] data = null;
            System.IO.FileStream fs = null;
            try
            {
                fs = System.IO.File.OpenRead(s);
                data = new byte[fs.Length];
                int br = fs.Read(data, 0, data.Length);
                if (br != fs.Length)
                    throw new System.IO.IOException(s);
            }
            catch (Exception e)
            {
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return data;
        }
    }
}
