using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class MCDT
    {
        public static readonly uint StructSize = 12;
        public float Weight { get; set; }
        public uint Value { get; set; }
        public uint Junk { get; set; } // 0 or 1. Kept as a field for byte parity

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("MCDT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Weight));
            ts.Write(BitConverter.GetBytes(Value));
            ts.Write(BitConverter.GetBytes(Junk));
        }
    }
}
