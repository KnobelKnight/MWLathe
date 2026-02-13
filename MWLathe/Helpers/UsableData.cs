using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class UsableData
    {
        public static readonly uint StructSize = 16;
        public float Weight { get; set; }
        public uint Value { get; set; }
        public float Quality { get; set; }
        public uint Uses { get; set; }

        public void Write(FileStream ts, string fieldName)
        {
            ts.Write(Record.Encoding.GetBytes(fieldName));
            ts.Write(BitConverter.GetBytes(16)); // Struct size
            ts.Write(BitConverter.GetBytes(Weight));
            ts.Write(BitConverter.GetBytes(Value));
            ts.Write(BitConverter.GetBytes(Quality));
            ts.Write(BitConverter.GetBytes(Uses));
        }
    }
}
