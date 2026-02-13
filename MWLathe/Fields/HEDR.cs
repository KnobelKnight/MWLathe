using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class HEDR
    {
        public static readonly uint StructSize = 300;
        public float Version { get; set; }
        public uint Flags { get; set; }
        // Max 32 chars
        public string Developer { get; set; }
        // Max 256 chars
        public string Description { get; set; }
        public uint TotalRecords { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("HEDR"));
            ts.Write(BitConverter.GetBytes(StructSize)); // HEDR struct is 300 bytes
            ts.Write(BitConverter.GetBytes(Version));
            ts.Write(BitConverter.GetBytes(Flags));
            ts.Write(Record.Encoding.GetBytes(Developer));
            ts.Write(Record.Encoding.GetBytes(Description));
            ts.Write(BitConverter.GetBytes(TotalRecords));
        }
    }
}
