using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class WEAP : Record
    {
        public string NAME { get; set; }
        public string? MODL { get; set; }
        public string? FNAM { get; set; }
        public WPDT? WPDT { get; set; }
        public string? ITEX { get; set; }
        public string? ENAM { get; set; }
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
                    case "WPDT":
                        WPDT = new WPDT();
                        bytesRead += bs.Read(buffer, 0, 32);
                        WPDT.Weight = BitConverter.ToSingle(buffer);
                        WPDT.Value = BitConverter.ToUInt32(buffer, 4);
                        WPDT.Type = BitConverter.ToUInt16(buffer, 8);
                        WPDT.Health = BitConverter.ToUInt16(buffer, 10);
                        WPDT.Speed = BitConverter.ToSingle(buffer, 12);
                        WPDT.Reach = BitConverter.ToSingle(buffer, 16);
                        WPDT.EnchantCapacity = BitConverter.ToUInt16(buffer, 20);
                        WPDT.ChopMin = buffer[22];
                        WPDT.ChopMax = buffer[23];
                        WPDT.SlashMin = buffer[24];
                        WPDT.SlashMax = buffer[25];
                        WPDT.ThrustMin = buffer[26];
                        WPDT.ThrustMax = buffer[27];
                        WPDT.Flags = BitConverter.ToUInt32(buffer, 28);
                        break;
                    case "ITEX":
                        ITEX = ReadZString(bs);
                        bytesRead += ITEX.Length + 1;
                        break;
                    case "ENAM":
                        ENAM = ReadZString(bs);
                        bytesRead += ENAM.Length + 1;
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
            ENAM = ReplaceID(ENAM, oldID, newID);
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
            if (WPDT is not null)
            {
                RecordSize += WPDT.StructSize + 8;
            }
            if (SCRI is not null)
            {
                RecordSize += (uint)(8 + SCRI.Length + 1);
            }
            if (ITEX is not null)
            {
                RecordSize += (uint)(8 + ITEX.Length + 1);
            }
            if (ENAM is not null)
            {
                RecordSize += (uint)(8 + ENAM.Length + 1);
            }
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
            if (WPDT is not null)
            {
                WPDT.Write(ts);
            }
            if (SCRI is not null)
            {
                ts.Write(Encoding.GetBytes("SCRI"));
                ts.Write(BitConverter.GetBytes(SCRI.Length + 1));
                ts.Write(EncodeZString(SCRI));
            }
            if (ITEX is not null)
            {
                ts.Write(Encoding.GetBytes("ITEX"));
                ts.Write(BitConverter.GetBytes(ITEX.Length + 1));
                ts.Write(EncodeZString(ITEX));
            }
            if (ENAM is not null)
            {
                ts.Write(Encoding.GetBytes("ENAM"));
                ts.Write(BitConverter.GetBytes(ENAM.Length + 1));
                ts.Write(EncodeZString(ENAM));
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
