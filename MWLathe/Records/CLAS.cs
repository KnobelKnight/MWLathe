using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class CLAS : Record
    {
        public string NAME { get; set; }
        public string FNAM { get; set; }
        public CLDT CLDT { get; set; } = new CLDT();
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
                    case "CLDT":
                        bytesRead += bs.Read(buffer, 0, 60);
                        CLDT.PrimaryAttribute1 = BitConverter.ToUInt32(buffer);
                        CLDT.PrimaryAttribute2 = BitConverter.ToUInt32(buffer, 4);
                        CLDT.Specialization = BitConverter.ToUInt32(buffer, 8);
                        CLDT.MinorSkill1 = BitConverter.ToUInt32(buffer, 12);
                        CLDT.MajorSkill1 = BitConverter.ToUInt32(buffer, 16);
                        CLDT.MinorSkill2 = BitConverter.ToUInt32(buffer, 20);
                        CLDT.MajorSkill2 = BitConverter.ToUInt32(buffer, 24);
                        CLDT.MinorSkill3 = BitConverter.ToUInt32(buffer, 28);
                        CLDT.MajorSkill3 = BitConverter.ToUInt32(buffer, 32);
                        CLDT.MinorSkill4 = BitConverter.ToUInt32(buffer, 36);
                        CLDT.MajorSkill4 = BitConverter.ToUInt32(buffer, 40);
                        CLDT.MinorSkill5 = BitConverter.ToUInt32(buffer, 44);
                        CLDT.MajorSkill5 = BitConverter.ToUInt32(buffer, 48);
                        CLDT.Flags = BitConverter.ToUInt32(buffer, 52);
                        CLDT.AutocalcFlags = BitConverter.ToUInt32(buffer, 56);
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
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + NAME.Length + 1);
            RecordSize += (uint)(8 + FNAM.Length + 1);
            RecordSize += CLDT.StructSize + 8;
            if (DESC is not null)
            {
                RecordSize += (uint)(8 + DESC.Length);
            }
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length + 1));
            ts.Write(EncodeZString(NAME));
            ts.Write(Encoding.GetBytes("FNAM"));
            ts.Write(BitConverter.GetBytes(FNAM.Length + 1));
            ts.Write(EncodeZString(FNAM));
            CLDT.Write(ts);
            if (DESC is not null)
            {
                ts.Write(Encoding.GetBytes("DESC"));
                ts.Write(BitConverter.GetBytes(DESC.Length));
                ts.Write(Encoding.GetBytes(DESC));
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
