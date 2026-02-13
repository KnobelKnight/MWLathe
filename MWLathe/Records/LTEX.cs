using System.Text;

namespace MWLathe.Records
{
    public class LTEX : Record
    {
        public string NAME { get; set; }
        public uint INTV { get; set; }
        public string DATA { get; set; }

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
                    case "INTV":
                        bytesRead += bs.Read(buffer, 0, 4);
                        INTV = BitConverter.ToUInt32(buffer);
                        break;
                    case "DATA":
                        DATA = ReadZString(bs);
                        bytesRead += DATA.Length + 1;
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
            RecordSize += (uint)(8 + NAME.Length + 1);
            RecordSize += 12; // INTV
            RecordSize += (uint)(8 + DATA.Length + 1);
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length + 1));
            ts.Write(EncodeZString(NAME));
            ts.Write(Encoding.GetBytes("INTV"));
            ts.Write(BitConverter.GetBytes(4));
            ts.Write(BitConverter.GetBytes(INTV));
            ts.Write(Encoding.GetBytes("DATA"));
            ts.Write(BitConverter.GetBytes(DATA.Length + 1));
            ts.Write(EncodeZString(DATA));
            if (Deleted.HasValue)
            {
                ts.Write(Encoding.GetBytes("DELE"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(Deleted.Value));
            }
        }
    }
}
