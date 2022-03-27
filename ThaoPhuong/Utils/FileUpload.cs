using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ThaoPhuong.Utils
{
    public class FileUpload
    {
        public static string Upload(HttpServerUtilityBase server, HttpPostedFileBase file)
        {
            string tempFolder = Path.Combine(server.MapPath("~/Images/Upload/"));
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            string temp = file.FileName;
            string[] options = temp.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            string fileName = Guid.NewGuid().ToString();
            if (options.Length > 1)
            {
                fileName = fileName + "." + options[options.Length - 1];
            }
            else
            {
                fileName = fileName + ".jpg";
            }

            string path = Path.Combine(server.MapPath("~/Images/Upload/" + fileName));

            if (File.Exists(path)) File.Delete(path);

            file.SaveAs(path);

            return fileName;
        }

        public static void Delete(HttpServerUtilityBase server, string fileName)
        {
            string path = Path.Combine(server.MapPath("~/Images/Upload/" + fileName));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}