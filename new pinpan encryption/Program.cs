using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        // Data
        string track2Data = ";5351290102107506=21112011557206710000?";
        string pin = "1234";
        string key = "ED2307743BAFC53FA0315C89116BCABF";
        string pinblock = CalculatePinBlock(track2Data, pin);
        string panblock = CalculatePanBlock(track2Data, pin);
        string pinpanblock = CalculatePinPanBlock(pinblock, panblock);
        byte[] dataBytes = HexStringToByteArray(pinpanblock);
        byte[] keyBytes = HexStringToByteArray(key);

        // Performing 3DES encryption
        byte[] encryptedData = Encrypt3DES(dataBytes, keyBytes);

        // Convert the encrypted byte array to a hexadecimal string
        string encryptedHex = ByteArrayToHexString(encryptedData);

        // Print the encrypted result
        Console.WriteLine("The Pinblock is:" + pinblock);
        Console.WriteLine("The Panblock is:" + panblock);
        Console.WriteLine("The Non-Encrypted PinPan Block is:" + pinpanblock);
        Console.WriteLine("Encrypted Data: " + encryptedHex);
        Console.ReadKey();

    }
    static string CalculatePinBlock(string track2Data, string pin)
    {
        string pinblock = "";
        int len = pin.Length;

        if (len == 4)
        {
            pinblock = "0" + len + pin;
            pinblock = pinblock.PadRight(16, 'F');
            return pinblock;
        }
        else if (len >= 10)
        {
            char asciiChar = (char)('A' + (len - 10)); // Calculate the corresponding ASCII character.
            pinblock = "0" + asciiChar + pin.PadRight(15, 'F');
            return pinblock;
        }
        return pinblock;

    }

    
    static string CalculatePanBlock(string track2Data, string pin)
    {
        string panblock = track2Data.Substring(4, 12);
        panblock = panblock.PadLeft(16, '0');
        return panblock;
    }
    static string CalculatePinPanBlock(string hexString1, string hexString2)
    {
        if (hexString1.Length != hexString2.Length)
            throw new ArgumentException("Input strings must have the same length.");

        char[] result = new char[hexString1.Length];
        for (int i = 0; i < hexString1.Length; i++)
        {
            int value1 = Convert.ToInt32(hexString1[i].ToString(), 16);
            int value2 = Convert.ToInt32(hexString2[i].ToString(), 16);
            int xorResult = value1 ^ value2;
            result[i] = Convert.ToChar(xorResult.ToString("X"));
        }

        return new string(result);
    }

    static byte[] HexStringToByteArray(string hex)
    {
        int byteCount = hex.Length / 2;
        byte[] bytes = new byte[byteCount];
        for (int i = 0; i < byteCount; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return bytes;
    }
    // ConvertING a byte array to a hexadecimal string
    static string ByteArrayToHexString(byte[] bytes)
    {
        StringBuilder hex = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            hex.AppendFormat("{0:X2}", b);
        }
        return hex.ToString();
    }
    static byte[] Encrypt3DES(byte[] data, byte[] key)
    {
        using (TripleDESCryptoServiceProvider desProvider = new TripleDESCryptoServiceProvider())
        {
            // Set the encryption mode and padding
            desProvider.Mode = CipherMode.ECB;
            desProvider.Padding = PaddingMode.None;

            // Set the provided key
            desProvider.Key = key;

            // Create the encryptor
            ICryptoTransform encryptor = desProvider.CreateEncryptor();

            // Encrypt the data
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }
    }
}