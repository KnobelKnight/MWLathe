using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class PGRD : Record
    {
        public PGRD_DATA DATA { get; set; } = new PGRD_DATA();
        public string NAME { get; set; }
        public List<PathgridPoint>? PGRP { get; set; }
        public List<uint>? PGRC { get; set; }

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
                    case "DATA":
                        bytesRead += bs.Read(buffer, 0, 12);
                        DATA.GridX = BitConverter.ToInt32(buffer);
                        DATA.GridY = BitConverter.ToInt32(buffer, 4);
                        DATA.Flags = BitConverter.ToUInt16(buffer, 8);
                        DATA.PathgridPointCount = BitConverter.ToUInt16(buffer, 10);
                        break;
                    case "NAME":
                        NAME = ReadZString(bs);
                        bytesRead += NAME.Length + 1;
                        break;
                    case "PGRP":
                        PGRP = new List<PathgridPoint>();
                        var pgrpBytes = 0;
                        while (pgrpBytes < fieldSize)
                        {
                            pgrpBytes += bs.Read(buffer, 0, 16);
                            var newPoint = new PathgridPoint
                            {
                                X = BitConverter.ToInt32(buffer),
                                Y = BitConverter.ToInt32(buffer, 4),
                                Z = BitConverter.ToInt32(buffer, 8),
                                Flags = buffer[12],
                                ConnectionCount = buffer[13]
                                // Last 2 bytes are junk
                            };
                            PGRP.Add(newPoint);
                        }
                        bytesRead += pgrpBytes;
                        break;
                    case "PGRC":
                        PGRC = new List<uint>();
                        var pgrcBytes = 0;
                        while (pgrcBytes < fieldSize)
                        {
                            pgrcBytes += bs.Read(buffer, 0, 4);
                            PGRC.Add(BitConverter.ToUInt32(buffer));
                        }
                        bytesRead += pgrcBytes;
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
            RecordSize += 8 + PGRD_DATA.StructSize;
            RecordSize += (uint)(8 + NAME.Length + 1);
            if (PGRP is not null)
            {
                RecordSize += (uint)(8 + PGRP.Count * 16);
            }
            if (PGRC is not null)
            {
                RecordSize += (uint)(8 + PGRC.Count * 4);
            }
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            DATA.Write(ts);
            ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length + 1));
            ts.Write(EncodeZString(NAME));
            if (PGRP is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("PGRP"));
                ts.Write(BitConverter.GetBytes(PGRP.Count * 16));
                foreach (var point in PGRP)
                {
                    ts.Write(BitConverter.GetBytes(point.X));
                    ts.Write(BitConverter.GetBytes(point.Y));
                    ts.Write(BitConverter.GetBytes(point.Z));
                    ts.WriteByte(point.Flags);
                    ts.WriteByte(point.ConnectionCount);
                    ts.WriteByte(0);
                    ts.WriteByte(0); // Junk bytes
                }
            }
            if (PGRC is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("PGRC"));
                ts.Write(BitConverter.GetBytes(PGRC.Count * 4));
                foreach (var connection in PGRC)
                {
                    ts.Write(BitConverter.GetBytes(connection));
                }
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
