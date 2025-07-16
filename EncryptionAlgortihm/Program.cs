using System;
using System.Collections.Generic;

namespace EncryptionAlgorithm_ASCII
{
    internal class Program
    {
        static Dictionary<char, char> encryptionMap;

        static void Main()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            encryptionMap = CreateMap(username);

            while (true)
            {
                Console.WriteLine("Choose an operation:");
                Console.WriteLine("1 - Encode to ASCII");
                Console.WriteLine("2 - Decode from ASCII");
                Console.WriteLine("3 - Exit");

                string choice = Console.ReadLine();

                if (choice == "3")
                {
                    Console.WriteLine("Exiting program...");
                    break;
                }

                if (choice == "1")
                {
                    Console.Write("Enter text to encode: ");
                    string input = Console.ReadLine();
                    string encoded = EncodeToAscii(input);
                    Console.WriteLine("ASCII Encoded: " + encoded);
                }
                else if (choice == "2")
                {
                    Console.Write("Enter ASCII values: ");
                    string asciiInput = Console.ReadLine();
                    string decoded = DecodeFromAscii(asciiInput);
                    Console.WriteLine("Decoded Result: " + decoded);
                }
                else
                {
                    Console.WriteLine("Invalid selection.Please try again.");
                }
            }
        }

        static Dictionary<char, char> CreateMap(string key)
        {
            string source = key.ToUpper();
            List<char> letters = new List<char>();

            for (char ch = 'A'; ch <= 'Z'; ch++)
            {
                letters.Add(ch);
            }

            int seed = source.GetHashCode();
            Random rnd = new Random(seed);

            for (int i = letters.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (letters[i], letters[j]) = (letters[j], letters[i]);
            }

            Dictionary<char, char> map = new Dictionary<char, char>();
            for (int i = 0; i < 26; i++)
            {
                map[(char)('A' + i)] = letters[i];
            }

            return map;
        }

        static string EncodeToAscii(string input)
        {
            List<string> asciiList = new List<string>();

            foreach (char ch in input)
            {
                if (char.IsLetter(ch)) 
                {
                    if (char.IsLower(ch)) 
                    {
                        char upperCh = char.ToUpper(ch); 
                        if (encryptionMap.ContainsKey(upperCh)) 
                        {
                            char mappedCh = encryptionMap[upperCh]; 
                            char finalCh = char.ToLower(mappedCh); 
                            asciiList.Add(((int)finalCh).ToString()); 
                        }
                        else 
                        {
                            char finalCh = char.ToLower(upperCh); 
                            asciiList.Add(((int)finalCh).ToString());
                        }
                    }
                    else
                    {
                        if (encryptionMap.ContainsKey(ch)) 
                        {
                            char mappedCh = encryptionMap[ch]; 
                            asciiList.Add(((int)mappedCh).ToString()); 
                        }
                        else 
                        {
                            asciiList.Add(((int)ch).ToString()); 
                        }
                    }
                }
                else 
                {
                    asciiList.Add(((int)ch).ToString());
                }
            }

            return string.Join(" ", asciiList);
        }

        static string DecodeFromAscii(string asciiText)
        {
            string[] asciiValues = asciiText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string result = "";

            foreach (string val in asciiValues)
            {
                if (int.TryParse(val, out int ascii))
                {
                    char ch = (char)ascii;

                    if (char.IsLetter(ch))
                    {
                        bool isLower = char.IsLower(ch);
                        char upperCh = char.ToUpper(ch);
                        char originalCh = DecodeChar(upperCh);
                        if (isLower)
                        {
                            result += char.ToLower(originalCh);
                        }
                        else
                        {
                            result += originalCh;
                        }
                    }
                    else
                    {
                        result += ch;
                    }
                }
                else
                {
                    result += "?"; 
                }
            }

            return result;
        }

        static char DecodeChar(char encodedChar)
        {
            foreach (var number in encryptionMap)
            {
                if (number.Value == encodedChar)
                {
                    return number.Key;
                }
            }

            return encodedChar;
        }
    }
}