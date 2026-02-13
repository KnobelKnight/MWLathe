using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class WPDT
    {
        public static readonly uint StructSize = 32;
        public float Weight { get; set; }
        public uint Value { get; set; }
        public ushort Type { get; set; }
        public ushort Health { get; set; }
        public float Speed { get; set; }
        public float Reach { get; set; }
        public ushort EnchantCapacity { get; set; }
        public byte ChopMin { get; set; }
        public byte ChopMax { get; set; }
        public byte SlashMin { get; set; }
        public byte SlashMax { get; set; }
        public byte ThrustMin { get; set; }
        public byte ThrustMax { get; set; }
        public uint Flags { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("WPDT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Weight));
            ts.Write(BitConverter.GetBytes(Value));
            ts.Write(BitConverter.GetBytes(Type));
            ts.Write(BitConverter.GetBytes(Health));
            ts.Write(BitConverter.GetBytes(Speed));
            ts.Write(BitConverter.GetBytes(Reach));
            ts.Write(BitConverter.GetBytes(EnchantCapacity));
            ts.WriteByte(ChopMin);
            ts.WriteByte(ChopMax);
            ts.WriteByte(SlashMin);
            ts.WriteByte(SlashMax);
            ts.WriteByte(ThrustMin);
            ts.WriteByte(ThrustMax);
            ts.Write(BitConverter.GetBytes(Flags));
        }
    }
}
