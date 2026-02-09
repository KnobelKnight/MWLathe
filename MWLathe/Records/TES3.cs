using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class TES3 : Record
    {
        public HEDR HEDR { get; set; } = new HEDR();
        public List<Master> Masters { get; set; } = new List<Master>();

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
                    case "HEDR":
                        bytesRead += bs.Read(buffer, 0, 40);
                        HEDR.Version = BitConverter.ToSingle(buffer);
                        HEDR.Flags = BitConverter.ToUInt32(buffer, 4);
                        HEDR.Developer = Encoding.GetEncoding("Windows-1252").GetString(buffer, 8, 32);
                        bytesRead += bs.Read(buffer, 0, 256);
                        HEDR.Description = Encoding.GetEncoding("Windows-1252").GetString(buffer);
                        bytesRead += bs.Read(buffer, 0, 4);
                        HEDR.TotalRecords = BitConverter.ToUInt32(buffer);
                        break;
                    case "MAST":
                        var masterName = ReadZString(bs);
                        bytesRead += masterName.Length + 1;
                        // Data subfield
                        bytesRead += bs.Read(buffer, 0, 4);
                        fieldType = Encoding.GetEncoding("Windows-1252").GetString(buffer, 0, 4);
                        if (fieldType != "DATA")
                        {
                            throw new Exception($"Expected field \"DATA\", got field \"{fieldType}\"");
                        }
                        bytesRead += bs.Read(buffer, 0, 12);
                        fieldSize = BitConverter.ToUInt32(buffer);
                        var masterSize = BitConverter.ToUInt64(buffer, 4);
                        Masters.Add(new Master
                        {
                            Name = masterName,
                            Size = masterSize
                        });
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
            RecordSize += 8 + HEDR.StructSize;
            RecordSize += (uint)Masters.Sum(x => x.GetByteSize());
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            HEDR.Write(ts);
            foreach (var master in Masters)
            {
                master.Write(ts);
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
