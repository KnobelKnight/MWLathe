using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class LIGH : Record
    {
        public string NAME { get; set; }
        public string? MODL { get; set; }
        public string? FNAM { get; set; }
        public string? ITEX { get; set; }
        public LHDT? LHDT { get; set; }
        public string? SNAM { get; set; }
        public string? SCRI { get; set; }

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
                    case "FNAM":
                        FNAM = ReadZString(bs);
                        bytesRead += FNAM.Length + 1;
                        break;
                    case "ITEX":
                        ITEX = ReadZString(bs);
                        bytesRead += ITEX.Length + 1;
                        break;
                    case "LHDT":
                        LHDT = new LHDT();
                        bytesRead += bs.Read(buffer, 0, 24);
                        LHDT.Weight = BitConverter.ToSingle(buffer);
                        LHDT.Value = BitConverter.ToUInt32(buffer, 4);
                        LHDT.Time = BitConverter.ToInt32(buffer, 8);
                        LHDT.Radius = BitConverter.ToUInt32(buffer, 12);
                        LHDT.Color = new RGB
                        {
                            Red = buffer[16],
                            Green = buffer[17],
                            Blue = buffer[18]
                        };
                        // Next byte is junk
                        LHDT.Flags = BitConverter.ToUInt32(buffer, 20);
                        break;
                    case "SNAM":
                        SNAM = ReadZString(bs);
                        bytesRead += SNAM.Length + 1;
                        break;
                    case "SCRI":
                        SCRI = ReadZString(bs);
                        bytesRead += SCRI.Length + 1;
                        break;
                    default:
                        throw new Exception($"Unknown {GetType().Name} field \"{fieldType}\"");
                }
            }
        }

        public override void UpdateID(string oldID, string newID)
        {
            NAME = ReplaceID(NAME, oldID, newID);
            SNAM = ReplaceID(SNAM, oldID, newID);
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
            if (ITEX is not null)
            {
                RecordSize += (uint)(8 + ITEX.Length + 1);
            }
            if (LHDT is not null)
            {
                RecordSize += LHDT.StructSize + 8;
            }
            if (SCRI is not null)
            {
                RecordSize += (uint)(8 + SCRI.Length + 1);
            }
            if (SNAM is not null)
            {
                RecordSize += (uint)(8 + SNAM.Length + 1);
            }
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
            if (FNAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("FNAM"));
                ts.Write(BitConverter.GetBytes(FNAM.Length + 1));
                ts.Write(EncodeZString(FNAM));
            }
            if (ITEX is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("ITEX"));
                ts.Write(BitConverter.GetBytes(ITEX.Length + 1));
                ts.Write(EncodeZString(ITEX));
            }
            if (LHDT is not null)
            {
                LHDT.Write(ts);
            }
            if (SCRI is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("SCRI"));
                ts.Write(BitConverter.GetBytes(SCRI.Length + 1));
                ts.Write(EncodeZString(SCRI));
            }
            if (SNAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("SNAM"));
                ts.Write(BitConverter.GetBytes(SNAM.Length + 1));
                ts.Write(EncodeZString(SNAM));
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
