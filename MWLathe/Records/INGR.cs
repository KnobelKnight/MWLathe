using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class INGR : Record
    {
        public string NAME { get; set; }
        public string? MODL { get; set; }
        public string? FNAM { get; set; }
        public IRDT? IRDT { get; set; }
        public string? SCRI { get; set; }
        public string? ITEX { get; set; }

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
                    case "IRDT":
                        IRDT = new IRDT();
                        bytesRead += bs.Read(buffer, 0, 56);
                        IRDT.Weight = BitConverter.ToSingle(buffer);
                        IRDT.Value = BitConverter.ToUInt32(buffer, 4);
                        IRDT.Effect1 = BitConverter.ToInt32(buffer, 8);
                        IRDT.Effect2 = BitConverter.ToInt32(buffer, 12);
                        IRDT.Effect3 = BitConverter.ToInt32(buffer, 16);
                        IRDT.Effect4 = BitConverter.ToInt32(buffer, 20);
                        IRDT.Skill1 = BitConverter.ToInt32(buffer, 24);
                        IRDT.Skill2 = BitConverter.ToInt32(buffer, 28);
                        IRDT.Skill3 = BitConverter.ToInt32(buffer, 32);
                        IRDT.Skill4 = BitConverter.ToInt32(buffer, 36);
                        IRDT.Attribute1 = BitConverter.ToInt32(buffer, 40);
                        IRDT.Attribute2 = BitConverter.ToInt32(buffer, 44);
                        IRDT.Attribute3 = BitConverter.ToInt32(buffer, 48);
                        IRDT.Attribute4 = BitConverter.ToInt32(buffer, 52);
                        break;
                    case "SCRI":
                        SCRI = ReadZString(bs);
                        bytesRead += SCRI.Length + 1;
                        break;
                    case "ITEX":
                        ITEX = ReadZString(bs);
                        bytesRead += ITEX.Length + 1;
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
            if (FNAM is not null)
            {
                RecordSize += (uint)(8 + FNAM.Length + 1);
            }
            if (IRDT is not null)
            {
                RecordSize += IRDT.StructSize + 8;
            }
            if (SCRI is not null)
            {
                RecordSize += (uint)(8 + SCRI.Length + 1);
            }
            if (ITEX is not null)
            {
                RecordSize += (uint)(8 + ITEX.Length + 1);
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
            if (IRDT is not null)
            {
                IRDT.Write(ts);
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
            if (Deleted.HasValue)
            {
                ts.Write(Encoding.GetBytes("DELE"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(Deleted.Value));
            }
        }
    }
}
