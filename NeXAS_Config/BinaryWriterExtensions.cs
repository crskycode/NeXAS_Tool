using System.IO;
using System.Text;

namespace NeXAS_Script
{
    internal static class BinaryWriterExtensions
    {
        internal static void WriteNullTerminatedString(this BinaryWriter writer, string value, Encoding encoding)
        {
            var bytes = encoding.GetBytes(value);
            writer.Write(bytes);
            writer.Write((byte)0);
        }

        internal static void WriteNullTerminatedString(this BinaryWriter writer, string value)
        {
            WriteNullTerminatedString(writer, value, Encoding.UTF8);
        }
    }
}
