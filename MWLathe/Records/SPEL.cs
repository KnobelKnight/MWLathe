using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class SPEL : Record
    {
        public string NAME { get; set; }
        public string? FNAM { get; set; }
        public SPDT? SPDT { get; set; }
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
                    case "FNAM":
                        FNAM = ReadZString(bs);
                        bytesRead += FNAM.Length + 1;
                        break;
                    case "SPDT":
                        SPDT = new SPDT();
                        bytesRead += bs.Read(buffer, 0, 12);
                        SPDT.Type = BitConverter.ToUInt32(buffer);
                        SPDT.Cost = BitConverter.ToUInt32(buffer, 4);
                        SPDT.Flags = BitConverter.ToUInt32(buffer, 8);
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
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + NAME.Length + 1);
            if (FNAM is not null)
            {
                RecordSize += (uint)(8 + FNAM.Length + 1);
            }
            if (SPDT is not null)
            {
                RecordSize += SPDT.StructSize + 8;
            }
            RecordSize += (uint)Effects.Count * (8 + ENAM.StructSize);
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length + 1));
            ts.Write(EncodeZString(NAME));
            if (FNAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("FNAM"));
                ts.Write(BitConverter.GetBytes(FNAM.Length + 1));
                ts.Write(EncodeZString(FNAM));
            }
            if (SPDT is not null)
            {
                SPDT.Write(ts);
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
