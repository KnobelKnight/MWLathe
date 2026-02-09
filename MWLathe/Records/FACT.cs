using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class FACT : Record
    {
        public string NAME { get; set; }
        public string? FNAM { get; set; }
        public List<string> RankNames { get; set; } = new List<string>();
        public FADT? FADT { get; set; }
        public List<(string, int)> Reactions { get; set; } = new List<(string, int)>();

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
                    case "RNAM":
                        bytesRead += bs.Read(buffer, 0, 32);
                        var rankName = Encoding.GetEncoding("Windows-1252").GetString(buffer, 0, 32);
                        RankNames.Add(rankName);
                        break;
                    case "FADT":
                        FADT = new FADT();
                        bytesRead += bs.Read(buffer, 0, 8);
                        FADT.Attribute1 = BitConverter.ToUInt32(buffer);
                        FADT.Attribute2 = BitConverter.ToUInt32(buffer, 4);
                        for (int i = 0; i < 10; i++)
                        {
                            var rankData = new RankData();
                            bytesRead += bs.Read(buffer, 0, 20);
                            rankData.Attribute1Modifier = BitConverter.ToUInt32(buffer);
                            rankData.Attribute2Modifier = BitConverter.ToUInt32(buffer, 4);
                            rankData.PrimarySkill = BitConverter.ToUInt32(buffer, 8);
                            rankData.FavoredSkill = BitConverter.ToUInt32(buffer, 12);
                            rankData.FactionReaction = BitConverter.ToUInt32(buffer, 16);
                            FADT.RankData.Add(rankData);
                        }
                        bytesRead += bs.Read(buffer, 0, 32);
                        FADT.Skill1 = BitConverter.ToInt32(buffer);
                        FADT.Skill2 = BitConverter.ToInt32(buffer, 4);
                        FADT.Skill3 = BitConverter.ToInt32(buffer, 8);
                        FADT.Skill4 = BitConverter.ToInt32(buffer, 12);
                        FADT.Skill5 = BitConverter.ToInt32(buffer, 16);
                        FADT.Skill6 = BitConverter.ToInt32(buffer, 20);
                        FADT.Skill7 = BitConverter.ToInt32(buffer, 24);
                        FADT.Flags = BitConverter.ToUInt32(buffer, 28);
                        break;
                    case "ANAM":
                        var reactName = ReadStringField(bs, fieldSize);
                        bytesRead += (int)fieldSize;
                        // Reaction subfield
                        bytesRead += bs.Read(buffer, 0, 12);
                        fieldType = Encoding.GetEncoding("Windows-1252").GetString(buffer, 0, 4);
                        if (fieldType != "INTV")
                        {
                            throw new Exception($"Expected field \"INTV\", got field \"{fieldType}\"");
                        }
                        fieldSize = BitConverter.ToUInt32(buffer, 4);
                        var reactValue = BitConverter.ToInt32(buffer, 8);
                        Reactions.Add((reactName, reactValue));
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
            Reactions = Reactions.Select(x => x.Item1.Equals(oldID, StringComparison.OrdinalIgnoreCase) ? (newID, x.Item2) : (x.Item1, x.Item2)).ToList();
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + NAME.Length + 1);
            if (FNAM is not null)
            {
                RecordSize += (uint)(8 + FNAM.Length + 1);
            }
            RecordSize += (uint)RankNames.Count * (32 + 8);
            if (FADT is not null)
            {
                RecordSize += FADT.StructSize + 8;
            }
            RecordSize += (uint)Reactions.Sum(x => 8 + x.Item1.Length + 8 + 4);
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
            foreach (var rankName in RankNames)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("RNAM"));
                ts.Write(BitConverter.GetBytes(32));
                ts.Write(EncodeChar32(rankName));
            }
            if (FADT is not null)
            {
                FADT.Write(ts);
            }
            foreach (var reaction in Reactions)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("ANAM"));
                ts.Write(BitConverter.GetBytes(reaction.Item1.Length));
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes(reaction.Item1));
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("INTV"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(reaction.Item2));
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
