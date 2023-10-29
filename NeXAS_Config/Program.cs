using System;
using System.IO;

namespace NeXAS_Config
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("NeXAS_Config");
                Console.WriteLine("Usage:");
                Console.WriteLine("  Extract as JSON : NeXAS_Config -xj [file|folder]");
                Console.WriteLine("  Extract as CSV  : NeXAS_Config -xc [file|folder]");
                Console.WriteLine("  Rebuild         : NeXAS_Config -b  [file|folder]");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            var mode = args[0];
            var path = Path.GetFullPath(args[1]);

            switch (mode)
            {
                case "-xj":
                {
                    if (Directory.Exists(path))
                    {
                        foreach (var it in Directory.GetFiles(path, "*.dat"))
                        {
                            ExtractAsJson(it);
                        }
                    }
                    else
                    {
                        ExtractAsJson(path);
                    }

                    break;
                }
                case "-xc":
                {
                    if (Directory.Exists(path))
                    {
                        foreach (var it in Directory.GetFiles(path, "*.dat"))
                        {
                            ExtractAsCsv(it);
                        }
                    }
                    else
                    {
                        ExtractAsCsv(path);
                    }

                    break;
                }
                case "-b":
                {
                    if (Directory.Exists(path))
                    {
                        foreach (var it in Directory.GetFiles(path, "*.json"))
                        {
                            Rebuild(it);
                        }

                        foreach (var it in Directory.GetFiles(path, "*.csv"))
                        {
                            Rebuild(it);
                        }
                    }
                    else
                    {
                        Rebuild(path);
                    }

                    break;
                }
            }
        }

        static void ExtractAsJson(string filePath)
        {
            Console.WriteLine($"Extract {filePath}");

            try
            {
                var jsonPath = Path.ChangeExtension(filePath, ".json");
                var table = new Table();
                table.Load(filePath);
                table.SaveAsJson(jsonPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void ExtractAsCsv(string filePath)
        {
            Console.WriteLine($"Extract {filePath}");

            try
            {
                var csvPath = Path.ChangeExtension(filePath, ".csv");
                var table = new Table();
                table.Load(filePath);
                table.SaveAsCsv(csvPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Rebuild(string filePath)
        {
            Console.WriteLine($"Rebuild {filePath}");

            try
            {
                var extension = Path.GetExtension(filePath) ?? string.Empty;
                var table = new Table();

                switch (extension.ToLower())
                {
                    case ".json":
                        table.LoadFromJson(filePath);
                        break;
                    case ".csv":
                        table.LoadFromCsv(filePath);
                        break;
                    default:
                        throw new Exception("Extension not supported.");
                }

                var newPath = Path.ChangeExtension(filePath, ".new");
                table.Save(newPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}