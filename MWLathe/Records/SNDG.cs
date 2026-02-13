using System.Text;

namespace MWLathe.Records
{
    public class SNDG : Record
    {
        public string NAME { get; set; }
        public uint DATA { get; set; }
        public string? CNAM { get; set; }
        public string? SNAM { get; set; }

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
                    case "NAME":
                        NAME = ReadZString(bs);
                        bytesRead += NAME.Length + 1;
                        break;
                    case "DATA":
                        bytesRead += bs.Read(buffer, 0, 4);
                        DATA = BitConverter.ToUInt32(buffer);
                        break;
                    case "CNAM":
                        CNAM = ReadZString(bs);
                        bytesRead += CNAM.Length + 1;
                        break;
                    case "SNAM":
                        SNAM = ReadZString(bs);
                        bytesRead += SNAM.Length + 1;
                        break;
                    default:
                        throw new Exception($"Unknown {GetType().Name} field \"{fieldType}\"");
                }
            }
        }

        public override void UpdateID(string oldID, string newID)
        {
            CNAM = ReplaceID(CNAM, oldID, newID);
            SNAM = ReplaceID(SNAM, oldID, newID);
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + NAME.Length + 1);
            RecordSize += 12; // DATA
            if (CNAM is not null)
            {
                RecordSize += (uint)(8 + CNAM.Length + 1);
            }
            if (SNAM is not null)
            {
                RecordSize += (uint)(8 + SNAM.Length + 1);
            }
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length + 1));
            ts.Write(EncodeZString(NAME));
            ts.Write(Encoding.GetBytes("DATA"));
            ts.Write(BitConverter.GetBytes(4));
            ts.Write(BitConverter.GetBytes(DATA));
            if (CNAM is not null)
            {
                ts.Write(Encoding.GetBytes("CNAM"));
                ts.Write(BitConverter.GetBytes(CNAM.Length + 1));
                ts.Write(EncodeZString(CNAM));
            }
            if (SNAM is not null)
            {
                ts.Write(Encoding.GetBytes("SNAM"));
                ts.Write(BitConverter.GetBytes(SNAM.Length + 1));
                ts.Write(EncodeZString(SNAM));
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
