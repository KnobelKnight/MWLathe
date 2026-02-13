using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class ARMO : Record
    {
        public string NAME { get; set; }
        public string? MODL { get; set; }
        public string? FNAM { get; set; }
        public string? SCRI { get; set; }
        public AODT? AODT { get; set; }
        public string? ITEX { get; set; }
        public List<ModelPart> Parts { get; set; } = new List<ModelPart>();
        public string? ENAM { get; set; }

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
                    case "SCRI":
                        SCRI = ReadZString(bs);
                        bytesRead += SCRI.Length + 1;
                        break;
                    case "AODT":
                        AODT = new AODT();
                        bytesRead += bs.Read(buffer, 0, 24);
                        AODT.Type = BitConverter.ToUInt32(buffer);
                        AODT.Weight = BitConverter.ToSingle(buffer, 4);
                        AODT.Value = BitConverter.ToUInt32(buffer, 8);
                        AODT.Health = BitConverter.ToUInt32(buffer, 12);
                        AODT.EnchantCapacity = BitConverter.ToUInt32(buffer, 16);
                        AODT.Rating = BitConverter.ToUInt32(buffer, 20);
                        break;
                    case "ITEX":
                        ITEX = ReadZString(bs);
                        bytesRead += ITEX.Length + 1;
                        break;
                    case "INDX":
                        var newPart = new ModelPart();
                        bytesRead += bs.Read(buffer, 0, 1);
                        newPart.BipedObject = buffer[0];
                        Parts.Add(newPart);
                        break;
                    case "BNAM":
                        var existingPart = Parts.LastOrDefault() ?? throw new Exception($"BNAM before INDX in ARMO");
                        existingPart.MaleModel = ReadStringField(bs, fieldSize);
                        bytesRead += (int)fieldSize;
                        break;
                    case "CNAM":
                        var previousPart = Parts.LastOrDefault() ?? throw new Exception($"CNAM before INDX in ARMO");
                        previousPart.FemaleModel = ReadStringField(bs, fieldSize);
                        bytesRead += (int)fieldSize;
                        break;
                    case "ENAM":
                        ENAM = ReadZString(bs);
                        bytesRead += ENAM.Length + 1;
                        break;
                    default:
                        throw new Exception($"Unknown {GetType().Name} field \"{fieldType}\"");
                }
            }
        }

        public override void UpdateID(string oldID, string newID)
        {
            NAME = ReplaceID(NAME, oldID, newID);
            SCRI = ReplaceID(SCRI, oldID, newID);
            ENAM = ReplaceID(ENAM, oldID, newID);
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
            if (SCRI is not null)
            {
                RecordSize += (uint)(8 + SCRI.Length + 1);
            }
            if (AODT is not null)
            {
                RecordSize += AODT.StructSize + 8;
            }
            if (ITEX is not null)
            {
                RecordSize += (uint)(8 + ITEX.Length + 1);
            }
            RecordSize += (uint)Parts.Sum(x => x.GetByteSize());
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
            if (SCRI is not null)
            {
                ts.Write(Encoding.GetBytes("SCRI"));
                ts.Write(BitConverter.GetBytes(SCRI.Length + 1));
                ts.Write(EncodeZString(SCRI));
            }
            if (AODT is not null)
            {
                AODT.Write(ts);
            }
            if (ITEX is not null)
            {
                ts.Write(Encoding.GetBytes("ITEX"));
                ts.Write(BitConverter.GetBytes(ITEX.Length + 1));
                ts.Write(EncodeZString(ITEX));
            }
            foreach (var part in Parts)
            {
                part.Write(ts);
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
