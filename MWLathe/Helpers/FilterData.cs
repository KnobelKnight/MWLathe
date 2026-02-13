using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class FilterData
    {
        public char Index { get; set; }
        public char Type { get; set; }
        // Always 2 chars
        public string Details { get; set; } = "00";
        public char Operator { get; set; }
        public string? Name { get; set; }
        public uint? IntValue { get; set; }
        public float? FloatValue { get; set; }

        public uint GetByteSize()
        {
            uint byteSize = 8 + 1 + 1 + 2 + 1;
            if (Name is not null)
            {
                byteSize += (uint)Name.Length;
            }
            if (IntValue.HasValue)
            {
                byteSize += 12;
            }
            if (FloatValue.HasValue)
            {
                byteSize += 12;
            }
            return byteSize;
        }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("SCVR"));
            var byteSize = 5;
            var outputString = string.Concat(Index, Type, Details, Operator);
            if (Name is not null)
            {
                byteSize += Name.Length;
                outputString = outputString + Name;
            }
            ts.Write(BitConverter.GetBytes(byteSize));
            ts.Write(Record.Encoding.GetBytes(outputString)); // NOT a zstring
            if (IntValue.HasValue)
            {
                ts.Write(Record.Encoding.GetBytes("INTV"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(IntValue.Value));
            }
            if (FloatValue.HasValue)
            {
                ts.Write(Record.Encoding.GetBytes("FLTV"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(FloatValue.Value));
            }
        }
    }
}
