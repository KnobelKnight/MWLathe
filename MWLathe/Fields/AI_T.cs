using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class AI_T : AIPackage
    {
        public float DestX { get; set; }
        public float DestY { get; set; }
        public float DestZ { get; set; }

        public override void UpdateID(string oldID, string newID)
        {

        }

        public override uint GetByteSize()
        {
            return 8 + 16;
        }

        public override void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("AI_T"));
            ts.Write(BitConverter.GetBytes(16));
            ts.Write(BitConverter.GetBytes(DestX));
            ts.Write(BitConverter.GetBytes(DestY));
            ts.Write(BitConverter.GetBytes(DestZ));
            ts.WriteByte(1);
            ts.WriteByte(0);
            ts.WriteByte(0);
            ts.WriteByte(0); // Junk values
        }
    }
}
