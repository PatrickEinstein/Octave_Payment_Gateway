using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using OCPG.Infrastructure.Interfaces.ICryptographies;
using OCPG.Core.Models;
using System.IO;

namespace OCPG.Infrastructure
{
    public class FlutterCryptographyCryptography : IFlutterCryptography
    {
        public FlutterCryptographyCryptography(CryptographyConfig authConfig)
        {
            this.authConfig = authConfig;
        }

        private const string DefaultString = "";
        private readonly CryptographyConfig authConfig;

        public string EncryptFlutter3DESAlgo(string data, string encryptionKey)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(encryptionKey))
                return DefaultString;

            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);

                // Ensure the key is exactly 24 bytes
                if (keyBytes.Length < 24)
                {
                    Array.Resize(ref keyBytes, 24);
                }
                else if (keyBytes.Length > 24)
                {
                    Array.Resize(ref keyBytes, 24);
                }

                using (var tripleDes = TripleDES.Create())
                {
                    tripleDes.Key = keyBytes;
                    tripleDes.Mode = CipherMode.ECB;
                    tripleDes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform encryptor = tripleDes.CreateEncryptor();
                    byte[] inputBytes = Encoding.UTF8.GetBytes(data);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

                    return Convert.ToBase64String(encryptedBytes);
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string DecryptFlutter3DESAlgo(string encryptedData, string encryptionKey)
        {
            const string defaultString = "";

            if (string.IsNullOrEmpty(encryptedData) || string.IsNullOrEmpty(encryptionKey))
                return defaultString;

            try
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);

                // Ensure the key is exactly 24 bytes
                if (keyBytes.Length < 24)
                {
                    Array.Resize(ref keyBytes, 24);
                }
                else if (keyBytes.Length > 24)
                {
                    Array.Resize(ref keyBytes, 24);
                }

                using (var tripleDes = TripleDES.Create())
                {
                    tripleDes.Key = keyBytes;
                    tripleDes.Mode = CipherMode.ECB;
                    tripleDes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = tripleDes.CreateDecryptor();
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public byte[] EncryptAES(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                var key = authConfig.generalEncryptionKey;
                var iv = authConfig.generalEncryptionIV;
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
                var key = authConfig.generalEncryptionKey;
                var iv = authConfig.generalEncryptionIV;
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