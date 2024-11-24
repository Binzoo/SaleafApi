using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SeleafAPI.Helper;

public class EncryptionHelper
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("KDJFIOLoIU897YUHJNBGER3IOLJU89ER"); 
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("OLP098UHJKiONGRE"); 

    public static string EncryptString(int plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        // Write the plain integer as a string
                        swEncrypt.Write(plainText.ToString());
                    }
                }

                // Return Base64 string
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public static string DecryptString(string cipherText)
    {
        try
        {
            Console.WriteLine($"CipherText Input: {cipherText}");
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(buffer))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            string decryptedValue = srDecrypt.ReadToEnd();
                            Console.WriteLine($"Decrypted Value: {decryptedValue}");
                            return decryptedValue;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Decryption Error: {ex.Message}");
            throw;
        }
    }


    
    
}