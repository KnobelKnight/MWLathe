using System.Text;

namespace MWLathe.Records
{
    public class SSCR : Record
    {
        public string DATA { get; set; }
        public string? NAME { get; set; }

        public override void Populate(BufferedStream bs)
        {
            base.Populate(bs);
            byte[] buffer = new byte[256];
            int bytesRead = 0;
            while (bytesRead < RecordSize)
            {
                bytesRead += bs.Read(buffer, 0, 8);
                var fieldType = Encoding.GetString(buffer, 0, 4);
                var fieldSize = BitConverter.ToUInt32(buffer, 4);
                switch (fieldType)
                {
                    case "DELE":
                        bytesRead += bs.Read(buffer, 0, 4);
                        Deleted = BitConverter.ToUInt32(buffer);
                        break;
                    case "DATA":
                        DATA = ReadStringField(bs, fieldSize);
                        bytesRead += (int)fieldSize;
                        break;
                    case "NAME":
                        NAME = ReadStringField(bs, fieldSize);
                        bytesRead += (int)fieldSize;
                        break;
                    default:
                        throw new Exception($"Unknown {GetType().Name} field \"{fieldType}\"");
                }
            }
        }

        public override void UpdateID(string oldID, string newID)
        {
            NAME = ReplaceID(NAME, oldID, newID);
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + DATA.Length);
            if (NAME is not null)
            {
                RecordSize += (uint)(8 + NAME.Length);
            }
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetBytes("DATA"));
            ts.Write(BitConverter.GetBytes(DATA.Length));
            ts.Write(Encoding.GetBytes(DATA));
            if (NAME is not null)
            {
                ts.Write(Encoding.GetBytes("NAME"));
                ts.Write(BitConverter.GetBytes(NAME.Length));
                ts.Write(Encoding.GetBytes(NAME));
            }
            if (Deleted.HasValue)
            {
                ts.Write(Encoding.GetBytes("DELE"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(Deleted.Value));
            }
        }
    }
}
