using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class LHDT
    {
        public static readonly uint StructSize = 24;
        public float Weight { get; set; }
        public uint Value { get; set; }
        public int Time { get; set; }
        public uint Radius { get; set; }
        public RGB Color { get; set; } = new RGB();
        public uint Flags { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("LHDT"));
            ts.Write(BitConverter.GetBytes(StructSize)); // LHDT struct size
            ts.Write(BitConverter.GetBytes(Weight));
            ts.Write(BitConverter.GetBytes(Value));
            ts.Write(BitConverter.GetBytes(Time));
            ts.Write(BitConverter.GetBytes(Radius));
            Color.Write(ts, true);
            ts.Write(BitConverter.GetBytes(Flags));
        }
    }
}
