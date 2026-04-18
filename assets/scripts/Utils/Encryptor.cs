using System;
using System.Text;

namespace LOSUtils
{
    public static class Encryptor
    {
        private static string key = "Voxopolis";
        private static string header = "AEXTRIPROT 1.0.0\n";

        public static byte[] Encrypt(string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            var keyBytes = Encoding.UTF8.GetBytes(key);

            for (int i = 0; i < data.Length; i++)
                data[i] ^= keyBytes[i % keyBytes.Length];

            var headerBytes = Encoding.UTF8.GetBytes(header);

            var result = new byte[headerBytes.Length + data.Length];

            Buffer.BlockCopy(headerBytes, 0, result, 0, headerBytes.Length);
            Buffer.BlockCopy(data, 0, result, headerBytes.Length, data.Length);

            return result;
        }

        public static string Decrypt(byte[] bytes)
        {
            var headerBytes = Encoding.UTF8.GetBytes(header);

            if (bytes.Length < headerBytes.Length)
                throw new Exception("Invalid save (too small)");

            for (int i = 0; i < headerBytes.Length; i++)
            {
                if (bytes[i] != headerBytes[i])
                    throw new Exception("Invalid save header");
            }

            int dataLength = bytes.Length - headerBytes.Length;
            var data = new byte[dataLength];

            Buffer.BlockCopy(bytes, headerBytes.Length, data, 0, dataLength);

            var keyBytes = Encoding.UTF8.GetBytes(key);

            for (int i = 0; i < data.Length; i++)
                data[i] ^= keyBytes[i % keyBytes.Length];

            return Encoding.UTF8.GetString(data);
        }
    }
}