using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace laudosPiscinasProject.Api.Models
{
    public class Seguranca
    {
        public static string EncryptTripleDES(string sIn, string sKey = "KEYTOPOS")
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();

            //  scramble the key
            sKey = ScrambleKey(sKey);
            //  Compute the MD5 hash.
            DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(sKey));
            //  Set the cipher mode.
            DES.Mode = CipherMode.ECB;
            //  Create the encryptor.
            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            //  Get a byte array of the string.
            byte[] Buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(sIn);
            //  Transform and return the string.
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        public static string DecryptTripleDES(string sOut, string sKey = "KEYTOPOS")
        {
            sOut = sOut.Replace(" ", "+");

            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();

            MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();

            //  scramble the key
            sKey = ScrambleKey(sKey);
            //  Compute the MD5 hash.
            DES.Key = hashMD5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(sKey));
            //  Set the cipher mode.
            DES.Mode = CipherMode.ECB;
            //  Create the decryptor.
            ICryptoTransform DESDecrypt = DES.CreateDecryptor();
            byte[] Buffer = Convert.FromBase64String(sOut);
            //  Transform and return the string.
            return System.Text.ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }

        private static string ScrambleKey(string v_strKey)
        {
            System.Text.StringBuilder sbKey = new System.Text.StringBuilder();
            int intPtr;
            for (intPtr = 1; (intPtr <= v_strKey.Length); intPtr++)
            {
                int intIn = ((v_strKey.Length - intPtr)
                            + 1);
                sbKey.Append(v_strKey.Substring((intIn - 1), 1));
            }
            string strKey = sbKey.ToString();
            return sbKey.ToString();
        }

    }
}