using CsvHelper;
using Newtonsoft.Json;
using NeXAS_Script;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace NeXAS_Config
{
    internal class Table
    {
        public enum ValueType
        {
            String = 1,
            Int32,
            Int8,
            Int64,
            Int16
        }

        public class Value
        {
            public ValueType Type { get; set; }
            public string? StringValue { get; set; }
            public long? IntegerValue { get; set; }
        }

        public List<ValueType> Types { get; set; } = new();
        public List<List<Value>> Records { get; set; } = new();

        public void Load(string filePath)
        {
            using var reader = new BinaryReader(File.OpenRead(filePath));

            var count = reader.ReadInt32();

            Types = new List<ValueType>(count);

            for (int i = 0; i < count; i++)
            {
                Types.Add((ValueType)reader.ReadInt32());
            }

            Records = new List<List<Value>>(256);

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var values = new List<Value>(count);

                for (var i = 0; i < count; i++)
                {
                    var v = new Value { Type = Types[i] };

                    switch (Types[i])
                    {
                        case ValueType.String:
                            v.StringValue = reader.ReadNullTerminatedString();
                            break;
                        case ValueType.Int32:
                            v.IntegerValue = reader.ReadInt32();
                            break;
                        case ValueType.Int8:
                            v.IntegerValue = reader.ReadByte();
                            break;
                        case ValueType.Int64:
                            v.IntegerValue = reader.ReadInt64();
                            break;
                        case ValueType.Int16:
                            v.IntegerValue = reader.ReadInt16();
                            break;
                        default:
                            throw new InvalidDataException();
                    }

                    values.Add(v);
                }

                Records.Add(values);
            }

            Debug.Assert(reader.BaseStream.Position == reader.BaseStream.Length);
        }

        public void Save(string filePath)
        {
            using var writer = new BinaryWriter(File.Create(filePath));

            writer.Write(Types.Count);

            foreach (var e in Types)
            {
                writer.Write((int)e);
            }

            foreach (var values in Records)
            {
                foreach (var e in values)
                {
                    switch (e.Type)
                    {
                        case ValueType.String:
                            ArgumentNullException.ThrowIfNull(e.StringValue);
                            writer.WriteNullTerminatedString(e.StringValue);
                            break;
                        case ValueType.Int32:
                            ArgumentNullException.ThrowIfNull(e.IntegerValue);
                            writer.Write((int)e.IntegerValue);
                            break;
                        case ValueType.Int8:
                            ArgumentNullException.ThrowIfNull(e.IntegerValue);
                            writer.Write((byte)e.IntegerValue);
                            break;
                        case ValueType.Int64:
                            ArgumentNullException.ThrowIfNull(e.IntegerValue);
                            writer.Write((long)e.IntegerValue);
                            break;
                        case ValueType.Int16:
                            ArgumentNullException.ThrowIfNull(e.IntegerValue);
                            writer.Write((short)e.IntegerValue);
                            break;
                    }
                }
            }
        }

        public void SaveAsJson(string filePath)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(this, settings);

            File.WriteAllText(filePath, json);
        }

        public void LoadFromJson(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var obj = JsonConvert.DeserializeObject<Table>(json);
            ArgumentNullException.ThrowIfNull(obj);
            Types = obj.Types;
            Records = obj.Records;
        }

        public void SaveAsCsv(string filePath)
        {
            var writer = new CsvWriter(File.CreateText(filePath), CultureInfo.InvariantCulture);

            foreach (var type in Types)
            {
                writer.WriteField(type.ToString());
            }

            writer.NextRecord();

            foreach (var values in Records)
            {
                foreach (var value in values)
                {
                    switch (value.Type)
                    {
                        case ValueType.String:
                            ArgumentNullException.ThrowIfNull(value.StringValue);
                            writer.WriteField(value.StringValue, true);
                            break;
                        case ValueType.Int32:
                        case ValueType.Int8:
                        case ValueType.Int64:
                        case ValueType.Int16:
                            ArgumentNullException.ThrowIfNull(value.IntegerValue);
                            writer.WriteField((long)value.IntegerValue);
                            break;
                    }
                }

                writer.NextRecord();
            }

            writer.Flush();
            writer.Dispose();
        }

        public void LoadFromCsv(string filePath)
        {
            using var reader = new CsvReader(File.OpenText(filePath), CultureInfo.InvariantCulture);

            if (reader.Read() == false || reader.Parser.Count == 0)
            {
                throw new Exception("Missing header row.");
            }

            Types = new List<ValueType>();

            for (var i = 0; i < reader.Parser.Count; i++)
            {
                var s = reader.GetField<string>(i);
                ArgumentNullException.ThrowIfNull(s);
                Types.Add(Enum.Parse<ValueType>(s));
            }

            Records = new List<List<Value>>();

            while (reader.Read())
            {
                var values = new List<Value>(reader.Parser.Count);

                for (var i = 0; i < reader.Parser.Count; i++)
                {
                    var val = new Value { Type = Types[i] };

                    switch (Types[i])
                    {
                        case ValueType.String:
                            val.StringValue = reader.GetField<string>(i);
                            break;
                        case ValueType.Int32:
                        case ValueType.Int8:
                        case ValueType.Int64:
                        case ValueType.Int16:
                            val.IntegerValue = reader.GetField<long>(i);
                            break;
                    }

                    values.Add(val);
                }

                Records.Add(values);
            }

            reader.Dispose();
        }
    }
}
