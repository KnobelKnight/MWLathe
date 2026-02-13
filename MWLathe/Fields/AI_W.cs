using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class AI_W : AIPackage
    {
        public ushort Distance { get; set; }
        public ushort Duration { get; set; }
        public byte Time { get; set; }
        // Idle1 does not exist
        public byte Idle2 { get; set; }
        public byte Idle3 { get; set; }
        public byte Idle4 { get; set; }
        public byte Idle5 { get; set; }
        public byte Idle6 { get; set; }
        public byte Idle7 { get; set; }
        public byte Idle8 { get; set; }
        public byte Idle9 { get; set; }

        public override void UpdateID(string oldID, string newID)
        {

        }

        public override uint GetByteSize()
        {
            return 8 + 14;
        }

        public override void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("AI_W"));
            ts.Write(BitConverter.GetBytes(14));
            ts.Write(BitConverter.GetBytes(Distance));
            ts.Write(BitConverter.GetBytes(Duration));
            ts.WriteByte(Time);
            ts.WriteByte(Idle2);
            ts.WriteByte(Idle3);
            ts.WriteByte(Idle4);
            ts.WriteByte(Idle5);
            ts.WriteByte(Idle6);
            ts.WriteByte(Idle7);
            ts.WriteByte(Idle8);
            ts.WriteByte(Idle9);
            ts.WriteByte(1); // Junk value
        }
    }
}
