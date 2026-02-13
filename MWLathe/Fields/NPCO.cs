using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class NPCO
    {
        public static readonly uint StructSize = 36;
        public int Count { get; set; }
        public string ID { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("NPCO"));
            ts.Write(BitConverter.GetBytes(36)); // NPCO struct size
            ts.Write(BitConverter.GetBytes(Count));
            ts.Write(Record.EncodeChar32(ID));
        }
    }
}
