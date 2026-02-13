using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class INFO_DATA
    {
        public static readonly uint StructSize = 12;
        public byte Type { get; set; }
        public uint DispOrJournal { get; set; }
        public sbyte Rank { get; set; } = -1;
        public sbyte Gender { get; set; } = -1;
        public sbyte PCRank { get; set; } = -1;

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("DATA"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.WriteByte(Type);
            ts.WriteByte(0);
            ts.WriteByte(0);
            ts.WriteByte(0); // Junk values
            ts.Write(BitConverter.GetBytes(DispOrJournal));
            ts.WriteByte((byte)Rank);
            ts.WriteByte((byte)Gender);
            ts.WriteByte((byte)PCRank);
            ts.WriteByte(0); // Junk value
        }
    }
}
