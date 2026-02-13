using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class PGRD_DATA
    {
        public static readonly uint StructSize = 12;
        public int GridX { get; set; }
        public int GridY { get; set; }
        public ushort Flags { get; set; }
        public ushort PathgridPointCount { get; set; }

        public virtual void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("DATA"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(GridX));
            ts.Write(BitConverter.GetBytes(GridY));
            ts.Write(BitConverter.GetBytes(Flags));
            ts.Write(BitConverter.GetBytes(PathgridPointCount));
        }
    }
}
