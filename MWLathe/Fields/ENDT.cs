using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class ENDT
    {
        public static readonly uint StructSize = 16;
        public uint Type { get; set; }
        public uint Cost { get; set; }
        public uint Charge { get; set; }
        public uint Flags { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("ENDT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Type));
            ts.Write(BitConverter.GetBytes(Cost));
            ts.Write(BitConverter.GetBytes(Charge));
            ts.Write(BitConverter.GetBytes(Flags));
        }
    }
}
