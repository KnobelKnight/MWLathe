using System.Text;

namespace MWLathe.Records
{
    public class GMST : Record
    {
        public string NAME { get; set; }
        public float? FLTV { get; set; }
        public int? INTV { get; set; }
        public string? STRV { get; set; }

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
                        NAME = ReadStringField(bs, fieldSize);
                        bytesRead += (int)fieldSize;
                        break;
                    case "FLTV":
                        bytesRead += bs.Read(buffer, 0, 4);
                        FLTV = BitConverter.ToSingle(buffer);
                        break;
                    case "INTV":
                        bytesRead += bs.Read(buffer, 0, 4);
                        INTV = BitConverter.ToInt32(buffer);
                        break;
                    case "STRV":
                        STRV = ReadStringField(bs, fieldSize);
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
            RecordSize += (uint)(8 + NAME.Length);
            if (FLTV.HasValue)
            {
                RecordSize += 12;
            }
            if (INTV.HasValue)
            {
                RecordSize += 12;
            }
            if (STRV is not null)
            {
                RecordSize += (uint)(8 + STRV.Length);
            }
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length));
            ts.Write(Encoding.GetBytes(NAME));
            if (FLTV.HasValue)
            {
                ts.Write(Encoding.GetBytes("FLTV"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(FLTV.Value));
            }
            if (INTV.HasValue)
            {
                ts.Write(Encoding.GetBytes("INTV"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(INTV.Value));
            }
            if (STRV is not null)
            {
                ts.Write(Encoding.GetBytes("STRV"));
                ts.Write(BitConverter.GetBytes(STRV.Length));
                ts.Write(Encoding.GetBytes(STRV));
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
