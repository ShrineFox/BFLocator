using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BFLocator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("BFLocator by ShrineFox");
            Console.WriteLine("Returns filenames/line names of strings in AtlusFlowScriptExtractorOutput.txt.\n");

            if (args.Length < 2)
            {
                Console.WriteLine("Error: Not enough args!\n\nUsage:\nBFLocator.exe txtPath searchString");
                Console.ReadKey();
                return;
            }

            string txtPath = "";
            if (args.Length > 0)
                txtPath = args[0];
            string searchString = "";
            if (args.Length > 1)
                searchString = args[1];

            Console.WriteLine($"txtPath: {txtPath}\nsearchString: {searchString}\n\n");

            string[] txtLines = File.ReadAllLines(txtPath);
            List<string> newLines = new List<string>();

            if (!File.Exists("BFLocator.txt"))
            {
                File.Delete("BFLocator.txt");
                File.Create("BFLocator.txt");
            }

            for (int i = 0; i < txtLines.Count(); i++)
            {
                if (txtLines[i].Contains(searchString))
                {
                    string fileName = Path.GetFileName(GetName(txtLines, i, out int relativeLineNumber));
                    string newLine = $"\"{searchString}\" found in {fileName} (line {relativeLineNumber + 1})";

                    if (!newLines.Any(x => x.Equals(newLine)))
                    {
                        Console.WriteLine(newLine);
                        newLines.Add(newLine);
                    }
                }
            }

            using (WaitForFile("BFLocator.txt", FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { };
                File.AppendAllLines("BFLocator.txt", newLines.ToArray());
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        public static string GetName(string[] lines, int lineNumber, out int relativeLineNumber)
        {
            for (int x = lineNumber; x > 0; x--)
                if (!lines[x].Contains("[") && lines[x].Contains(".") && lines[x].Contains("\\"))
                {
                    relativeLineNumber = lineNumber - x;
                    return lines[x];
                }

            relativeLineNumber = -1;
            return "";
        }

        public static FileStream WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share)
        {
            for (int numTries = 0; numTries < 10; numTries++)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fullPath, mode, access, share);
                    return fs;
                }
                catch (IOException)
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                    Thread.Sleep(100);
                }
            }

            return null;
        }
    }
}
