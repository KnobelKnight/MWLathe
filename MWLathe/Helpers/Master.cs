using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class Master
    {
        public string Name { get; set; }
        public ulong Size { get; set; }

        public uint GetByteSize()
        {
            return 8 + (uint)Name.Length + 1 + 8 + 8;
        }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("MAST"));
            ts.Write(BitConverter.GetBytes(Name.Length + 1));
            ts.Write(Record.EncodeZString(Name));
            ts.Write(Record.Encoding.GetBytes("DATA"));
            ts.Write(BitConverter.GetBytes(8));
            ts.Write(BitConverter.GetBytes(Size));
        }
    }
}
