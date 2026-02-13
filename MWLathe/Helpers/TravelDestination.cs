using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class TravelDestination
    {
        public DODT Location { get; set; } = new DODT();
        public string? Cell { get; set; }

        public uint GetByteSize()
        {
            uint byteSize = 8 + DODT.StructSize;
            if (Cell != null)
            {
                byteSize += 8 + (uint)Cell.Length + 1;
            }
            return byteSize;
        }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("DODT"));
            ts.Write(BitConverter.GetBytes(DODT.StructSize));
            ts.Write(BitConverter.GetBytes(Location.PositionX));
            ts.Write(BitConverter.GetBytes(Location.PositionY));
            ts.Write(BitConverter.GetBytes(Location.PositionZ));
            ts.Write(BitConverter.GetBytes(Location.RotationX));
            ts.Write(BitConverter.GetBytes(Location.RotationY));
            ts.Write(BitConverter.GetBytes(Location.RotationZ));
            if (Cell is not null)
            {
                ts.Write(Record.Encoding.GetBytes("DNAM"));
                ts.Write(BitConverter.GetBytes(Cell.Length + 1));
                ts.Write(Record.EncodeZString(Cell));
            }
        }
    }
}
