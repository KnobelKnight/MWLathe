using System.Text;

namespace MWLathe.Records
{
    public class BSGN : Record
    {
        public string NAME { get; set; }
        public string? FNAM { get; set; }
        // Max 32 chars each
        public List<string> Spells { get; set; } = new List<string>();
        public string? TNAM { get; set; }
        public string? DESC { get; set; }

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
                    case "FNAM":
                        FNAM = ReadZString(bs);
                        bytesRead += FNAM.Length + 1;
                        break;
                    case "NPCS":
                        var newSpell = ReadStringField(bs, fieldSize);
                        bytesRead += (int)fieldSize;
                        Spells.Add(newSpell);
                        break;
                    case "TNAM":
                        TNAM = ReadZString(bs);
                        bytesRead += TNAM.Length + 1;
                        break;
                    case "DESC":
                        DESC = ReadZString(bs);
                        bytesRead += DESC.Length + 1;
                        break;
                    default:
                        throw new Exception($"Unknown {GetType().Name} field \"{fieldType}\"");
                }
            }
        }

        public override void UpdateID(string oldID, string newID)
        {
            NAME = ReplaceID(NAME, oldID, newID);
            // TODO: set Updated here
            Spells = Spells.Select(x => x.Equals(oldID, StringComparison.OrdinalIgnoreCase) ? newID : x).ToList();
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + NAME.Length + 1);
            if (FNAM is not null)
            {
                RecordSize += (uint)(8 + FNAM.Length + 1);
            }
            if (TNAM is not null)
            {
                RecordSize += (uint)(8 + TNAM.Length + 1);
            }
            if (DESC is not null)
            {
                RecordSize += (uint)(8 + DESC.Length + 1);
            }
            RecordSize += (uint)(40 * Spells.Count);
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length + 1));
            ts.Write(EncodeZString(NAME));
            if (FNAM is not null)
            {
                ts.Write(Encoding.GetBytes("FNAM"));
                ts.Write(BitConverter.GetBytes(FNAM.Length + 1));
                ts.Write(EncodeZString(FNAM));
            }
            if (TNAM is not null)
            {
                ts.Write(Encoding.GetBytes("TNAM"));
                ts.Write(BitConverter.GetBytes(TNAM.Length + 1));
                ts.Write(EncodeZString(TNAM));
            }
            if (DESC is not null)
            {
                ts.Write(Encoding.GetBytes("DESC"));
                ts.Write(BitConverter.GetBytes(DESC.Length + 1));
                ts.Write(EncodeZString(DESC));
            }
            foreach (var spell in Spells)
            {
                ts.Write(Encoding.GetBytes("NPCS"));
                ts.Write(BitConverter.GetBytes(32));
                ts.Write(EncodeChar32(spell));
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
