using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotahold.Core.Utils
{
    public static class MD5Utils
    {
        public static string StringMD5(string OriString)
        {
            try
            {
                if (OriString.Length == 0)
                {
                    return "";
                }
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] ret = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(OriString));

                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < ret.Length; ++i)
                {
                    sBuilder.Append(ret[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
            catch { }
            return OriString;
        }
    }
}
