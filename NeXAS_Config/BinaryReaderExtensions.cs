using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NeXAS_Script
{
    internal static class BinaryReaderExtensions
    {
        internal static string ReadNullTerminatedString(this BinaryReader reader, Encoding encoding)
        {
            var buffer = new List<byte>(256);

            for (var c = reader.ReadByte(); c != 0; c = reader.ReadByte())
            {
                buffer.Add(c);
            }

            if (buffer.Count > 0)
            {
                return encoding.GetString(buffer.ToArray());
            }

            return string.Empty;
        }

        internal static string ReadNullTerminatedString(this BinaryReader reader)
        {
            return ReadNullTerminatedString(reader, Encoding.UTF8);
        }
    }
}
