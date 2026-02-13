using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class AI_F : AILocationPackage
    {
        public override void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("AI_F"));
            ts.Write(BitConverter.GetBytes(48)); // AI_F struct size
            ts.Write(BitConverter.GetBytes(DestX));
            ts.Write(BitConverter.GetBytes(DestY));
            ts.Write(BitConverter.GetBytes(DestZ));
            ts.Write(BitConverter.GetBytes(Duration));
            ts.Write(Record.EncodeChar32(ID));
            ts.WriteByte(1); // Junk value
            ts.WriteByte(0); // Junk value

            if (Cell is not null)
            {
                ts.Write(Record.Encoding.GetBytes("CNDT"));
                ts.Write(BitConverter.GetBytes(Cell.Length + 1));
                ts.Write(Record.EncodeZString(Cell));
            }
        }
    }
}
