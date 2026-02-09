using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class RACE : Record
    {
        public string NAME { get; set; }
        public string? FNAM { get; set; }
        public RADT RADT { get; set; } = new RADT();
        public List<string> Abilities { get; set; } = new List<string>();
        public string? DESC { get; set; }

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
                    case "RADT":
                        bytesRead += bs.Read(buffer, 0, 140);
                        RADT.Skill1 = BitConverter.ToInt32(buffer);
                        RADT.Bonus1 = BitConverter.ToInt32(buffer, 4);
                        RADT.Skill2 = BitConverter.ToInt32(buffer, 8);
                        RADT.Bonus2 = BitConverter.ToInt32(buffer, 12);
                        RADT.Skill3 = BitConverter.ToInt32(buffer, 16);
                        RADT.Bonus3 = BitConverter.ToInt32(buffer, 20);
                        RADT.Skill4 = BitConverter.ToInt32(buffer, 24);
                        RADT.Bonus4 = BitConverter.ToInt32(buffer, 28);
                        RADT.Skill5 = BitConverter.ToInt32(buffer, 32);
                        RADT.Bonus5 = BitConverter.ToInt32(buffer, 36);
                        RADT.Skill6 = BitConverter.ToInt32(buffer, 40);
                        RADT.Bonus6 = BitConverter.ToInt32(buffer, 44);
                        RADT.Skill7 = BitConverter.ToInt32(buffer, 48);
                        RADT.Bonus7 = BitConverter.ToInt32(buffer, 52);
                        RADT.MaleStrength = BitConverter.ToUInt32(buffer, 56);
                        RADT.FemaleStrength = BitConverter.ToUInt32(buffer, 60);
                        RADT.MaleIntelligence = BitConverter.ToUInt32(buffer, 64);
                        RADT.FemaleIntelligence = BitConverter.ToUInt32(buffer, 68);
                        RADT.MaleWillpower = BitConverter.ToUInt32(buffer, 72);
                        RADT.FemaleWillpower = BitConverter.ToUInt32(buffer, 76);
                        RADT.MaleAgility = BitConverter.ToUInt32(buffer, 80);
                        RADT.FemaleAgility = BitConverter.ToUInt32(buffer, 84);
                        RADT.MaleSpeed = BitConverter.ToUInt32(buffer, 88);
                        RADT.FemaleSpeed = BitConverter.ToUInt32(buffer, 92);
                        RADT.MaleEndurance = BitConverter.ToUInt32(buffer, 96);
                        RADT.FemaleEndurance = BitConverter.ToUInt32(buffer, 100);
                        RADT.MalePersonality = BitConverter.ToUInt32(buffer, 104);
                        RADT.FemalePersonality = BitConverter.ToUInt32(buffer, 108);
                        RADT.MaleLuck = BitConverter.ToUInt32(buffer, 112);
                        RADT.FemaleLuck = BitConverter.ToUInt32(buffer, 116);
                        RADT.MaleHeight = BitConverter.ToSingle(buffer, 120);
                        RADT.FemaleHeight = BitConverter.ToSingle(buffer, 124);
                        RADT.MaleWeight = BitConverter.ToSingle(buffer, 128);
                        RADT.FemaleWeight = BitConverter.ToSingle(buffer, 132);
                        RADT.Flags = BitConverter.ToUInt32(buffer, 136);
                        break;
                    case "NPCS":
                        bytesRead += bs.Read(buffer, 0, 32);
                        var newAbility = Encoding.GetEncoding("Windows-1252").GetString(buffer, 0, 32).TrimEnd('\0');
                        Abilities.Add(newAbility);
                        break;
                    case "DESC":
                        DESC = ReadStringField(bs, fieldSize);
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
            // TODO: set Updated here
            Abilities = Abilities.Select(x => x.Equals(oldID, StringComparison.OrdinalIgnoreCase) ? newID : x).ToList();
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + NAME.Length + 1);
            if (FNAM is not null)
            {
                RecordSize += (uint)(8 + FNAM.Length + 1);
            }
            RecordSize += RADT.StructSize + 8;
            RecordSize += (uint)Abilities.Count * (8 + 32);
            if (DESC is not null)
            {
                RecordSize += (uint)(8 + DESC.Length);
            }
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
            RADT.Write(ts);
            foreach (var ability in Abilities)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("NPCS"));
                ts.Write(BitConverter.GetBytes(32));
                ts.Write(EncodeChar32(ability));
            }
            if (DESC is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("DESC"));
                ts.Write(BitConverter.GetBytes(DESC.Length));
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes(DESC));
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
