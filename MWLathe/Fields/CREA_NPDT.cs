using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class CREA_NPDT
    {
        public static readonly uint StructSize = 96;
        public uint Type { get; set; }
        public uint Level { get; set; }
        public uint Attribute1 { get; set; }
        public uint Attribute2 { get; set; }
        public uint Attribute3 { get; set; }
        public uint Attribute4 { get; set; }
        public uint Attribute5 { get; set; }
        public uint Attribute6 { get; set; }
        public uint Attribute7 { get; set; }
        public uint Attribute8 { get; set; }
        public uint Health { get; set; }
        public uint Magicka { get; set; }
        public uint Fatigue { get; set; }
        public uint SoulSize { get; set; }
        public uint Combat { get; set; }
        public uint Magic { get; set; }
        public uint Stealth { get; set; }
        public uint Attack1Min { get; set; }
        public uint Attack1Max { get; set; }
        public uint Attack2Min { get; set; }
        public uint Attack2Max { get; set; }
        public uint Attack3Min { get; set; }
        public uint Attack3Max { get; set; }
        public uint Gold { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("NPDT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Type));
            ts.Write(BitConverter.GetBytes(Level));
            ts.Write(BitConverter.GetBytes(Attribute1));
            ts.Write(BitConverter.GetBytes(Attribute2));
            ts.Write(BitConverter.GetBytes(Attribute3));
            ts.Write(BitConverter.GetBytes(Attribute4));
            ts.Write(BitConverter.GetBytes(Attribute5));
            ts.Write(BitConverter.GetBytes(Attribute6));
            ts.Write(BitConverter.GetBytes(Attribute7));
            ts.Write(BitConverter.GetBytes(Attribute8));
            ts.Write(BitConverter.GetBytes(Health));
            ts.Write(BitConverter.GetBytes(Magicka));
            ts.Write(BitConverter.GetBytes(Fatigue));
            ts.Write(BitConverter.GetBytes(SoulSize));
            ts.Write(BitConverter.GetBytes(Combat));
            ts.Write(BitConverter.GetBytes(Magic));
            ts.Write(BitConverter.GetBytes(Stealth));
            ts.Write(BitConverter.GetBytes(Attack1Min));
            ts.Write(BitConverter.GetBytes(Attack1Max));
            ts.Write(BitConverter.GetBytes(Attack2Min));
            ts.Write(BitConverter.GetBytes(Attack2Max));
            ts.Write(BitConverter.GetBytes(Attack3Min));
            ts.Write(BitConverter.GetBytes(Attack3Max));
            ts.Write(BitConverter.GetBytes(Gold));
        }
    }
}
