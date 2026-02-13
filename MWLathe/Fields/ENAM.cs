using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class ENAM
    {
        public static readonly uint StructSize = 24;
        public ushort Index { get; set; }
        public sbyte Skill { get; set; } = -1;
        public sbyte Attribute { get; set; } = -1;
        public uint Range { get; set; }
        public uint Area { get; set; }
        public uint Duration { get; set; }
        public uint MagnitudeMax { get; set; }
        public uint MagnitudeMin { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("ENAM"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Index));
            ts.WriteByte((byte)Skill);
            ts.WriteByte((byte)Attribute);
            ts.Write(BitConverter.GetBytes(Range));
            ts.Write(BitConverter.GetBytes(Area));
            ts.Write(BitConverter.GetBytes(Duration));
            ts.Write(BitConverter.GetBytes(MagnitudeMin));
            ts.Write(BitConverter.GetBytes(MagnitudeMax));
        }
    }
}
