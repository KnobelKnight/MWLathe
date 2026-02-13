using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class MEDT
    {
        public static readonly uint StructSize = 36;
        public uint School { get; set; }
        public float BaseCost { get; set; }
        public uint Flags { get; set; }
        public uint Red { get; set; }
        public uint Green { get; set; }
        public uint Blue { get; set; }
        public float Speed { get; set; }
        public float Size { get; set; }
        public float SizeCap { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("MEDT"));
            ts.Write(BitConverter.GetBytes(StructSize)); // Struct size
            ts.Write(BitConverter.GetBytes(School));
            ts.Write(BitConverter.GetBytes(BaseCost));
            ts.Write(BitConverter.GetBytes(Flags));
            ts.Write(BitConverter.GetBytes(Red));
            ts.Write(BitConverter.GetBytes(Green));
            ts.Write(BitConverter.GetBytes(Blue));
            ts.Write(BitConverter.GetBytes(Speed));
            ts.Write(BitConverter.GetBytes(Size));
            ts.Write(BitConverter.GetBytes(SizeCap));
        }
    }
}
