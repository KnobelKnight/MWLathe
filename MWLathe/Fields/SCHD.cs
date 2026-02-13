using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class SCHD
    {
        public static readonly uint StructSize = 52;
        public string Name { get; set; }
        public uint ShortCount { get; set; }
        public uint LongCount { get; set; }
        public uint FloatCount { get; set; }
        public uint ScriptSize { get; set; }
        public uint LocalSize { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("SCHD"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(Record.EncodeChar32(Name));
            ts.Write(BitConverter.GetBytes(ShortCount));
            ts.Write(BitConverter.GetBytes(LongCount));
            ts.Write(BitConverter.GetBytes(FloatCount));
            ts.Write(BitConverter.GetBytes(ScriptSize));
            ts.Write(BitConverter.GetBytes(LocalSize));
        }
    }
}
