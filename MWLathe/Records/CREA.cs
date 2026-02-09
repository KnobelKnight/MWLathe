using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class CREA : Record
    {
        public string NAME { get; set; }
        public string? MODL { get; set; }
        public string? CNAM { get; set; }
        public string? FNAM { get; set; }
        public string? SCRI { get; set; }
        public CREA_NPDT? NPDT { get; set; }
        public uint? FLAG { get; set; }
        public float? XSCL { get; set; }
        public List<NPCO> Items { get; set; } = new List<NPCO>();
        public List<string> Spells { get; set; } = new List<string>();
        public AIDT? AIDT { get; set; }
        public List<TravelDestination> Destinations { get; set; } = new List<TravelDestination>();
        public List<AIPackage> AIPackages { get; set; } = new List<AIPackage>();

        public override void Populate(BufferedStream bs)
        {
            base.Populate(bs);
            byte[] buffer = new byte[256];
            int bytesRead = 0;

            AILocationPackage? lastLocationPackage = null;
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
                    case "MODL":
                        MODL = ReadZString(bs);
                        bytesRead += MODL.Length + 1;
                        break;
                    case "CNAM":
                        CNAM = ReadZString(bs);
                        bytesRead += CNAM.Length + 1;
                        break;
                    case "FNAM":
                        FNAM = ReadZString(bs);
                        bytesRead += FNAM.Length + 1;
                        break;
                    case "SCRI":
                        SCRI = ReadZString(bs);
                        bytesRead += SCRI.Length + 1;
                        break;
                    case "NPDT":
                        NPDT = new CREA_NPDT();
                        bytesRead += bs.Read(buffer, 0, 96);
                        NPDT.Type = BitConverter.ToUInt32(buffer);
                        NPDT.Level = BitConverter.ToUInt32(buffer, 4);
                        NPDT.Attribute1 = BitConverter.ToUInt32(buffer, 8);
                        NPDT.Attribute2 = BitConverter.ToUInt32(buffer, 12);
                        NPDT.Attribute3 = BitConverter.ToUInt32(buffer, 16);
                        NPDT.Attribute4 = BitConverter.ToUInt32(buffer, 20);
                        NPDT.Attribute5 = BitConverter.ToUInt32(buffer, 24);
                        NPDT.Attribute6 = BitConverter.ToUInt32(buffer, 28);
                        NPDT.Attribute7 = BitConverter.ToUInt32(buffer, 32);
                        NPDT.Attribute8 = BitConverter.ToUInt32(buffer, 36);
                        NPDT.Health = BitConverter.ToUInt32(buffer, 40);
                        NPDT.Magicka = BitConverter.ToUInt32(buffer, 44);
                        NPDT.Fatigue = BitConverter.ToUInt32(buffer, 48);
                        NPDT.SoulSize = BitConverter.ToUInt32(buffer, 52);
                        NPDT.Combat = BitConverter.ToUInt32(buffer, 56);
                        NPDT.Magic = BitConverter.ToUInt32(buffer, 60);
                        NPDT.Stealth = BitConverter.ToUInt32(buffer, 64);
                        NPDT.Attack1Min = BitConverter.ToUInt32(buffer, 68);
                        NPDT.Attack1Max = BitConverter.ToUInt32(buffer, 72);
                        NPDT.Attack2Min = BitConverter.ToUInt32(buffer, 76);
                        NPDT.Attack2Max = BitConverter.ToUInt32(buffer, 80);
                        NPDT.Attack3Min = BitConverter.ToUInt32(buffer, 84);
                        NPDT.Attack3Max = BitConverter.ToUInt32(buffer, 88);
                        NPDT.Gold = BitConverter.ToUInt32(buffer, 92);
                        break;
                    case "FLAG":
                        bytesRead += bs.Read(buffer, 0, 4);
                        FLAG = BitConverter.ToUInt32(buffer);
                        break;
                    case "XSCL":
                        bytesRead += bs.Read(buffer, 0, 4);
                        XSCL = BitConverter.ToSingle(buffer);
                        break;
                    case "NPCO":
                        bytesRead += bs.Read(buffer, 0, 36);
                        Items.Add(new NPCO
                        {
                            Count = BitConverter.ToInt32(buffer),
                            ID = Encoding.GetEncoding("Windows-1252").GetString(buffer, 4, 32).TrimEnd('\0')
                        });
                        break;
                    case "NPCS":
                        bytesRead += bs.Read(buffer, 0, 32);
                        Spells.Add(Encoding.GetEncoding("Windows-1252").GetString(buffer, 0, 32).TrimEnd('\0'));
                        break;
                    case "AIDT":
                        AIDT = new AIDT();
                        bytesRead += bs.Read(buffer, 0, 12);
                        AIDT.Hello = buffer[0];
                        AIDT.Junk = buffer[1];
                        AIDT.Fight = buffer[2];
                        AIDT.Flee = buffer[3];
                        AIDT.Alarm = buffer[4];
                        // 3 junk bytes
                        AIDT.Flags = BitConverter.ToUInt32(buffer, 8);
                        break;
                    case "DODT":
                        bytesRead += bs.Read(buffer, 0, 24);
                        var newDestination = new TravelDestination();
                        newDestination.Location.PositionX = BitConverter.ToSingle(buffer);
                        newDestination.Location.PositionY = BitConverter.ToSingle(buffer, 4);
                        newDestination.Location.PositionZ = BitConverter.ToSingle(buffer, 8);
                        newDestination.Location.RotationX = BitConverter.ToSingle(buffer, 12);
                        newDestination.Location.RotationY = BitConverter.ToSingle(buffer, 16);
                        newDestination.Location.RotationZ = BitConverter.ToSingle(buffer, 20);
                        Destinations.Add(newDestination);
                        break;
                    case "DNAM":
                        var existingDestination = Destinations.LastOrDefault() ?? throw new Exception($"DNAM before DODT in CREA");
                        existingDestination.Cell = ReadZString(bs);
                        bytesRead += existingDestination.Cell.Length + 1;
                        break;
                    case "AI_A":
                        bytesRead += bs.Read(buffer, 0, 33);
                        AIPackages.Add(new AI_A
                        {
                            ID = Encoding.GetEncoding("Windows-1252").GetString(buffer, 0, 32).TrimEnd('\0')
                        });
                        // Remaining byte is junk
                        break;
                    case "AI_E":
                        bytesRead += bs.Read(buffer, 0, 48);
                        var newEscort = new AI_E
                        {
                            DestX = BitConverter.ToSingle(buffer),
                            DestY = BitConverter.ToSingle(buffer, 4),
                            DestZ = BitConverter.ToSingle(buffer, 8),
                            Duration = BitConverter.ToUInt16(buffer, 12),
                            ID = Encoding.GetEncoding("Windows-1252").GetString(buffer, 14, 32).TrimEnd('\0')
                        };
                        // Remaining 2 bytes are junk
                        AIPackages.Add(newEscort);
                        lastLocationPackage = newEscort;
                        break;
                    case "AI_F":
                        bytesRead += bs.Read(buffer, 0, 48);
                        var newFollow = new AI_F
                        {
                            DestX = BitConverter.ToSingle(buffer),
                            DestY = BitConverter.ToSingle(buffer, 4),
                            DestZ = BitConverter.ToSingle(buffer, 8),
                            Duration = BitConverter.ToUInt16(buffer, 12),
                            ID = Encoding.GetEncoding("Windows-1252").GetString(buffer, 14, 32).TrimEnd('\0')
                        };
                        // Remaining 2 bytes are junk
                        AIPackages.Add(newFollow);
                        lastLocationPackage = newFollow;
                        break;
                    case "AI_T":
                        bytesRead += bs.Read(buffer, 0, 16);
                        AIPackages.Add(new AI_T
                        {
                            DestX = BitConverter.ToSingle(buffer),
                            DestY = BitConverter.ToSingle(buffer, 4),
                            DestZ = BitConverter.ToSingle(buffer, 8)
                        });
                        // Remaining 4 bytes are junk
                        break;
                    case "AI_W":
                        bytesRead += bs.Read(buffer, 0, 14);
                        AIPackages.Add(new AI_W
                        {
                            Distance = BitConverter.ToUInt16(buffer),
                            Duration = BitConverter.ToUInt16(buffer, 2),
                            Time = buffer[4],
                            Idle2 = buffer[5],
                            Idle3 = buffer[6],
                            Idle4 = buffer[7],
                            Idle5 = buffer[8],
                            Idle6 = buffer[9],
                            Idle7 = buffer[10],
                            Idle8 = buffer[11],
                            Idle9 = buffer[12]
                        });
                        // Remaining byte is junk
                        break;
                    case "CNDT":
                        var newCell = ReadZString(bs);
                        bytesRead += newCell.Length + 1;
                        (lastLocationPackage ?? throw new Exception($"CNDT before AI_E or AI_F in CREA")).Cell = newCell;
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
                    Console.Error.WriteLine($"Aborting: new creature ID \"{newID}\" is {newID.Length} characters (max 23)");
                    Environment.Exit(3);
                }
                NAME = newID;
                Updated = true;
            }
            CNAM = ReplaceID(CNAM, oldID, newID);
            SCRI = ReplaceID(SCRI, oldID, newID);
            // TODO: set Updated for all these
            foreach (var item in Items.Where(x => x.ID.Equals(oldID, StringComparison.OrdinalIgnoreCase)))
            {
                item.ID = newID;
            }
            Spells = Spells.Select(x => x.Equals(oldID, StringComparison.OrdinalIgnoreCase) ? newID : x).ToList();
            foreach (var aiPackage in AIPackages)
            {
                aiPackage.UpdateID(oldID, newID);
            }
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + NAME.Length + 1);
            if (MODL is not null)
            {
                RecordSize += (uint)(8 + MODL.Length + 1);
            }
            if (CNAM is not null)
            {
                RecordSize += (uint)(8 + CNAM.Length + 1);
            }
            if (FNAM is not null)
            {
                RecordSize += (uint)(8 + FNAM.Length + 1);
            }
            if (SCRI is not null)
            {
                RecordSize += (uint)(8 + SCRI.Length + 1);
            }
            if (NPDT is not null)
            {
                RecordSize += CREA_NPDT.StructSize + 8;
            }
            if (FLAG.HasValue)
            {
                RecordSize += 12;
            }
            if (XSCL.HasValue)
            {
                RecordSize += 12;
            }
            RecordSize += (uint)Items.Count * (NPCO.StructSize + 8);
            RecordSize += (uint)(40 * Spells.Count);
            if (AIDT is not null)
            {
                RecordSize += AIDT.StructSize + 8;
            }
            RecordSize += (uint)Destinations.Sum(x => x.GetByteSize());
            RecordSize += (uint)AIPackages.Sum(x => x.GetByteSize());
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length + 1));
            ts.Write(EncodeZString(NAME));
            if (MODL is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("MODL"));
                ts.Write(BitConverter.GetBytes(MODL.Length + 1));
                ts.Write(EncodeZString(MODL));
            }
            if (CNAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("CNAM"));
                ts.Write(BitConverter.GetBytes(CNAM.Length + 1));
                ts.Write(EncodeZString(CNAM));
            }
            if (FNAM is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("FNAM"));
                ts.Write(BitConverter.GetBytes(FNAM.Length + 1));
                ts.Write(EncodeZString(FNAM));
            }
            if (SCRI is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("SCRI"));
                ts.Write(BitConverter.GetBytes(SCRI.Length + 1));
                ts.Write(EncodeZString(SCRI));
            }
            if (NPDT is not null)
            {
                NPDT.Write(ts);
            }
            if (FLAG.HasValue)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("FLAG"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(FLAG.Value));
            }
            if (XSCL.HasValue)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("XSCL"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(XSCL.Value));
            }
            foreach (var item in Items)
            {
                item.Write(ts);
            }
            foreach (var spell in Spells)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("NPCS"));
                ts.Write(BitConverter.GetBytes(32));
                ts.Write(EncodeChar32(spell));
            }
            if (AIDT is not null)
            {
                AIDT.Write(ts);
            }
            foreach (var destination in Destinations)
            {
                destination.Write(ts);
            }
            foreach (var aiPackage in AIPackages)
            {
                aiPackage.Write(ts);
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
