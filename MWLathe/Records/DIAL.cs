using System.Text;

namespace MWLathe.Records
{
    public class DIAL : Record
    {
        public string NAME { get; set; }
        public byte DATA { get; set; }

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
                    case "DATA":
                        // Can be 4 bytes in some rare cases
                        if (fieldSize == 4)
                        {
                            bytesRead += bs.Read(buffer, 0, 4);
                        }
                        else if (fieldSize == 1)
                        {
                            bytesRead += bs.Read(buffer, 0, 1);
                        }
                        else
                        {
                            throw new Exception("Unkown DATA format in DIAL");
                        }
                        DATA = buffer[0];
                        break;
                    default:
                        throw new Exception($"Unknown {GetType().Name} field \"{fieldType}\"");
                }
            }
        }

        public override void UpdateID(string oldID, string newID)
        {
            if (DATA == 4 && NAME.Equals(oldID, StringComparison.OrdinalIgnoreCase))
            {
                NAME = newID;
                Updated = true;
            }
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + NAME.Length + 1);
            RecordSize += 8 + 1; // DATA
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length + 1));
            ts.Write(EncodeZString(NAME));
            ts.Write(Encoding.GetBytes("DATA"));
            ts.Write(BitConverter.GetBytes(1));
            ts.WriteByte(DATA);
            if (Deleted.HasValue)
            {
                ts.Write(Encoding.GetBytes("DELE"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(Deleted.Value));
            }
        }
    }
}
