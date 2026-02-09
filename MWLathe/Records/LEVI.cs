using System.Text;

namespace MWLathe.Records
{
    public class LEVI : Record
    {
        public string NAME { get; set; }
        public uint? DATA { get; set; }
        public byte? NNAM { get; set; }
        public uint? INDX { get; set; }
        public List<(string, ushort)> Items { get; set; } = new List<(string, ushort)>();

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
                    case "DATA":
                        bytesRead += bs.Read(buffer, 0, 4);
                        DATA = BitConverter.ToUInt32(buffer);
                        break;
                    case "NNAM":
                        bytesRead += bs.Read(buffer, 0, 1);
                        NNAM = buffer[0];
                        break;
                    case "INDX":
                        bytesRead += bs.Read(buffer, 0, 4);
                        INDX = BitConverter.ToUInt32(buffer);
                        break;
                    case "INAM":
                        var itemID = ReadZString(bs);
                        bytesRead += itemID.Length + 1;
                        // INTV subfield
                        bytesRead += bs.Read(buffer, 0, 4);
                        fieldType = Encoding.GetEncoding("Windows-1252").GetString(buffer, 0, 4);
                        if (fieldType != "INTV")
                        {
                            throw new Exception($"Expected field \"INTV\", got field \"{fieldType}\"");
                        }
                        bytesRead += bs.Read(buffer, 0, 6);
                        fieldSize = BitConverter.ToUInt32(buffer);
                        Items.Add((itemID, BitConverter.ToUInt16(buffer, 4)));
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
            Items = Items.Select(x => x.Item1.Equals(oldID, StringComparison.OrdinalIgnoreCase) ? (newID, x.Item2) : (x.Item1, x.Item2)).ToList();
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + NAME.Length + 1);
            if (DATA.HasValue)
            {
                RecordSize += 12;
            }
            if (NNAM.HasValue)
            {
                RecordSize += 9;
            }
            if (INDX.HasValue)
            {
                RecordSize += 12;
            }
            RecordSize += (uint)Items.Sum(x => 8 + x.Item1.Length + 1 + 8 + 2);
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length + 1));
            ts.Write(EncodeZString(NAME));
            if (DATA.HasValue)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("DATA"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(DATA.Value));
            }
            if (NNAM.HasValue)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("NNAM"));
                ts.Write(BitConverter.GetBytes(1));
                ts.WriteByte(NNAM.Value);
            }
            if (INDX.HasValue)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("INDX"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(INDX.Value));
            }
            foreach (var item in Items)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("INAM"));
                ts.Write(BitConverter.GetBytes(item.Item1.Length + 1));
                ts.Write(EncodeZString(item.Item1));
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("INTV"));
                ts.Write(BitConverter.GetBytes(2));
                ts.Write(BitConverter.GetBytes(item.Item2));
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
