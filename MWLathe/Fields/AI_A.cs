using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class AI_A : AIPackage
    {
        public string ID { get; set; }

        public override uint GetByteSize()
        {
            return 8 + 33;
        }

        public override void UpdateID(string oldID, string newID)
        {
            ID = ReplaceID(ID, oldID, newID);
        }

        public override void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("AI_A"));
            ts.Write(BitConverter.GetBytes(33));
            ts.Write(Record.EncodeChar32(ID));
            ts.WriteByte(0); // Junk value
        }
    }
}
