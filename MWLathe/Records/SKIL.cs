using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class SKIL : Record
    {
        public uint INDX { get; set; }
        public SKDT SKDT { get; set; } = new SKDT();
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
                    case "INDX":
                        bytesRead += bs.Read(buffer, 0, 4);
                        INDX = BitConverter.ToUInt32(buffer);
                        break;
                    case "SKDT":
                        bytesRead += bs.Read(buffer, 0, 24);
                        SKDT.Attribute = BitConverter.ToUInt32(buffer);
                        SKDT.Specialization = BitConverter.ToUInt32(buffer, 4);
                        SKDT.UseValue1 = BitConverter.ToSingle(buffer, 8);
                        SKDT.UseValue2 = BitConverter.ToSingle(buffer, 12);
                        SKDT.UseValue3 = BitConverter.ToSingle(buffer, 16);
                        SKDT.UseValue4 = BitConverter.ToSingle(buffer, 20);
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
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += 12; // INDX
            RecordSize += 8 + SKDT.StructSize;
            if (DESC is not null)
            {
                RecordSize += (uint)(8 + DESC.Length);
            }
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetBytes("INDX"));
            ts.Write(BitConverter.GetBytes(4));
            ts.Write(BitConverter.GetBytes(INDX));
            SKDT.Write(ts);
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
