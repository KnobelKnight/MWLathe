using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class WEAT
    {
        public static readonly uint StructSize = 10;
        public byte Clear { get; set; }
        public byte Cloudy { get; set; }
        public byte Foggy { get; set; }
        public byte Overcast { get; set; }
        public byte Rain { get; set; }
        public byte Thunder { get; set; }
        public byte Ash { get; set; }
        public byte Blight { get; set; }
        public byte Snow { get; set; }
        public byte Blizzard { get; set; }

        public virtual void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("WEAT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.WriteByte(Clear);
            ts.WriteByte(Cloudy);
            ts.WriteByte(Foggy);
            ts.WriteByte(Overcast);
            ts.WriteByte(Rain);
            ts.WriteByte(Thunder);
            ts.WriteByte(Ash);
            ts.WriteByte(Blight);
            ts.WriteByte(Snow);
            ts.WriteByte(Blizzard);
        }
    }
}
