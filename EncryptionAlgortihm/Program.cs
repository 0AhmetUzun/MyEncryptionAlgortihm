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
                Console.WriteLine("3 - Exit");

                string choice = Console.ReadLine();

                if (choice == "3")
                {
                    Console.WriteLine("Exitting...");
                    break;
                }

                if (choice == "1")
                {
                    Console.Write("Text to encode: ");
                    string input = Console.ReadLine();
                    string encoded = Encode(input);
                    Console.WriteLine("Encoded result: " + encoded);
                }
                else if (choice == "2")
                {
                    Console.Write("Text to decode: ");
                    string hexInput = Console.ReadLine();
                    string decoded = Decode(hexInput);
                    Console.WriteLine("Decoded result: " + decoded);
                }
                else
                {
                    Console.WriteLine("Invalid option.Please try again");
                }
            }
        }

        static Dictionary<char, char> CreateMap(string key)
        {
            string source = key.ToUpper();
            string alphabet = "ABCÇDEFGĞHIİJKLMNOÖPRSŞTUÜVYZ";

            List<char> letters = new List<char>(alphabet);
            int seed = source.GetHashCode();
            Random rnd = new Random(seed);
            
            //Fisher Yates Shuffle
            for (int i = letters.Count - 1; i > 0; i--)  
            {
                int j = rnd.Next(i + 1);
                (letters[i], letters[j]) = (letters[j], letters[i]);
            }

            Dictionary<char, char> map = new Dictionary<char, char>();
            for (int i = 0; i < alphabet.Length; i++)
            {
                map[alphabet[i]] = letters[i];
            }

            return map;
        }

        static string Encode(string input)
        {
            List<string> hexList = new List<string>();

            foreach (char ch in input)
            {
                char mappedChar = ch;
                char upperCh = char.ToUpper(ch);

                if (encryptionMap.ContainsKey(upperCh))
                {
                    mappedChar = encryptionMap[upperCh];
                    if (char.IsLower(ch))
                        mappedChar = char.ToLower(mappedChar);
                }

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
            string result = "";

            foreach (char ch in decoded)
            {
                char upperCh = char.ToUpper(ch);
                bool isLower = char.IsLower(ch);
                char original;

                if (encryptionMap.ContainsValue(upperCh))
                {
                    original = DecodeChar(upperCh);
                }
                else
                {
                    original = ch;
                }

                if (isLower)
                {
                    result += char.ToLower(original);
                }
                else
                {
                    result += original;
                }
            }

            return result;
        }

        static char DecodeChar(char encodedChar)
        {
            foreach (var kvp in encryptionMap)
            {
                if (kvp.Value == encodedChar)
                    return kvp.Key;
            }

            return encodedChar;
        }
    }
}