using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
//aici am incercat alta metoda de criptare
namespace SolutionCodeCraftersByBT
{
    public static class EncryptionHelper
    {
        static readonly string KeySalt = "asdad@!#!@#ADasD!@#@!#@!#!@#";
        static readonly string initializeatinvocatorIVKey = "HR$2pIjHR$2pIj12";
        public static string Encrypt(string plainText, string secretKey) {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            var rijandelAlgo = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            byte[] keyBytes = new Rfc2898DeriveBytes(secretKey, Encoding.ASCII.GetBytes(KeySalt)).GetBytes(rijandelAlgo.KeySize);

            var encryptor = rijandelAlgo.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(initializeatinvocatorIVKey));
            byte[] cipherTextBytes;
            using (var memoryStream = new MemoryStream()) {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }
        public static string Decrypt(string encryptedText, string secretKey) {

            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);

            var rijanAlgo = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };
            byte[] keyBytes = new Rfc2898DeriveBytes(secretKey, Encoding.ASCII.GetBytes(KeySalt)).GetBytes(rijanAlgo.KeySize);

            var decryptor = rijanAlgo.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(initializeatinvocatorIVKey));
            var memoryStream = new MemoryStream(cipherTextBytes);

            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());

        }
    }
}
