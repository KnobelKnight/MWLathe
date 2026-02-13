using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class IRDT
    {
        public static readonly uint StructSize = 56;
        public float Weight { get; set; }
        public uint Value { get; set; }
        public int Effect1 { get; set; } = -1;
        public int Effect2 { get; set; } = -1;
        public int Effect3 { get; set; } = -1;
        public int Effect4 { get; set; } = -1;
        public int Skill1 { get; set; } = -1;
        public int Skill2 { get; set; } = -1;
        public int Skill3 { get; set; } = -1;
        public int Skill4 { get; set; } = -1;
        public int Attribute1 { get; set; } = -1;
        public int Attribute2 { get; set; } = -1;
        public int Attribute3 { get; set; } = -1;
        public int Attribute4 { get; set; } = -1;

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("IRDT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Weight));
            ts.Write(BitConverter.GetBytes(Value));
            ts.Write(BitConverter.GetBytes(Effect1));
            ts.Write(BitConverter.GetBytes(Effect2));
            ts.Write(BitConverter.GetBytes(Effect3));
            ts.Write(BitConverter.GetBytes(Effect4));
            ts.Write(BitConverter.GetBytes(Skill1));
            ts.Write(BitConverter.GetBytes(Skill2));
            ts.Write(BitConverter.GetBytes(Skill3));
            ts.Write(BitConverter.GetBytes(Skill4));
            ts.Write(BitConverter.GetBytes(Attribute1));
            ts.Write(BitConverter.GetBytes(Attribute2));
            ts.Write(BitConverter.GetBytes(Attribute3));
            ts.Write(BitConverter.GetBytes(Attribute4));
        }
    }
}
