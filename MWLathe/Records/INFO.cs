using MWLathe.Helpers;
using System.Text;
using System.Text.RegularExpressions;

namespace MWLathe.Records
{
    public class INFO : Record
    {
        public string INAM { get; set; }
        public string PNAM { get; set; }
        public string NNAM { get; set; }
        public INFO_DATA? DATA { get; set; }
        public string? ONAM { get; set; }
        public string? RNAM { get; set; }
        public string? CNAM { get; set; }
        public string? FNAM { get; set; }
        public string? ANAM { get; set; }
        public string? DNAM { get; set; }
        public string? SNAM { get; set; }
        public string? NAME { get; set; }
        public List<FilterData> Filters { get; set; } = new List<FilterData>();
        public string? BNAM { get; set; }
        public bool? QSTN { get; set; }
        public bool? QSTF { get; set; }
        public bool? QSTR { get; set; }

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
                    case "INAM":
                        INAM = ReadZString(bs);
                        bytesRead += INAM.Length + 1;
                        break;
                    case "PNAM":
                        PNAM = ReadZString(bs);
                        bytesRead += PNAM.Length + 1;
                        break;
                    case "NNAM":
                        NNAM = ReadZString(bs);
                        bytesRead += NNAM.Length + 1;
                        break;
                    case "DATA":
                        bytesRead += bs.Read(buffer, 0, 12);
                        DATA = new INFO_DATA
                        {
                            Type = buffer[0],
                            // Next 3 bytes are junk
                            DispOrJournal = BitConverter.ToUInt32(buffer, 4),
                            Rank = (sbyte)buffer[8],
                            Gender = (sbyte)buffer[9],
                            PCRank = (sbyte)buffer[10]
                            // Last byte is junk
                        };
                        break;
                    case "ONAM":
                        ONAM = ReadStringField(bs, fieldSize);
                        bytesRead += (int)fieldSize;
                        break;
                    case "RNAM":
                        RNAM = ReadZString(bs);
                        bytesRead += RNAM.Length + 1;
                        break;
                    case "CNAM":
                        CNAM = ReadZString(bs);
                        bytesRead += CNAM.Length + 1;
                        break;
                    case "FNAM":
                        FNAM = ReadZString(bs);
                        bytesRead += FNAM.Length + 1;
                        break;
                    case "ANAM":
                        ANAM = ReadZString(bs);
                        bytesRead += ANAM.Length + 1;
                        break;
                    case "DNAM":
                        DNAM = ReadZString(bs);
                        bytesRead += DNAM.Length + 1;
                        break;
                    case "SNAM":
                        SNAM = ReadZString(bs);
                        bytesRead += SNAM.Length + 1;
                        break;
                    case "NAME":
                        NAME = ReadStringField(bs, fieldSize);
                        bytesRead += (int)fieldSize;
                        break;
                    case "SCVR":
                        bytesRead += bs.Read(buffer, 0, 5);
                        var newFilter = new FilterData
                        {
                            Index = (char)buffer[0],
                            Type = (char)buffer[1],
                            Details = Encoding.GetEncoding("Windows-1252").GetString(buffer, 2, 2),
                            Operator = (char)buffer[4]
                        };
                        if (fieldSize > 5)
                        {
                            newFilter.Name = ReadStringField(bs, fieldSize - 5);
                            bytesRead += (int)fieldSize - 5;
                        }
                        Filters.Add(newFilter);
                        break;
                    case "INTV":
                        bytesRead += bs.Read(buffer, 0, 4);
                        (Filters.LastOrDefault() ?? throw new Exception($"INTV before SCVR in INFO")).IntValue = BitConverter.ToUInt32(buffer);
                        break;
                    case "FLTV":
                        bytesRead += bs.Read(buffer, 0, 4);
                        (Filters.LastOrDefault() ?? throw new Exception($"FLTV before SCVR in INFO")).FloatValue = BitConverter.ToSingle(buffer);
                        break;
                    case "BNAM":
                        BNAM = ReadStringField(bs, fieldSize);
                        bytesRead += (int)fieldSize;
                        break;
                    case "QSTN":
                        bytesRead += bs.Read(buffer, 0, 1);
                        QSTN = buffer[0] != 0;
                        break;
                    case "QSTF":
                        bytesRead += bs.Read(buffer, 0, 1);
                        QSTF = buffer[0] != 0;
                        break;
                    case "QSTR":
                        bytesRead += bs.Read(buffer, 0, 1);
                        QSTR = buffer[0] != 0;
                        break;
                    default:
                        throw new Exception($"Unknown {GetType().Name} field \"{fieldType}\"");
                }
            }
        }

        public override void UpdateID(string oldID, string newID)
        {
            ONAM = ReplaceID(ONAM, oldID, newID);
            RNAM = ReplaceID(RNAM, oldID, newID);
            CNAM = ReplaceID(CNAM, oldID, newID);
            FNAM = ReplaceID(FNAM, oldID, newID);
            DNAM = ReplaceID(DNAM, oldID, newID);
            foreach (var filter in Filters)
            {
                filter.Name = ReplaceID(filter.Name, oldID, newID);
            }
            // TODO: set Updated here
            if (BNAM is not null)
            {
                BNAM = Regex.Replace(BNAM, $"""(?:^|(?<=\W)|(?<=\\t)|(?<=(\\r\\n))){oldID}\b""", newID, RegexOptions.IgnoreCase);
            }
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + INAM.Length + 1);
            RecordSize += (uint)(8 + PNAM.Length + 1);
            RecordSize += (uint)(8 + NNAM.Length + 1);
            if (DATA is not null)
            {
                RecordSize += 8 + INFO_DATA.StructSize;
            }
            if (ONAM is not null)
            {
                RecordSize += (uint)(8 + ONAM.Length + 1);
            }
            if (RNAM is not null)
            {
                RecordSize += (uint)(8 + RNAM.Length + 1);
            }
            if (CNAM is not null)
            {
                RecordSize += (uint)(8 + CNAM.Length + 1);
            }
            if (FNAM is not null)
            {
                RecordSize += (uint)(8 + FNAM.Length + 1);
            }
            if (ANAM is not null)
            {
                RecordSize += (uint)(8 + ANAM.Length + 1);
            }
            if (DNAM is not null)
            {
                RecordSize += (uint)(8 + DNAM.Length + 1);
            }
            if (SNAM is not null)
            {
                RecordSize += (uint)(8 + SNAM.Length + 1);
            }
            if (NAME is not null)
            {
                RecordSize += (uint)(8 + NAME.Length);
            }
            if (QSTR.HasValue)
            {
                RecordSize += 8 + 1;
            }
            RecordSize += (uint)Filters.Sum(x => x.GetByteSize());
            if (BNAM is not null)
            {
                RecordSize += (uint)(8 + BNAM.Length);
            }
            if (QSTN.HasValue)
            {
                RecordSize += 8 + 1;
            }
            if (QSTF.HasValue)
            {
                RecordSize += 8 + 1;
            }
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("INAM"));
            ts.Write(BitConverter.GetBytes(INAM.Length + 1));
            ts.Write(EncodeZString(INAM));
            ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("PNAM"));
            ts.Write(BitConverter.GetBytes(PNAM.Length + 1));
            ts.Write(EncodeZString(PNAM));
            ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("NNAM"));
            ts.Write(BitConverter.GetBytes(NNAM.Length + 1));
            ts.Write(EncodeZString(NNAM));
            if (DATA is not null)
            {
                DATA.Write(ts);
            }
            if (ONAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("ONAM"));
                ts.Write(BitConverter.GetBytes(ONAM.Length + 1));
                ts.Write(EncodeZString(ONAM));
            }
            if (RNAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("RNAM"));
                ts.Write(BitConverter.GetBytes(RNAM.Length + 1));
                ts.Write(EncodeZString(RNAM));
            }
            if (CNAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("CNAM"));
                ts.Write(BitConverter.GetBytes(CNAM.Length + 1));
                ts.Write(EncodeZString(CNAM));
            }
            if (FNAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("FNAM"));
                ts.Write(BitConverter.GetBytes(FNAM.Length + 1));
                ts.Write(EncodeZString(FNAM));
            }
            if (ANAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("ANAM"));
                ts.Write(BitConverter.GetBytes(ANAM.Length + 1));
                ts.Write(EncodeZString(ANAM));
            }
            if (DNAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("DNAM"));
                ts.Write(BitConverter.GetBytes(DNAM.Length + 1));
                ts.Write(EncodeZString(DNAM));
            }
            if (SNAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("SNAM"));
                ts.Write(BitConverter.GetBytes(SNAM.Length + 1));
                ts.Write(EncodeZString(SNAM));
            }
            if (NAME is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("NAME"));
                ts.Write(BitConverter.GetBytes(NAME.Length));
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes(NAME));
            }
            if (QSTR.HasValue)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("QSTR"));
                ts.Write(BitConverter.GetBytes(1));
                ts.WriteByte(QSTR.Value ? (byte)1 : (byte)0);
            }
            foreach (var filter in Filters)
            {
                filter.Write(ts);
            }
            if (BNAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("BNAM"));
                ts.Write(BitConverter.GetBytes(BNAM.Length));
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes(BNAM));
            }
            if (QSTN.HasValue)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("QSTN"));
                ts.Write(BitConverter.GetBytes(1));
                ts.WriteByte(QSTN.Value ? (byte)1 : (byte)0);
            }
            if (QSTF.HasValue)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("QSTF"));
                ts.Write(BitConverter.GetBytes(1));
                ts.WriteByte(QSTF.Value ? (byte)1 : (byte)0);
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
