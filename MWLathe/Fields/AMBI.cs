using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class AMBI
    {
        public static readonly uint StructSize = 16;
        public RGB AmbientColor { get; set; } = new RGB();
        public RGB SunColor { get; set; } = new RGB();
        public RGB FogColor { get; set; } = new RGB();
        public float FogDensity { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("AMBI"));
            ts.Write(BitConverter.GetBytes(StructSize));
            AmbientColor.Write(ts, true);
            SunColor.Write(ts, true);
            FogColor.Write(ts, true);
            ts.Write(BitConverter.GetBytes(FogDensity));
        }
    }
}
