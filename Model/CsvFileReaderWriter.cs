using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ins_Assignment_3.Model
{
    public class CsvFileReaderWriter
    {
        public CsvFileReaderWriter()
        {
        }

        public static char Delimiter { get; set; } = ','; //Comma by default
        public static char TextQualifier { get; set; } = '"'; //Quote by default

        /// Reads the contents of a file into a string
        public string Read(string filePath)
        {
            using (StreamReader stream = new StreamReader(filePath))
            {
                return stream.ReadToEnd();
            }
        }

        /// Reads the contents of a file into a List of strings
        public List<string> ParseFile(string filePath)
        {
            string fileContent = Read(filePath);

            return ParseString(fileContent);
        }

        /// Reads the contents of a file into a List of strings
        public List<string> ParseString(string delimitedText)
        {
            return delimitedText.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// Reads the contents of a file into a List of tokenized string arrays
        public List<string[]> GetEntities(string filePath)
        {
            List<string> entries = ParseFile(filePath);

            List<string[]> entities = new List<string[]>();

            foreach (var entry in entries)
            {
                entities.Add(CSVEntityParser(entry));
            }

            return entities;
        }

        /// Reads the contents of a file into a List of tokenized string arrays
        public List<string[]> GetEntities(List<string> entries)
        {
            List<string[]> entities = new List<string[]>();

            foreach (var entry in entries)
            {
                entities.Add(CSVEntityParser(entry));
            }

            return entities;
        }

        public string[] CSVEntityParser(string delimitedText)
        {
            List<string> tokens = new List<string>();

            bool isInText = false;
            int lastChar = -1;
            int currentChar = 0;

            while (currentChar < delimitedText.Length)
            {
                if (delimitedText[currentChar] == TextQualifier)
                {
                    isInText = !isInText;
                }
                else if (delimitedText[currentChar] == Delimiter)
                {
                    if (!isInText)
                    {
                        tokens.Add(delimitedText.Substring(lastChar + 1, (currentChar - lastChar)).Trim(' ', Delimiter));
                        lastChar = currentChar;
                    }
                }
                currentChar++;
            }

            if (lastChar != delimitedText.Length - 1)
            {
                tokens.Add(delimitedText.Substring(lastChar + 1).Trim());
            }

            return tokens.ToArray();
        }

        /// Writes a List of string array to a file delimited by a delimiter
        public void WriteFile(string filePath, List<string[]> entities)
        {
            StreamWriter sw;

            //Decide which stream to write to Console or File
            if (string.IsNullOrEmpty(filePath))
            {
                //Write to the console if the file path is not specified
                sw = new StreamWriter(Console.OpenStandardOutput());
                sw.AutoFlush = true;
                Console.SetOut(sw);
            }
            else
            {
                //Write to a file if the file path is valid
                sw = new StreamWriter(filePath);
            }

            //Iterate over entries and join the array by the Delimiter
            foreach (var entity in entities)
            {
                sw.WriteLine(string.Join(Delimiter, entity));
            }

            //Close the stream
            sw.Close();
        }
    }
}