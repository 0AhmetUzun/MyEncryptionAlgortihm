using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EncryptionAlgorithm
{
    internal class Program
    {
        static Dictionary<char, char> encryptionMap;

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            encryptionMap = CreateMap(username);

            while (true)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1 - Encode");
                Console.WriteLine("2 - Decode");
                Console.WriteLine("3 - Change Username");
                Console.WriteLine("4 - Exit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Text to encode: ");
                        string input = Console.ReadLine();
                        Console.WriteLine("Encoded result: " + Encode(input));
                        break;

                    case "2":
                        Console.Write("Text to decode: ");
                        string hexInput = Console.ReadLine();
                        Console.WriteLine("Decoded result: " + Decode(hexInput));
                        break;

                    case "3":
                        Console.Write("Enter new username: ");
                        string newUsername = Console.ReadLine();
                        encryptionMap = CreateMap(newUsername);
                        Console.WriteLine("Map updated with new username.");
                        break;

                    case "4":
                        Console.WriteLine("Exiting...");
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static Dictionary<char, char> CreateMap(string key)
        {
            string seedKey = key.ToUpperInvariant();

            string baseChars =
                "ABCÇDEFGĞHIİJKLMNOÖPRSŞTUÜVYZ" +    
                "abcçdefgğhiıijklmnoöprsştuüvyz" +    
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +       
                "abcdefghijklmnopqrstuvwxyz" +       
                "0123456789" +                        
                "!@#$%^&*()_-+=[]{}|:;'<>,.?/~`\\\""; 

            List<char> originalSet = new List<char>();
            foreach (char ch in baseChars)
                if (!originalSet.Contains(ch))
                    originalSet.Add(ch);

            List<char> shuffledSet = new List<char>(originalSet);

            Random rnd = new Random(seedKey.GetHashCode());
            for (int i = shuffledSet.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (shuffledSet[i], shuffledSet[j]) = (shuffledSet[j], shuffledSet[i]);
            }

            Dictionary<char, char> map = new Dictionary<char, char>();
            for (int i = 0; i < originalSet.Count; i++)
            {
                map[originalSet[i]] = shuffledSet[i];
            }

            return map;
        }

        static string Encode(string input)
        {
            List<string> hexList = new List<string>();

            foreach (char ch in input)
            {
                char mappedChar = encryptionMap.ContainsKey(ch) ? encryptionMap[ch] : ch;
                byte[] bytes = Encoding.UTF8.GetBytes(mappedChar.ToString());
                foreach (byte b in bytes)
                {
                    hexList.Add(b.ToString("X2"));
                }
            }

            return string.Join("", hexList);
        }

        static string Decode(string hexText)
        {
            List<byte> byteList = new List<byte>();

            for (int i = 0; i < hexText.Length; i += 2)
            {
                string hexPair = hexText.Substring(i, 2);
                if (byte.TryParse(hexPair, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b))
                {
                    byteList.Add(b);
                }
            }

            string decoded = Encoding.UTF8.GetString(byteList.ToArray());
            StringBuilder result = new StringBuilder();

            foreach (char ch in decoded)
            {
                char original = DecodeChar(ch);
                result.Append(original);
            }

            return result.ToString();
        }

        static char DecodeChar(char encodedChar)
        {
            foreach (var kv in encryptionMap)
            {
                if (kv.Value == encodedChar)
                    return kv.Key;
            }
            return encodedChar;
        }
    }
}