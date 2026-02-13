using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class MovedReference
    {
        public uint ID { get; set; }
        public string? CellName { get; set; }
        public (int, int)? GridCoordinates { get; set; }
        public FormReference? FormMoved { get; set; }

        public uint GetByteSize()
        {
            uint byteSize = 12;
            if (CellName is not null)
            {
                byteSize += 8 + (uint)CellName.Length + 1;
            }
            if (GridCoordinates.HasValue)
            {
                byteSize += 8 + 8;
            }
            if (FormMoved is not null)
            {
                byteSize += FormMoved.GetByteSize();
            }
            return byteSize;
        }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("MVRF"));
            ts.Write(BitConverter.GetBytes(4));
            ts.Write(BitConverter.GetBytes(ID));
            if (CellName is not null)
            {
                ts.Write(Record.Encoding.GetBytes("CNAM"));
                ts.Write(BitConverter.GetBytes(CellName.Length + 1));
                ts.Write(Record.EncodeZString(CellName));
            }
            if (GridCoordinates.HasValue)
            {
                ts.Write(Record.Encoding.GetBytes("CNDT"));
                ts.Write(BitConverter.GetBytes(8));
                ts.Write(BitConverter.GetBytes(GridCoordinates.Value.Item1));
                ts.Write(BitConverter.GetBytes(GridCoordinates.Value.Item2));
            }
            if (FormMoved is not null)
            {
                FormMoved.Write(ts);
            }
        }
    }
}
