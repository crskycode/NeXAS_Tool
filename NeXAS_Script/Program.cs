using System;
using System.IO;

namespace NeXAS_Script
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("NeXAS_Script");
                Console.WriteLine("Usage:");
                Console.WriteLine("  Extract : NeXAS_Script -x [file|folder]");
                Console.WriteLine("  Rebuild : NeXAS_Script -b [file|folder]");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            var mode = args[0];
            var path = Path.GetFullPath(args[1]);

            switch (mode)
            {
                case "-x":
                {
                    if (Directory.Exists(path))
                    {
                        foreach (var it in Directory.GetFiles(path, "*.bin"))
                        {
                            Extract(it);
                        }
                    }
                    else
                    {
                        Extract(path);
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
                    }
                    else
                    {
                        Rebuild(path);
                    }

                    break;
                }
            }
        }

        static void Extract(string filePath)
        {
            Console.WriteLine($"Extract {filePath}");

            try
            {
                var script = new Script();
                script.Load(filePath);
                var jsonPath = Path.ChangeExtension(filePath, ".json");
                Script.SaveToJsonFile(jsonPath, script);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void Rebuild(string filePath)
        {
            Console.WriteLine($"Rebuild {filePath}");

            try
            {
                var script = Script.LoadFromJsonFile(filePath);
                var newPath = Path.ChangeExtension(filePath, ".new");
                script.Save(newPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}