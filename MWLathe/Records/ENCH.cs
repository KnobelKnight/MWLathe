using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class ENCH : Record
    {
        public string NAME { get; set; }
        public ENDT? ENDT { get; set; }
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
                    case "ENDT":
                        ENDT = new ENDT();
                        bytesRead += bs.Read(buffer, 0, 16);
                        ENDT.Type = BitConverter.ToUInt32(buffer);
                        ENDT.Cost = BitConverter.ToUInt32(buffer, 4);
                        ENDT.Charge = BitConverter.ToUInt32(buffer, 8);
                        ENDT.Flags = BitConverter.ToUInt32(buffer, 12);
                        break;
                    case "ENAM":
                        bytesRead += bs.Read(buffer, 0, 24);
                        var newEffect = new ENAM();
                        newEffect.Index = BitConverter.ToUInt16(buffer);
                        newEffect.Skill = (sbyte)buffer[2];
                        newEffect.Attribute = (sbyte)buffer[3];
                        newEffect.Range = BitConverter.ToUInt32(buffer, 4);
                        newEffect.Area = BitConverter.ToUInt32(buffer, 8);
                        newEffect.Duration = BitConverter.ToUInt32(buffer, 12);
                        newEffect.MagnitudeMin = BitConverter.ToUInt32(buffer, 16);
                        newEffect.MagnitudeMax = BitConverter.ToUInt32(buffer, 20);
                        Effects.Add(newEffect);
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
            if (ENDT is not null)
            {
                RecordSize += 8 + ENDT.StructSize;
            }
            RecordSize += (uint)Effects.Count * (8 + ENAM.StructSize);
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length + 1));
            ts.Write(EncodeZString(NAME));
            if (ENDT is not null)
            {
                ENDT.Write(ts);
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
