using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NeXAS_Script
{
    internal class Script
    {
        public class Function
        {
            public int Id { get; set; }
            public List<int[]> UnknowArray1 { get; set; } = new();
            public List<int[]> Opcodes { get; set; } = new();
            public List<string> ConstantStrings { get; set; } = new();
            public List<string> LocalVariableDeclarations { get; set; } = new();
            public List<string> ParameterDeclarations { get; set; } = new();
            public List<byte[]> UnknowBlock { get; set; } = new();
        }


        public List<int> UnknowArray1 { get; set; } = new();
        public List<int[]> UnknowArray2 { get; set; } = new();
        public List<int[]> Opcodes { get; set; } = new();
        public List<string> ConstantStrings { get; set; } = new();
        public List<string> VariableDeclarations { get; set; } = new();
        public List<string> ParametersDeclarations { get; set; } = new();
        public List<byte[]> UnknowBlock { get; set; } = new();
        public List<Function> Functions { get; set; } = new();


        public static Script LoadFromJsonFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var script = JsonConvert.DeserializeObject<Script>(json);
            ArgumentNullException.ThrowIfNull(script);
            return script;
        }

        public static void SaveToJsonFile(string filePath, Script script)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                Formatting = Formatting.Indented,
            };

            var json = JsonConvert.SerializeObject(script, settings);

            File.WriteAllText(filePath, json);
        }

        public void Load(string filePath)
        {
            using var reader = new BinaryReader(File.OpenRead(filePath));

            var signature = reader.ReadBytes(9);

            if (signature.Length == 9 && Encoding.ASCII.GetString(signature, 0, 8) != "VER-1.00")
            {
                throw new Exception("Not a valid script file.");
            }

            ReadUnknowArray1(reader);
            ReadUnknowArray2(reader);
            ReadOpcodes(reader);
            ReadConstantStrings(reader);
            ReadVariableDeclarations(reader);
            ReadParameterDeclarations(reader);
            ReadUnknowBlock(reader);
            ReadFunctions(reader);

            if (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                throw new Exception("The file has not been read yet.");
            }
        }

        public void Save(string filePath)
        {
            using var writer = new BinaryWriter(File.Create(filePath));

            writer.WriteNullTerminatedString("VER-1.00");

            WriteUnknowArray1(writer);
            WriteUnknowArray2(writer);
            WriteOpcodes(writer);
            WriteConstantStrings(writer);
            WriteVariableDeclarations(writer);
            WriteParameterDeclarations(writer);
            WriteUnknowBlock(writer);
            WriteFunctions(writer);

            writer.Flush();
        }

        private void ReadUnknowArray1(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            UnknowArray1 = new List<int>(count);

            for (var i = 0; i < count; i++)
            {
                UnknowArray1.Add(reader.ReadInt32());
            }
        }

        private void ReadUnknowArray2(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            UnknowArray2 = new List<int[]>(count);

            for (var i = 0; i < count; i++)
            {
                var v1 = reader.ReadInt32();
                var v2 = reader.ReadInt32();

                UnknowArray2.Add(new int[] { v1, v2 });
            }
        }

        private void ReadOpcodes(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            Opcodes = new List<int[]>(count);

            for (var i = 0; i < count; i++)
            {
                var cmd = reader.ReadInt32();
                var arg = reader.ReadInt32();

                Opcodes.Add(new int[] { cmd, arg });
            }
        }

        private void ReadConstantStrings(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            ConstantStrings = new List<string>(count);

            for (int i = 0; i < count; i++)
            {
                ConstantStrings.Add(reader.ReadNullTerminatedString());
            }
        }

        private void ReadVariableDeclarations(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            VariableDeclarations = new List<string>(count);

            for (var i = 0; i < count; i++)
            {
                VariableDeclarations.Add(reader.ReadNullTerminatedString());
            }
        }

        private void ReadParameterDeclarations(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            ParametersDeclarations = new List<string>(count);

            for (var i = 0; i < count; i++)
            {
                ParametersDeclarations.Add(reader.ReadNullTerminatedString());
            }
        }

        private void ReadUnknowBlock(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            UnknowBlock = new List<byte[]>(count);

            for (int i = 0; i < count; i++)
            {
                UnknowBlock.Add(reader.ReadBytes(68));
            }
        }

        private void ReadFunctions(BinaryReader reader)
        {
            Functions = new List<Function>(256);

            while (reader.BaseStream.Length - reader.BaseStream.Position > 4)
            {
                var func = new Function();

                var id = reader.ReadInt32();
                func.Id = id;

                // unknow data #1
                {
                    var count = reader.ReadInt32();

                    for (var i = 0; i < count; i++)
                    {
                        var v1 = reader.ReadInt32();
                        var v2 = reader.ReadInt32();

                        func.UnknowArray1.Add(new int[] { v1, v2 });
                    }
                }

                // opcode
                {
                    var count = reader.ReadInt32();

                    for (var i = 0; i < count; i++)
                    {
                        var cmd = reader.ReadInt32();
                        var arg = reader.ReadInt32();

                        func.Opcodes.Add(new int[] { cmd, arg });
                    }
                }

                // constant string
                {
                    var count = reader.ReadInt32();

                    for (var i = 0; i < count; i++)
                    {
                        func.ConstantStrings.Add(reader.ReadNullTerminatedString());
                    }
                }

                // local variable declaration
                {
                    var count = reader.ReadInt32();

                    for (var i = 0; i < count; i++)
                    {
                        func.LocalVariableDeclarations.Add(reader.ReadNullTerminatedString());
                    }
                }

                // parameter declaration
                {
                    var count = reader.ReadInt32();

                    for (var i = 0; i < count; i++)
                    {
                        func.ParameterDeclarations.Add(reader.ReadNullTerminatedString());
                    }
                }

                // unknow array #2
                {
                    var count = reader.ReadInt32();

                    for (var i = 0; i < count; i++)
                    {
                        func.UnknowBlock.Add(reader.ReadBytes(68));
                    }
                }

                Functions.Add(func);
            }
        }

        private void WriteUnknowArray1(BinaryWriter writer)
        {
            writer.Write(UnknowArray1.Count);

            foreach (var e in UnknowArray1)
            {
                writer.Write(e);
            }
        }

        private void WriteUnknowArray2(BinaryWriter writer)
        {
            writer.Write(UnknowArray2.Count);

            foreach (var e in UnknowArray2)
            {
                writer.Write(e[0]);
                writer.Write(e[1]);
            }
        }

        private void WriteOpcodes(BinaryWriter writer)
        {
            writer.Write(Opcodes.Count);

            foreach (var e in Opcodes)
            {
                writer.Write(e[0]);
                writer.Write(e[1]);
            }
        }

        private void WriteConstantStrings(BinaryWriter writer)
        {
            writer.Write(ConstantStrings.Count);

            foreach (var e in ConstantStrings)
            {
                writer.WriteNullTerminatedString(e);
            }
        }

        private void WriteVariableDeclarations(BinaryWriter writer)
        {
            writer.Write(VariableDeclarations.Count);

            foreach (var e in VariableDeclarations)
            {
                writer.WriteNullTerminatedString(e);
            }
        }

        private void WriteParameterDeclarations(BinaryWriter writer)
        {
            writer.Write(ParametersDeclarations.Count);

            foreach (var e in ParametersDeclarations)
            {
                writer.WriteNullTerminatedString(e);
            }
        }

        private void WriteUnknowBlock(BinaryWriter writer)
        {
            writer.Write(UnknowBlock.Count);

            foreach (var e in UnknowBlock)
            {
                writer.Write(e);
            }
        }

        private void WriteFunctions(BinaryWriter writer)
        {
            foreach (var func in Functions)
            {
                writer.Write(func.Id);

                // unknow data #1
                {
                    writer.Write(func.UnknowArray1.Count);

                    foreach (var e in func.UnknowArray1)
                    {
                        writer.Write(e[0]);
                        writer.Write(e[1]);
                    }
                }

                // opcode
                {
                    writer.Write(func.Opcodes.Count);

                    foreach (var e in func.Opcodes)
                    {
                        writer.Write(e[0]);
                        writer.Write(e[1]);
                    }
                }

                // constant string
                {
                    writer.Write(func.ConstantStrings.Count);

                    foreach (var e in func.ConstantStrings)
                    {
                        writer.WriteNullTerminatedString(e);
                    }
                }

                // local variable declaration
                {
                    writer.Write(func.LocalVariableDeclarations.Count);

                    foreach (var e in func.LocalVariableDeclarations)
                    {
                        writer.WriteNullTerminatedString(e);
                    }
                }

                // parameter declaration
                {
                    writer.Write(func.ParameterDeclarations.Count);

                    foreach (var e in func.ParameterDeclarations)
                    {
                        writer.WriteNullTerminatedString(e);
                    }
                }

                // unknow array #2
                {
                    writer.Write(func.UnknowBlock.Count);

                    foreach (var e in func.UnknowBlock)
                    {
                        writer.Write(e);
                    }
                }
            }
        }
    }
}
