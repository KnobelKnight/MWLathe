using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class MGEF : Record
    {
        public uint INDX { get; set; }
        public MEDT MEDT { get; set; } = new MEDT();
        public string? ITEX { get; set; }
        public string? PTEX { get; set; }
        public string? BSND { get; set; }
        public string? CSND { get; set; }
        public string? HSND { get; set; }
        public string? ASND { get; set; }
        public string? CVFX { get; set; }
        public string? BVFX { get; set; }
        public string? HVFX { get; set; }
        public string? AVFX { get; set; }
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
                    case "INDX":
                        bytesRead += bs.Read(buffer, 0, 4);
                        INDX = BitConverter.ToUInt32(buffer);
                        break;
                    case "MEDT":
                        bytesRead += bs.Read(buffer, 0, 36);
                        MEDT.School = BitConverter.ToUInt32(buffer);
                        MEDT.BaseCost = BitConverter.ToSingle(buffer, 4);
                        MEDT.Flags = BitConverter.ToUInt32(buffer, 8);
                        MEDT.Red = BitConverter.ToUInt32(buffer, 12);
                        MEDT.Green = BitConverter.ToUInt32(buffer, 16);
                        MEDT.Blue = BitConverter.ToUInt32(buffer, 20);
                        MEDT.Speed = BitConverter.ToSingle(buffer, 24);
                        MEDT.Size = BitConverter.ToSingle(buffer, 28);
                        MEDT.SizeCap = BitConverter.ToSingle(buffer, 32);
                        break;
                    case "ITEX":
                        ITEX = ReadZString(bs);
                        bytesRead += ITEX.Length + 1;
                        break;
                    case "PTEX":
                        PTEX = ReadZString(bs);
                        bytesRead += PTEX.Length + 1;
                        break;
                    case "BSND":
                        BSND = ReadZString(bs);
                        bytesRead += BSND.Length + 1;
                        break;
                    case "CSND":
                        CSND = ReadZString(bs);
                        bytesRead += CSND.Length + 1;
                        break;
                    case "HSND":
                        HSND = ReadZString(bs);
                        bytesRead += HSND.Length + 1;
                        break;
                    case "ASND":
                        ASND = ReadZString(bs);
                        bytesRead += ASND.Length + 1;
                        break;
                    case "CVFX":
                        CVFX = ReadZString(bs);
                        bytesRead += CVFX.Length + 1;
                        break;
                    case "BVFX":
                        BVFX = ReadZString(bs);
                        bytesRead += BVFX.Length + 1;
                        break;
                    case "HVFX":
                        HVFX = ReadZString(bs);
                        bytesRead += HVFX.Length + 1;
                        break;
                    case "AVFX":
                        AVFX = ReadZString(bs);
                        bytesRead += AVFX.Length + 1;
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
            BSND = ReplaceID(BSND, oldID, newID);
            CSND = ReplaceID(CSND, oldID, newID);
            HSND = ReplaceID(HSND, oldID, newID);
            ASND = ReplaceID(ASND, oldID, newID);
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += 12; // INDX
            RecordSize += 8 + MEDT.StructSize;
            if (ITEX is not null)
            {
                RecordSize += (uint)(8 + ITEX.Length + 1);
            }
            if (PTEX is not null)
            {
                RecordSize += (uint)(8 + PTEX.Length + 1);
            }
            if (BSND is not null)
            {
                RecordSize += (uint)(8 + BSND.Length + 1);
            }
            if (CSND is not null)
            {
                RecordSize += (uint)(8 + CSND.Length + 1);
            }
            if (HSND is not null)
            {
                RecordSize += (uint)(8 + HSND.Length + 1);
            }
            if (ASND is not null)
            {
                RecordSize += (uint)(8 + ASND.Length + 1);
            }
            if (CVFX is not null)
            {
                RecordSize += (uint)(8 + CVFX.Length + 1);
            }
            if (BVFX is not null)
            {
                RecordSize += (uint)(8 + BVFX.Length + 1);
            }
            if (HVFX is not null)
            {
                RecordSize += (uint)(8 + HVFX.Length + 1);
            }
            if (AVFX is not null)
            {
                RecordSize += (uint)(8 + AVFX.Length + 1);
            }
            if (DESC is not null)
            {
                RecordSize += (uint)(8 + DESC.Length);
            }
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("INDX"));
            ts.Write(BitConverter.GetBytes(4));
            ts.Write(BitConverter.GetBytes(INDX));
            MEDT.Write(ts);
            if (ITEX is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("ITEX"));
                ts.Write(BitConverter.GetBytes(ITEX.Length + 1));
                ts.Write(EncodeZString(ITEX));
            }
            if (PTEX is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("PTEX"));
                ts.Write(BitConverter.GetBytes(PTEX.Length + 1));
                ts.Write(EncodeZString(PTEX));
            }
            if (BSND is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("BSND"));
                ts.Write(BitConverter.GetBytes(BSND.Length + 1));
                ts.Write(EncodeZString(BSND));
            }
            if (CSND is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("CSND"));
                ts.Write(BitConverter.GetBytes(CSND.Length + 1));
                ts.Write(EncodeZString(CSND));
            }
            if (HSND is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("HSND"));
                ts.Write(BitConverter.GetBytes(HSND.Length + 1));
                ts.Write(EncodeZString(HSND));
            }
            if (ASND is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("ASND"));
                ts.Write(BitConverter.GetBytes(ASND.Length + 1));
                ts.Write(EncodeZString(ASND));
            }
            if (CVFX is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("CVFX"));
                ts.Write(BitConverter.GetBytes(CVFX.Length + 1));
                ts.Write(EncodeZString(CVFX));
            }
            if (BVFX is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("BVFX"));
                ts.Write(BitConverter.GetBytes(BVFX.Length + 1));
                ts.Write(EncodeZString(BVFX));
            }
            if (HVFX is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("HVFX"));
                ts.Write(BitConverter.GetBytes(HVFX.Length + 1));
                ts.Write(EncodeZString(HVFX));
            }
            if (AVFX is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("AVFX"));
                ts.Write(BitConverter.GetBytes(AVFX.Length + 1));
                ts.Write(EncodeZString(AVFX));
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
