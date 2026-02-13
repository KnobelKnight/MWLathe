using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class SPDT
    {
        public static readonly uint StructSize = 12;
        public uint Type { get; set; }
        public uint Cost { get; set; }
        public uint Flags { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("SPDT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Type));
            ts.Write(BitConverter.GetBytes(Cost));
            ts.Write(BitConverter.GetBytes(Flags));
        }
    }
}
