using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using UnityEngine;

namespace KitchenKrapper
{
    public class SecureDataStorage : MonoBehaviour
    {
        private const string secretKey = "zCbkbhMr5bRcDx75IPYgOcV1R8ZtrSD5Z8";
        private const string fileName = "data.dat";

        public static void SaveData<T>(T data) where T : class
        {
            // Serialize the data to a byte array
            byte[] bytes = ObjectToByteArray(data);

            // Encrypt the byte array using AES
            byte[] encryptedBytes = Encrypt(bytes, secretKey);

            // Write the encrypted data to a file
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllBytes(filePath, encryptedBytes);
        }

        public static T LoadData<T>(T defaultValue = default) where T : class
        {
            // Read the encrypted data from a file, if it exists
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            if (!File.Exists(filePath)) return defaultValue; // If the file doesn't exist, return the default value
            byte[] encryptedBytes = File.ReadAllBytes(filePath);

            // Decrypt the byte array using AES
            byte[] bytes = Decrypt(encryptedBytes, secretKey);

            // Deserialize the byte array to an object
            T data = ByteArrayToObject<T>(bytes);

            return data ?? defaultValue;
        }


        private static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private static T ByteArrayToObject<T>(byte[] bytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }

        private static byte[] Encrypt(byte[] data, string key)
        {
            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.Mode = CipherMode.CBC;
            rijndael.Padding = PaddingMode.PKCS7;
            rijndael.KeySize = 256;
            rijndael.BlockSize = 128;

            byte[] keyBytes = new byte[32];
            byte[] secretKeyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            rijndael.Key = keyBytes;

            byte[] ivBytes = new byte[16];
            Array.Copy(keyBytes, ivBytes, Math.Min(ivBytes.Length, keyBytes.Length));
            rijndael.IV = ivBytes;

            ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }

        private static byte[] Decrypt(byte[] data, string key)
        {
            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.Mode = CipherMode.CBC;
            rijndael.Padding = PaddingMode.PKCS7;
            rijndael.KeySize = 256;
            rijndael.BlockSize = 128;

            byte[] keyBytes = new byte[32];
            byte[] secretKeyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            rijndael.Key = keyBytes;

            byte[] ivBytes = new byte[16];
            Array.Copy(keyBytes, ivBytes, Math.Min(ivBytes.Length, keyBytes.Length));
            rijndael.IV = ivBytes;

            ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);
            return decryptor.TransformFinalBlock(data, 0, data.Length);
        }
    }
}