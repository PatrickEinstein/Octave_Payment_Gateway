using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CentralPG.Infrasturcture.Interfaces.Utilities
{
    public interface ICryptoGraphies
    {
        byte[] EncryptAES(string plainText);
        string DecryptAES(byte[] cipherText);
    }
}