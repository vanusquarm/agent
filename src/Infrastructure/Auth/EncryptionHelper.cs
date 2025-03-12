using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Finacle.Infrastructure.Auth
{
    public static class EncryptionHelper
    {
        private static readonly byte[] EncryptionKey = Convert.FromHexString("a3b1c2d4e5f60789abcd1234567890abcdef1234567890abcdef1234567890ab");

        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = EncryptionKey;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV(); // Always generate a new IV

            var iv = aes.IV; // Store IV to prepend to ciphertext

            using var encryptor = aes.CreateEncryptor(aes.Key, iv);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs, Encoding.UTF8)) // Explicitly use UTF-8 encoding
            {
                sw.Write(plainText);
                sw.Flush(); // Ensure the StreamWriter is flushed
            }

            var encryptedBytes = ms.ToArray();

            // Combine IV and encrypted bytes
            var combinedBytes = new byte[iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, combinedBytes, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, combinedBytes, iv.Length, encryptedBytes.Length);

            return Convert.ToBase64String(combinedBytes);
        }

        public static string Decrypt(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = EncryptionKey;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var iv = new byte[16]; // AES IV is always 16 bytes
            var cipher = new byte[fullCipher.Length - iv.Length];

            // Extract IV and actual encrypted data
            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs, Encoding.UTF8); // Explicitly use UTF-8 encoding


            // Ensure the entire stream is read
            var result = sr.ReadToEnd();

            return result;
        }
    }
}
