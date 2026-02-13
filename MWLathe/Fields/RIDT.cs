using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class RIDT
    {
        public static readonly uint StructSize = 16;
        public float Weight { get; set; }
        public uint Value { get; set; }
        public uint Uses { get; set; }
        public float Quality { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("RIDT"));
            ts.Write(BitConverter.GetBytes(16)); // RIDT struct size
            ts.Write(BitConverter.GetBytes(Weight));
            ts.Write(BitConverter.GetBytes(Value));
            ts.Write(BitConverter.GetBytes(Uses));
            ts.Write(BitConverter.GetBytes(Quality));
        }
    }
}
