using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ThaoPhuong.Models;

namespace ThaoPhuong.Utils
{
    public class DbUtils
    {
        public static string EncrytPass(string password)
        {
            return GetMd5(GetMd5(password + "1998") + "1998");
        }

        private static string GetMd5(string password)
        {
            string rs = "";
            using (var md5Hash = MD5.Create())
            {
                // Byte array representation of source string
                var sourceBytes = Encoding.UTF8.GetBytes(password);

                // Generate hash value(Byte Array) for input data
                var hashBytes = md5Hash.ComputeHash(sourceBytes);

                // Convert hash byte array to string
                rs = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }
            return rs;
        }

        public static string GenCode(string startCode, string table, string field)
        {
            THAOPHUONGEntities db = new THAOPHUONGEntities();
            //lấy số thứ tự
            List<string> lst = db.Database.SqlQuery<string>("SELECT "+ field + " FROM "+ table + " WHERE "+ field + " LIKE '" + startCode + "%'").ToList();
            int max = 0;
            string temp = "";
            foreach (string item in lst)
            {
                int value = 0;
                temp = item.Replace(startCode, "");
                value = Convert.ToInt32(temp);
                max = value > max ? value : max;
            }
            max = max + 1;
            int maxLen = 6;
            temp = "";
            while (temp.Length + max.ToString().Length < maxLen)
            {
                temp += "0";
            }
            temp += max.ToString();
            startCode += temp;
            return startCode;
        }

        public static string NumberToText(decimal? v)
        {
            decimal rs = 0;
            rs = v ?? 0;
            return rs == 0 ? "0" : rs.ToString("###,###");
        }
    }
}