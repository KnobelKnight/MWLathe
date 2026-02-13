using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class REGN : Record
    {
        public string NAME { get; set; }
        public string FNAM { get; set; }
        public WEAT WEAT { get; set; } = new WEAT();
        public string? BNAM { get; set; }
        public RGB CNAM { get; set; } = new RGB();
        public List<(string, byte)> Sounds { get; set; } = new List<(string, byte)>();

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
                    case "FNAM":
                        FNAM = ReadZString(bs);
                        bytesRead += FNAM.Length + 1;
                        break;
                    case "WEAT":
                        // Legacy pre-Bloodmoon weather
                        if (fieldSize == 8)
                        {
                            bytesRead += bs.Read(buffer, 0, 8);
                            WEAT.Clear = buffer[0];
                            WEAT.Cloudy = buffer[1];
                            WEAT.Foggy = buffer[2];
                            WEAT.Overcast = buffer[3];
                            WEAT.Rain = buffer[4];
                            WEAT.Thunder = buffer[5];
                            WEAT.Ash = buffer[6];
                            WEAT.Blight = buffer[7];
                            WEAT.Snow = 0;
                            WEAT.Blizzard = 0;
                        }
                        else
                        {
                            bytesRead += bs.Read(buffer, 0, 10);
                            WEAT.Clear = buffer[0];
                            WEAT.Cloudy = buffer[1];
                            WEAT.Foggy = buffer[2];
                            WEAT.Overcast = buffer[3];
                            WEAT.Rain = buffer[4];
                            WEAT.Thunder = buffer[5];
                            WEAT.Ash = buffer[6];
                            WEAT.Blight = buffer[7];
                            WEAT.Snow = buffer[8];
                            WEAT.Blizzard = buffer[9];
                        }
                        break;
                    case "BNAM":
                        BNAM = ReadZString(bs);
                        bytesRead += BNAM.Length + 1;
                        break;
                    case "CNAM":
                        bytesRead += bs.Read(buffer, 0, 4);
                        CNAM = new RGB
                        {
                            Red = buffer[0],
                            Green = buffer[1],
                            Blue = buffer[2]
                        };
                        break;
                    case "SNAM":
                        bytesRead += bs.Read(buffer, 0, 33);
                        var soundName = Encoding.GetString(buffer, 0, 32).TrimEnd('\0');
                        Sounds.Add((soundName, buffer[32]));
                        break;
                    default:
                        throw new Exception($"Unknown {GetType().Name} field \"{fieldType}\"");
                }
            }
        }

        public override void UpdateID(string oldID, string newID)
        {
            NAME = ReplaceID(NAME, oldID, newID);
            BNAM = ReplaceID(BNAM, oldID, newID);
            // TODO: set Updated here
            Sounds = Sounds.Select(x => x.Item1.Equals(oldID, StringComparison.OrdinalIgnoreCase) ? (newID, x.Item2) : (x.Item1, x.Item2)).ToList();
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + NAME.Length + 1);
            RecordSize += (uint)(8 + FNAM.Length + 1);
            RecordSize += WEAT.StructSize + 8;
            if (BNAM is not null)
            {
                RecordSize += (uint)(8 + BNAM.Length + 1);
            }
            RecordSize += 12; // CNAM
            RecordSize += (uint)Sounds.Count * (8 + 33);
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length + 1));
            ts.Write(EncodeZString(NAME));
            ts.Write(Encoding.GetBytes("FNAM"));
            ts.Write(BitConverter.GetBytes(FNAM.Length + 1));
            ts.Write(EncodeZString(FNAM));
            WEAT.Write(ts);
            if (BNAM is not null)
            {
                ts.Write(Encoding.GetBytes("BNAM"));
                ts.Write(BitConverter.GetBytes(BNAM.Length + 1));
                ts.Write(EncodeZString(BNAM));
            }
            ts.Write(Encoding.GetBytes("CNAM"));
            ts.Write(BitConverter.GetBytes(4));
            CNAM.Write(ts, true);
            foreach (var sound in Sounds)
            {
                ts.Write(Encoding.GetBytes("SNAM"));
                ts.Write(BitConverter.GetBytes(33));
                ts.Write(EncodeChar32(sound.Item1));
                ts.WriteByte(sound.Item2);
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
