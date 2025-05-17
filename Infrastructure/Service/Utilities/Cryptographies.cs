

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CentralPG.Infrasturcture.Interfaces.Utilities;

namespace CentralPG.Infrastructure.Sevices.Utilities
{
    public class Cryptographies : ICryptoGraphies
    {
        private readonly IConfiguration configuration;

        public Cryptographies(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public byte[] EncryptAES(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                var key = configuration.GetSection("AppUrl:EncryptKey").Value;
                var iv = configuration.GetSection("AppUrl:EncryptIv").Value;
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);
                aesAlg.Mode = CipherMode.CBC;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    return msEncrypt.ToArray();
                }
            }
        }


        public string DecryptAES(byte[] cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                var key = configuration.GetSection("AppUrl:EncryptKey").Value;
                var iv = configuration.GetSection("AppUrl:EncryptIv").Value;
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);
                aesAlg.Mode = CipherMode.CBC;


                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}