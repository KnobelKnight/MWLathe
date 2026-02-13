using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class CELL_DATA
    {
        public static readonly uint StructSize = 12;
        public uint Flags { get; set; }
        public int GridX { get; set; }
        public int GridY { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("DATA"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Flags));
            ts.Write(BitConverter.GetBytes(GridX));
            ts.Write(BitConverter.GetBytes(GridY));
        }
    }
}
