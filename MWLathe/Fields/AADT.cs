using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class AADT
    {
        public static readonly uint StructSize = 16;
        public uint Type { get; set; }
        public float Quality { get; set; }
        public float Weight { get; set; }
        public uint Value { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("AADT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Type));
            ts.Write(BitConverter.GetBytes(Quality));
            ts.Write(BitConverter.GetBytes(Weight));
            ts.Write(BitConverter.GetBytes(Value));
        }
    }
}
