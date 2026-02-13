using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class CONT : Record
    {
        public string NAME { get; set; }
        public string? MODL { get; set; }
        public string? FNAM { get; set; }
        public float? CNDT { get; set; }
        public uint? FLAG { get; set; }
        public List<NPCO> Items { get; set; } = new List<NPCO>();
        public string? SCRI { get; set; }

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
                    case "CNDT":
                        bytesRead += bs.Read(buffer, 0, 4);
                        CNDT = BitConverter.ToSingle(buffer);
                        break;
                    case "FLAG":
                        bytesRead += bs.Read(buffer, 0, 4);
                        FLAG = BitConverter.ToUInt32(buffer);
                        break;
                    case "NPCO":
                        bytesRead += bs.Read(buffer, 0, 36);
                        Items.Add(new NPCO
                        {
                            Count = BitConverter.ToInt32(buffer),
                            ID = Encoding.GetString(buffer, 4, 32).TrimEnd('\0')
                        });
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
            if (NAME.Equals(oldID, StringComparison.OrdinalIgnoreCase))
            {
                if (newID.Length > 23)
                {
                    Console.Error.WriteLine($"Aborting: new container ID \"{newID}\" is {newID.Length} characters (max 23)");
                    Environment.Exit(3);
                }
                NAME = newID;
                Updated = true;
            }
            // TODO: set Updated here
            foreach (var item in Items.Where(x => x.ID.Equals(oldID, StringComparison.OrdinalIgnoreCase)))
            {
                item.ID = newID;
            }
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
            if (CNDT.HasValue)
            {
                RecordSize += 12;
            }
            if (FLAG.HasValue)
            {
                RecordSize += 12;
            }
            if (SCRI is not null)
            {
                RecordSize += (uint)(8 + SCRI.Length + 1);
            }
            RecordSize += (uint)Items.Count * (NPCO.StructSize + 8);
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
            if (CNDT.HasValue)
            {
                ts.Write(Encoding.GetBytes("CNDT"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(CNDT.Value));
            }
            if (FLAG.HasValue)
            {
                ts.Write(Encoding.GetBytes("FLAG"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(FLAG.Value));
            }
            if (SCRI is not null)
            {
                ts.Write(Encoding.GetBytes("SCRI"));
                ts.Write(BitConverter.GetBytes(SCRI.Length + 1));
                ts.Write(EncodeZString(SCRI));
            }
            foreach (var item in Items)
            {
                item.Write(ts);
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
