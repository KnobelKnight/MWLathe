using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class ALCH : Record
    {
        public string NAME { get; set; }
        public string? MODL { get; set; }
        public string? TEXT { get; set; }
        public string? SCRI { get; set; }
        public string? FNAM { get; set; }
        public ALDT? ALDT { get; set; }
        public List<ENAM> Effects { get; set; } = new List<ENAM>();

        public override void Populate(BufferedStream bs)
        {
            base.Populate(bs);
            byte[] buffer = new byte[256];
            int bytesRead = 0;
            while (bytesRead < RecordSize)
            {
                bytesRead += bs.Read(buffer, 0, 8);
                var fieldType = Encoding.GetEncoding("Windows-1252").GetString(buffer, 0, 4);
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
                    case "TEXT":
                        TEXT = ReadZString(bs);
                        bytesRead += TEXT.Length + 1;
                        break;
                    case "SCRI":
                        SCRI = ReadZString(bs);
                        bytesRead += SCRI.Length + 1;
                        break;
                    case "FNAM":
                        FNAM = ReadZString(bs);
                        bytesRead += FNAM.Length + 1;
                        break;
                    case "ALDT":
                        ALDT = new ALDT();
                        bytesRead += bs.Read(buffer, 0, 12);
                        ALDT.Weight = BitConverter.ToSingle(buffer);
                        ALDT.Value = BitConverter.ToUInt32(buffer, 4);
                        ALDT.Flags = BitConverter.ToUInt32(buffer, 8);
                        break;
                    case "ENAM":
                        bytesRead += bs.Read(buffer, 0, 24);
                        var effect = new ENAM();
                        effect.Index = BitConverter.ToUInt16(buffer);
                        effect.Skill = (sbyte)buffer[2];
                        effect.Attribute = (sbyte)buffer[3];
                        effect.Range = BitConverter.ToUInt32(buffer, 4);
                        effect.Area = BitConverter.ToUInt32(buffer, 8);
                        effect.Duration = BitConverter.ToUInt32(buffer, 12);
                        effect.MagnitudeMin = BitConverter.ToUInt32(buffer, 16);
                        effect.MagnitudeMax = BitConverter.ToUInt32(buffer, 20);
                        Effects.Add(effect);
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
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + NAME.Length + 1);
            if (MODL is not null)
            {
                RecordSize += (uint)(8 + MODL.Length + 1);
            }
            if (TEXT is not null)
            {
                RecordSize += (uint)(8 + TEXT.Length + 1);
            }
            if (SCRI is not null)
            {
                RecordSize += (uint)(8 + SCRI.Length + 1);
            }
            if (FNAM is not null)
            {
                RecordSize += (uint)(8 + FNAM.Length + 1);
            }
            if (ALDT is not null)
            {
                RecordSize += ALDT.StructSize + 8;
            }
            RecordSize += (uint)Effects.Count * (ENAM.StructSize + 8);
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length + 1));
            ts.Write(EncodeZString(NAME));
            if (MODL is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("MODL"));
                ts.Write(BitConverter.GetBytes(MODL.Length + 1));
                ts.Write(EncodeZString(MODL));
            }
            if (TEXT is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("TEXT"));
                ts.Write(BitConverter.GetBytes(TEXT.Length + 1));
                ts.Write(EncodeZString(TEXT));
            }
            if (SCRI is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("SCRI"));
                ts.Write(BitConverter.GetBytes(SCRI.Length + 1));
                ts.Write(EncodeZString(SCRI));
            }
            if (FNAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("FNAM"));
                ts.Write(BitConverter.GetBytes(FNAM.Length + 1));
                ts.Write(EncodeZString(FNAM));
            }
            if (ALDT is not null)
            {
                ALDT.Write(ts);
            }
            foreach (var effect in Effects)
            {
                effect.Write(ts);
            }
            if (Deleted.HasValue)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("DELE"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(Deleted.Value));
            }
        }
    }
}
