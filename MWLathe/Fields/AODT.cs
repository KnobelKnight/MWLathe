using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class AODT
    {
        public static readonly uint StructSize = 24;
        public uint Type { get; set; }
        public float Weight { get; set; }
        public uint Value { get; set; }
        public uint Health { get; set; }
        public uint EnchantCapacity { get; set; }
        public uint Rating { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("AODT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Type));
            ts.Write(BitConverter.GetBytes(Weight));
            ts.Write(BitConverter.GetBytes(Value));
            ts.Write(BitConverter.GetBytes(Health));
            ts.Write(BitConverter.GetBytes(EnchantCapacity));
            ts.Write(BitConverter.GetBytes(Rating));
        }
    }
}
