using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class AIDT
    {
        public static readonly uint StructSize = 12;
        public byte Hello { get; set; }
        public byte Junk { get; set; } // 0 or 1. Kept as a field for byte parity
        public byte Fight { get; set; }
        public byte Flee { get; set; }
        public byte Alarm { get; set; }
        public uint Flags { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("AIDT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.WriteByte(Hello);
            ts.WriteByte(Junk);
            ts.WriteByte(Fight);
            ts.WriteByte(Flee);
            ts.WriteByte(Alarm);
            ts.WriteByte(0);
            ts.WriteByte(0);
            ts.WriteByte(0); // Junk values
            ts.Write(BitConverter.GetBytes(Flags));
        }
    }
}
