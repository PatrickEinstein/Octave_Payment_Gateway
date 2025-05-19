using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using OCPG.Infrastructure.Interfaces.ICryptographies;

namespace OCPG.Infrastructure
{
    public class FlutterCryptographyCryptography : IFlutterCryptography
    {

        private const string DefaultString = "";

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

    }
}