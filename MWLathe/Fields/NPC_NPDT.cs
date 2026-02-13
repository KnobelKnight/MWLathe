using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class NPC_NPDT
    {
        public ushort Level { get; set; }
        public byte Disposition { get; set; }
        public byte Reputation { get; set; }
        public byte Rank { get; set; }
        public uint Gold { get; set; }

        // TODO: find a less stupid way to do this
        public virtual uint GetStructSize()
        {
            return 12;
        }

        public virtual void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("NPDT"));
            ts.Write(BitConverter.GetBytes(GetStructSize()));
            ts.Write(BitConverter.GetBytes(Level));
            ts.WriteByte(Disposition);
            ts.WriteByte(Reputation);
            ts.WriteByte(Rank);
            ts.WriteByte(0);
            ts.WriteByte(0);
            ts.WriteByte(0); // Junk bytes
            ts.Write(BitConverter.GetBytes(Gold));
        }
    }
}
