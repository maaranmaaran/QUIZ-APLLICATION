using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using System.Web;

namespace quiz.Models
{
    public class cyptop
    {
        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingReader = new AppSettingsReader();
            string key = (string)settingReader.GetValue("SecurityKey", typeof(String));
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
                return string.Empty;
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTranform = tdes.CreateEncryptor();
                byte[] resultArray = cTranform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);

            }


            

        }
    }
}