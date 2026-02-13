using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class NPC_NPDT_EXT : NPC_NPDT
    {
        public byte Attribute1 { get; set; }
        public byte Attribute2 { get; set; }
        public byte Attribute3 { get; set; }
        public byte Attribute4 { get; set; }
        public byte Attribute5 { get; set; }
        public byte Attribute6 { get; set; }
        public byte Attribute7 { get; set; }
        public byte Attribute8 { get; set; }
        public byte[] Skills { get; set; } = new byte[27];
        public ushort Health { get; set; }
        public ushort Magicka { get; set; }
        public ushort Fatigue { get; set; }

        public override uint GetStructSize()
        {
            return 52;
        }

        public override void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("NPDT"));
            ts.Write(BitConverter.GetBytes(GetStructSize()));
            ts.Write(BitConverter.GetBytes(Level));
            ts.WriteByte(Attribute1);
            ts.WriteByte(Attribute2);
            ts.WriteByte(Attribute3);
            ts.WriteByte(Attribute4);
            ts.WriteByte(Attribute5);
            ts.WriteByte(Attribute6);
            ts.WriteByte(Attribute7);
            ts.WriteByte(Attribute8);
            foreach (var skill in Skills[..27])
            {
                ts.WriteByte(skill);
            }
            ts.WriteByte(0); // Junk byte
            ts.Write(BitConverter.GetBytes(Health));
            ts.Write(BitConverter.GetBytes(Magicka));
            ts.Write(BitConverter.GetBytes(Fatigue));
            ts.WriteByte(Disposition);
            ts.WriteByte(Reputation);
            ts.WriteByte(Rank);
            ts.WriteByte(0); // Junk byte
            ts.Write(BitConverter.GetBytes(Gold));
        }
    }
}
