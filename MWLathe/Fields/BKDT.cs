using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class BKDT
    {
        public static readonly uint StructSize = 20;
        public float Weight { get; set; }
        public uint Value { get; set; }
        public uint Flags { get; set; }
        public int Skill { get; set; } = -1;
        public uint EnchantCapability { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("BKDT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Weight));
            ts.Write(BitConverter.GetBytes(Value));
            ts.Write(BitConverter.GetBytes(Flags));
            ts.Write(BitConverter.GetBytes(Skill));
            ts.Write(BitConverter.GetBytes(EnchantCapability));
        }
    }
}
