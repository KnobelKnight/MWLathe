using MWLathe.Helpers;
using System.Text;
using System.Text.RegularExpressions;

namespace MWLathe.Records
{
    public class BOOK : Record
    {
        public static bool replaceBookText = false;
        public string NAME { get; set; }
        public string? MODL { get; set; }
        public string? FNAM { get; set; }
        public BKDT? BKDT { get; set; }
        public string? SCRI { get; set; }
        public string? ITEX { get; set; }
        public string? TEXT { get; set; }
        public string? ENAM { get; set; }

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
                    case "MODL":
                        MODL = ReadZString(bs);
                        bytesRead += MODL.Length + 1;
                        break;
                    case "FNAM":
                        FNAM = ReadZString(bs);
                        bytesRead += FNAM.Length + 1;
                        break;
                    case "BKDT":
                        BKDT = new BKDT();
                        bytesRead += bs.Read(buffer, 0, 20);
                        BKDT.Weight = BitConverter.ToSingle(buffer);
                        BKDT.Value = BitConverter.ToUInt32(buffer, 4);
                        BKDT.Flags = BitConverter.ToUInt32(buffer, 8);
                        BKDT.Skill = BitConverter.ToInt32(buffer, 12);
                        BKDT.EnchantCapability = BitConverter.ToUInt32(buffer, 16);
                        break;
                    case "SCRI":
                        SCRI = ReadZString(bs);
                        bytesRead += SCRI.Length + 1;
                        break;
                    case "ITEX":
                        ITEX = ReadZString(bs);
                        bytesRead += ITEX.Length + 1;
                        break;
                    case "TEXT":
                        TEXT = ReadStringField(bs, fieldSize);
                        bytesRead += (int)fieldSize;
                        break;
                    case "ENAM":
                        ENAM = ReadZString(bs);
                        bytesRead += ENAM.Length + 1;
                        break;
                    default:
                        throw new Exception($"Unknown {GetType().Name} field \"{fieldType}\"");
                }
            }
        }

        public override void UpdateID(string oldID, string newID)
        {
            NAME = ReplaceID(NAME, oldID, newID);
            SCRI = ReplaceID(SCRI, oldID, newID);
            // TODO: set Updated here
            if (replaceBookText)
            {
                if (TEXT is not null)
                {
                    TEXT = Regex.Replace(TEXT, $"""(?:^|(?<=\W)|(?<=\\t)|(?<=(\\r\\n))){oldID}\b""", newID, RegexOptions.IgnoreCase);
                }
            }
            ENAM = ReplaceID(ENAM, oldID, newID);
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + NAME.Length + 1);
            if (MODL is not null)
            {
                RecordSize += (uint)(8 + MODL.Length + 1);
            }
            if (FNAM is not null)
            {
                RecordSize += (uint)(8 + FNAM.Length + 1);
            }
            if (BKDT is not null)
            {
                RecordSize += BKDT.StructSize + 8;
            }
            if (SCRI is not null)
            {
                RecordSize += (uint)(8 + SCRI.Length + 1);
            }
            if (ITEX is not null)
            {
                RecordSize += (uint)(8 + ITEX.Length + 1);
            }
            if (TEXT is not null)
            {
                RecordSize += (uint)(8 + TEXT.Length);
            }
            if (ENAM is not null)
            {
                RecordSize += (uint)(8 + ENAM.Length + 1);
            }
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length + 1));
            ts.Write(EncodeZString(NAME));
            if (MODL is not null)
            {
                ts.Write(Encoding.GetBytes("MODL"));
                ts.Write(BitConverter.GetBytes(MODL.Length + 1));
                ts.Write(EncodeZString(MODL));
            }
            if (FNAM is not null)
            {
                ts.Write(Encoding.GetBytes("FNAM"));
                ts.Write(BitConverter.GetBytes(FNAM.Length + 1));
                ts.Write(EncodeZString(FNAM));
            }
            if (BKDT is not null)
            {
                BKDT.Write(ts);
            }
            if (SCRI is not null)
            {
                ts.Write(Encoding.GetBytes("SCRI"));
                ts.Write(BitConverter.GetBytes(SCRI.Length + 1));
                ts.Write(EncodeZString(SCRI));
            }
            if (ITEX is not null)
            {
                ts.Write(Encoding.GetBytes("ITEX"));
                ts.Write(BitConverter.GetBytes(ITEX.Length + 1));
                ts.Write(EncodeZString(ITEX));
            }
            if (TEXT is not null)
            {
                ts.Write(Encoding.GetBytes("TEXT"));
                ts.Write(BitConverter.GetBytes(TEXT.Length));
                ts.Write(Encoding.GetBytes(TEXT));
            }
            if (ENAM is not null)
            {
                ts.Write(Encoding.GetBytes("ENAM"));
                ts.Write(BitConverter.GetBytes(ENAM.Length + 1));
                ts.Write(EncodeZString(ENAM));
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
