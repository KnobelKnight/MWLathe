using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class CTDT
    {
        public static readonly uint StructSize = 12;
        public uint Type { get; set; }
        public float Weight { get; set; }
        public ushort Value { get; set; }
        public ushort EnchantCapacity { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("CTDT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Type));
            ts.Write(BitConverter.GetBytes(Weight));
            ts.Write(BitConverter.GetBytes(Value));
            ts.Write(BitConverter.GetBytes(EnchantCapacity));
        }
    }
}
