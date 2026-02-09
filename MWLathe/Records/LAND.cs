using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class LAND : Record
    {
        public (int, int) INTV { get; set; }
        public uint DATA { get; set; }
        // 65 x 65
        public List<List<LandscapeVertex>>? VNML { get; set; }
        public VHGT? VHGT { get; set; }
        // 9 x 9
        public List<List<byte>>? WNAM { get; set; }
        // 65 x 65
        public List<List<RGB>>? VCLR { get; set; }
        // 16 x 16
        public List<List<ushort>>? VTEX { get; set; }

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
                    case "INTV":
                        bytesRead += bs.Read(buffer, 0, 8);
                        INTV = (BitConverter.ToInt32(buffer), BitConverter.ToInt32(buffer, 4));
                        break;
                    case "DATA":
                        bytesRead += bs.Read(buffer, 0, 4);
                        DATA = BitConverter.ToUInt32(buffer);
                        break;
                    case "VNML":
                        VNML = new List<List<LandscapeVertex>>();
                        for (int i = 0; i < 65; i++)
                        {
                            var innerList = new List<LandscapeVertex>();
                            VNML.Add(innerList);
                            for (int j = 0; j < 65; j++)
                            {
                                bytesRead += bs.Read(buffer, 0, 3);
                                innerList.Add(new LandscapeVertex
                                {
                                    X = (sbyte)buffer[0],
                                    Y = (sbyte)buffer[1],
                                    Z = (sbyte)buffer[2]
                                });
                            }
                        }
                        break;
                    case "VHGT":
                        VHGT = new VHGT();
                        bytesRead += bs.Read(buffer, 0, 4);
                        VHGT.Offset = BitConverter.ToSingle(buffer);
                        VHGT.Data = new List<List<sbyte>>();
                        for (int i = 0; i < 65; i++)
                        {
                            bytesRead += bs.Read(buffer, 0, 65);
                            VHGT.Data.Add(Array.ConvertAll(buffer[..65], x => (sbyte)x).ToList());
                        }
                        // Remaining 3 bytes are junk
                        bytesRead += bs.Read(buffer, 0, 3);
                        break;
                    case "WNAM":
                        WNAM = new List<List<byte>>();
                        for (int i = 0; i < 9; i++)
                        {
                            bytesRead += bs.Read(buffer, 0, 9);
                            WNAM.Add(buffer[..9].ToList());
                        }
                        break;
                    case "VCLR":
                        VCLR = new List<List<RGB>>();
                        for (int i = 0; i < 65; i++)
                        {
                            var innerList = new List<RGB>();
                            VCLR.Add(innerList);
                            for (int j = 0; j < 65; j++)
                            {
                                bytesRead += bs.Read(buffer, 0, 3);
                                innerList.Add(new RGB
                                {
                                    Red = buffer[0],
                                    Green = buffer[1],
                                    Blue = buffer[2]
                                });
                            }
                        }
                        break;
                    case "VTEX":
                        VTEX = new List<List<ushort>>();
                        for (int i = 0; i < 16; i++)
                        {
                            bytesRead += bs.Read(buffer, 0, 32);
                            var shortArray = new ushort[16];
                            Buffer.BlockCopy(buffer[..32], 0, shortArray, 0, 32);
                            VTEX.Add(shortArray.ToList());
                        }
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
            RecordSize += 8 + 8; // INTV
            RecordSize += 8 + 4; // DATA
            if (VNML is not null)
            {
                RecordSize += 8 + 12675;
            }
            if (VHGT is not null)
            {
                RecordSize += 8 + VHGT.StructSize;
            }
            if (WNAM is not null)
            {
                RecordSize += 8 + 81;
            }
            if (VCLR is not null)
            {
                RecordSize += 8 + 12675;
            }
            if (VTEX is not null)
            {
                RecordSize += 8 + 512;
            }
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("INTV"));
            ts.Write(BitConverter.GetBytes(8));
            ts.Write(BitConverter.GetBytes(INTV.Item1));
            ts.Write(BitConverter.GetBytes(INTV.Item2));
            ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("DATA"));
            ts.Write(BitConverter.GetBytes(4));
            ts.Write(BitConverter.GetBytes(DATA));
            if (VNML is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("VNML"));
                ts.Write(BitConverter.GetBytes(12675)); // VNML struct size (65 * 65 * 3)
                foreach (var innerList in VNML)
                {
                    foreach (var vertex in innerList)
                    {
                        ts.WriteByte((byte)vertex.X);
                        ts.WriteByte((byte)vertex.Y);
                        ts.WriteByte((byte)vertex.Z);
                    }
                }
            }
            if (VHGT is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("VHGT"));
                ts.Write(BitConverter.GetBytes(VHGT.StructSize));
                ts.Write(BitConverter.GetBytes(VHGT.Offset));
                foreach (var innerList in VHGT.Data)
                {
                    foreach (var height in innerList)
                    {
                        ts.WriteByte((byte)height);
                    }
                }
                ts.WriteByte(0);
                ts.WriteByte(0);
                ts.WriteByte(0); // Junk values
            }
            if (WNAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("WNAM"));
                ts.Write(BitConverter.GetBytes(81)); // WNAM struct size
                foreach (var innerList in WNAM)
                {
                    foreach (var mapHeight in innerList)
                    {
                        ts.WriteByte(mapHeight);
                    }
                }
            }
            if (VCLR is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("VCLR"));
                ts.Write(BitConverter.GetBytes(12675)); // VCLR struct size (65 * 65 * 3)
                foreach (var innerList in VCLR)
                {
                    foreach (var rgb in innerList)
                    {
                        rgb.Write(ts, false);
                    }
                }
            }
            if (VTEX is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("VTEX"));
                ts.Write(BitConverter.GetBytes(512)); // VTEX struct size (16 * 16 * 2)
                foreach (var innerList in VTEX)
                {
                    foreach (var index in innerList)
                    {
                        ts.Write(BitConverter.GetBytes(index));
                    }
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
