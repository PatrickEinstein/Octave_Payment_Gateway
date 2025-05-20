using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCPG.Infrastructure.Interfaces.ICryptographies
{
    public interface IFlutterCryptography
    {
        string EncryptFlutter3DESAlgo(string data, string encryptionKey);
        string DecryptFlutter3DESAlgo(string encryptedData, string encryptionKey);
        byte[] EncryptAES(string plainText);
        string DecryptAES(byte[] cipherText);
    }
}