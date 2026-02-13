using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class NPC_ : Record
    {
        public string NAME { get; set; }
        public string? MODL { get; set; }
        public string? FNAM { get; set; }
        public string? RNAM { get; set; }
        public string? CNAM { get; set; }
        public string? ANAM { get; set; }
        public string? BNAM { get; set; }
        public string? KNAM { get; set; }
        public string? SCRI { get; set; }
        public NPC_NPDT? NPDT { get; set; }
        public uint? FLAG { get; set; }
        public List<NPCO> Items { get; set; } = new List<NPCO>();
        public List<string> Spells { get; set; } = new List<string>();
        public AIDT? AIDT { get; set; }
        public List<TravelDestination> Destinations { get; set; } = new List<TravelDestination>();
        public List<AIPackage> AIPackages { get; set; } = new List<AIPackage>();

        public override void Populate(BufferedStream bs)
        {
            AILocationPackage? lastLocPackage = null;
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
                    case "RNAM":
                        RNAM = ReadZString(bs);
                        bytesRead += RNAM.Length + 1;
                        break;
                    case "CNAM":
                        CNAM = ReadZString(bs);
                        bytesRead += CNAM.Length + 1;
                        break;
                    case "ANAM":
                        ANAM = ReadZString(bs);
                        bytesRead += ANAM.Length + 1;
                        break;
                    case "BNAM":
                        BNAM = ReadZString(bs);
                        bytesRead += BNAM.Length + 1;
                        break;
                    case "KNAM":
                        KNAM = ReadZString(bs);
                        bytesRead += KNAM.Length + 1;
                        break;
                    case "SCRI":
                        SCRI = ReadZString(bs);
                        bytesRead += SCRI.Length + 1;
                        break;
                    case "NPDT":
                        if (fieldSize == 12)
                        {
                            NPDT = new NPC_NPDT();
                            bytesRead += bs.Read(buffer, 0, 12);
                            NPDT.Level = BitConverter.ToUInt16(buffer);
                            NPDT.Disposition = buffer[2];
                            NPDT.Reputation = buffer[3];
                            NPDT.Rank = buffer[4];
                            // 3 junk bytes
                            NPDT.Gold = BitConverter.ToUInt32(buffer, 8);
                        }
                        else if (fieldSize == 52)
                        {
                            bytesRead += bs.Read(buffer, 0, 52);
                            NPDT = new NPC_NPDT_EXT
                            {
                                Level = BitConverter.ToUInt16(buffer),
                                Attribute1 = buffer[2],
                                Attribute2 = buffer[3],
                                Attribute3 = buffer[4],
                                Attribute4 = buffer[5],
                                Attribute5 = buffer[6],
                                Attribute6 = buffer[7],
                                Attribute7 = buffer[8],
                                Attribute8 = buffer[9],
                                Skills = buffer[10..37],
                                // Junk byte
                                Health = BitConverter.ToUInt16(buffer, 38),
                                Magicka = BitConverter.ToUInt16(buffer, 40),
                                Fatigue = BitConverter.ToUInt16(buffer, 42),
                                Disposition = buffer[44],
                                Reputation = buffer[45],
                                Rank = buffer[46],
                                // Junk byte
                                Gold = BitConverter.ToUInt32(buffer, 48),
                            };
                        }
                        else
                        {
                            throw new Exception("Unknown NPDT format for NPC_");
                        }

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
                    case "NPCS":
                        bytesRead += bs.Read(buffer, 0, 32);
                        Spells.Add(Encoding.GetString(buffer, 0, 32).TrimEnd('\0'));
                        break;
                    case "AIDT":
                        bytesRead += bs.Read(buffer, 0, 12);
                        AIDT = new AIDT();
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
                        var existingDestination = Destinations.LastOrDefault() ?? throw new Exception($"DNAM before DODT in NPC_");
                        existingDestination.Cell = ReadZString(bs);
                        bytesRead += existingDestination.Cell.Length + 1;
                        break;
                    case "AI_A":
                        bytesRead += bs.Read(buffer, 0, 33);
                        AIPackages.Add(new AI_A
                        {
                            ID = Encoding.GetString(buffer, 0, 32).TrimEnd('\0')
                        });
                        // Remaining byte is junk
                        break;
                    case "AI_E":
                        bytesRead += bs.Read(buffer, 0, 48);
                        var newAIEscort = new AI_E
                        {
                            DestX = BitConverter.ToSingle(buffer),
                            DestY = BitConverter.ToSingle(buffer, 4),
                            DestZ = BitConverter.ToSingle(buffer, 8),
                            Duration = BitConverter.ToUInt16(buffer, 12),
                            ID = Encoding.GetString(buffer, 14, 32).TrimEnd('\0')
                        };
                        AIPackages.Add(newAIEscort);
                        lastLocPackage = newAIEscort;
                        // Remaining 2 bytes are junk
                        break;
                    case "AI_F":
                        bytesRead += bs.Read(buffer, 0, 48);
                        var newAIFollow = new AI_F
                        {
                            DestX = BitConverter.ToSingle(buffer),
                            DestY = BitConverter.ToSingle(buffer, 4),
                            DestZ = BitConverter.ToSingle(buffer, 8),
                            Duration = BitConverter.ToUInt16(buffer, 12),
                            ID = Encoding.GetString(buffer, 14, 32).TrimEnd('\0')
                        };
                        AIPackages.Add(newAIFollow);
                        lastLocPackage = newAIFollow;
                        // Remaining 2 bytes are junk
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
                        if (lastLocPackage is null)
                        {
                            throw new Exception($"CNDT before AI_E or AI_F in NPC_");
                        }
                        lastLocPackage.Cell = ReadZString(bs);
                        bytesRead += lastLocPackage.Cell.Length + 1;
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
                    Console.Error.WriteLine($"Aborting: new NPC ID \"{newID}\" is {newID.Length} characters (max 23)");
                    Environment.Exit(3);
                }
                NAME = newID;
                Updated = true;
            }
            RNAM = ReplaceID(RNAM, oldID, newID);
            CNAM = ReplaceID(CNAM, oldID, newID);
            ANAM = ReplaceID(ANAM, oldID, newID);
            BNAM = ReplaceID(BNAM, oldID, newID);
            KNAM = ReplaceID(KNAM, oldID, newID);
            SCRI = ReplaceID(SCRI, oldID, newID);
            // TODO: set Updated on all these
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
            if (FNAM is not null)
            {
                RecordSize += (uint)(8 + FNAM.Length + 1);
            }
            if (RNAM is not null)
            {
                RecordSize += (uint)(8 + RNAM.Length + 1);
            }
            if (CNAM is not null)
            {
                RecordSize += (uint)(8 + CNAM.Length + 1);
            }
            if (ANAM is not null)
            {
                RecordSize += (uint)(8 + ANAM.Length + 1);
            }
            if (BNAM is not null)
            {
                RecordSize += (uint)(8 + BNAM.Length + 1);
            }
            if (KNAM is not null)
            {
                RecordSize += (uint)(8 + KNAM.Length + 1);
            }
            if (SCRI is not null)
            {
                RecordSize += (uint)(8 + SCRI.Length + 1);
            }
            if (NPDT is not null)
            {
                RecordSize += 8 + NPDT.GetStructSize();
            }
            if (FLAG.HasValue)
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
            if (RNAM is not null)
            {
                ts.Write(Encoding.GetBytes("RNAM"));
                ts.Write(BitConverter.GetBytes(RNAM.Length + 1));
                ts.Write(EncodeZString(RNAM));
            }
            if (CNAM is not null)
            {
                ts.Write(Encoding.GetBytes("CNAM"));
                ts.Write(BitConverter.GetBytes(CNAM.Length + 1));
                ts.Write(EncodeZString(CNAM));
            }
            if (ANAM is not null)
            {
                ts.Write(Encoding.GetBytes("ANAM"));
                ts.Write(BitConverter.GetBytes(ANAM.Length + 1));
                ts.Write(EncodeZString(ANAM));
            }
            if (BNAM is not null)
            {
                ts.Write(Encoding.GetBytes("BNAM"));
                ts.Write(BitConverter.GetBytes(BNAM.Length + 1));
                ts.Write(EncodeZString(BNAM));
            }
            if (KNAM is not null)
            {
                ts.Write(Encoding.GetBytes("KNAM"));
                ts.Write(BitConverter.GetBytes(KNAM.Length + 1));
                ts.Write(EncodeZString(KNAM));
            }
            if (SCRI is not null)
            {
                ts.Write(Encoding.GetBytes("SCRI"));
                ts.Write(BitConverter.GetBytes(SCRI.Length + 1));
                ts.Write(EncodeZString(SCRI));
            }
            if (NPDT is not null)
            {
                NPDT.Write(ts);
            }
            if (FLAG.HasValue)
            {
                ts.Write(Encoding.GetBytes("FLAG"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(FLAG.Value));
            }
            foreach (var item in Items)
            {
                item.Write(ts);
            }
            foreach (var spell in Spells)
            {
                ts.Write(Encoding.GetBytes("NPCS"));
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
                ts.Write(Encoding.GetBytes("DELE"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(Deleted.Value));
            }
        }
    }
}
