using MWLathe.Helpers;
using System.Text;
using System.Text.RegularExpressions;

namespace MWLathe.Records
{
    public class SCPT : Record
    {
        public SCHD SCHD { get; set; } = new SCHD();
        public List<string>? SCVR { get; set; }
        public List<byte>? SCDT { get; set; }
        public string? SCTX { get; set; }

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
                    case "SCHD":
                        bytesRead += bs.Read(buffer, 0, 52);
                        SCHD.Name = Encoding.GetString(buffer, 0, 32).TrimEnd('\0');
                        SCHD.ShortCount = BitConverter.ToUInt32(buffer, 32);
                        SCHD.LongCount = BitConverter.ToUInt32(buffer, 36);
                        SCHD.FloatCount = BitConverter.ToUInt32(buffer, 40);
                        SCHD.ScriptSize = BitConverter.ToUInt32(buffer, 44);
                        SCHD.LocalSize = BitConverter.ToUInt32(buffer, 48);
                        break;
                    case "SCVR":
                        SCVR = new List<string>();
                        uint fieldBytes = 0;
                        while (fieldBytes < fieldSize)
                        {
                            var localName = ReadZString(bs);
                            fieldBytes += (uint)localName.Length + 1;
                            bytesRead += localName.Length + 1;
                            SCVR.Add(localName);
                        }
                        break;
                    case "SCDT":
                        SCDT = new List<byte>();
                        while (SCDT.Count < fieldSize)
                        {
                            bytesRead += bs.Read(buffer, 0, 1);
                            SCDT.Add(buffer[0]);
                        }
                        break;
                    case "SCTX":
                        SCTX = ReadStringField(bs, fieldSize);
                        bytesRead += (int)fieldSize;
                        break;
                    default:
                        throw new Exception($"Unknown {GetType().Name} field \"{fieldType}\"");
                }
            }
        }

        public override void UpdateID(string oldID, string newID)
        {
            SCHD.Name = ReplaceID(SCHD.Name, oldID, newID);
            // TODO: set Updated here
            if (SCTX is not null)
            {
                SCTX = Regex.Replace(SCTX, $"""(?:^|(?<=\W)|(?<=\\t)|(?<=(\\r\\n))){oldID}\b""", newID, RegexOptions.IgnoreCase);
            }
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += SCHD.StructSize + 8;
            if (SCVR is not null)
            {
                RecordSize += 8 + (uint)SCVR.Sum(x => x.Length + 1);
            }
            if (SCDT is not null)
            {
                RecordSize += 8 + (uint)SCDT.Count;
            }
            if (SCTX is not null)
            {
                RecordSize += 8 + (uint)SCTX.Length;
            }
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            SCHD.Write(ts);
            if (SCVR is not null)
            {
                ts.Write(Encoding.GetBytes("SCVR"));
                var byteCount = 0;
                foreach (var local in SCVR)
                {
                    byteCount += local.Length + 1;
                }
                ts.Write(BitConverter.GetBytes(byteCount));
                foreach (var local in SCVR)
                {
                    ts.Write(EncodeZString(local));
                }
            }
            if (SCDT is not null)
            {
                ts.Write(Encoding.GetBytes("SCDT"));
                ts.Write(BitConverter.GetBytes(SCDT.Count));
                ts.Write(SCDT.ToArray());
            }
            if (SCTX is not null)
            {
                ts.Write(Encoding.GetBytes("SCTX"));
                ts.Write(BitConverter.GetBytes(SCTX.Length));
                ts.Write(Encoding.GetBytes(SCTX));
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
