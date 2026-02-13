using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class SKDT
    {
        public static readonly uint StructSize = 24;
        public uint Attribute { get; set; }
        public uint Specialization { get; set; }
        public float UseValue1 { get; set; }
        public float UseValue2 { get; set; }
        public float UseValue3 { get; set; }
        public float UseValue4 { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("SKDT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Attribute));
            ts.Write(BitConverter.GetBytes(Specialization));
            ts.Write(BitConverter.GetBytes(UseValue1));
            ts.Write(BitConverter.GetBytes(UseValue2));
            ts.Write(BitConverter.GetBytes(UseValue3));
            ts.Write(BitConverter.GetBytes(UseValue4));
        }
    }
}
