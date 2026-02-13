using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class ALDT
    {
        public static readonly uint StructSize = 12;
        public float Weight { get; set; }
        public uint Value { get; set; }
        public uint Flags { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("ALDT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Weight));
            ts.Write(BitConverter.GetBytes(Value));
            ts.Write(BitConverter.GetBytes(Flags));
        }
    }
}
