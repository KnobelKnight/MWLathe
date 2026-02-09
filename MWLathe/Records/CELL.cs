using MWLathe.Helpers;
using System.Text;

namespace MWLathe.Records
{
    public class CELL : Record
    {
        public string NAME { get; set; }
        public CELL_DATA DATA { get; set; } = new CELL_DATA();
        public string? RGNN { get; set; }
        public RGB? NAM5 { get; set; }
        public float? WHGT { get; set; }
        public AMBI? AMBI { get; set; }
        public List<MovedReference> MovedReferences { get; set; } = new List<MovedReference>();
        public List<FormReference> PersistentChildren { get; set; } = new List<FormReference>();
        public uint? NAM0 { get; set; }
        public List<FormReference> TemporaryChildren { get; set; } = new List<FormReference>();

        public override void Populate(BufferedStream bs)
        {
            base.Populate(bs);
            byte[] buffer = new byte[256];
            int bytesRead = 0;

            MovedReference? lastMoved = null;
            FormReference? lastForm = null;
            bool movedLatest = false;

            while (bytesRead < RecordSize)
            {
                bytesRead += bs.Read(buffer, 0, 8);
                var fieldType = Encoding.GetEncoding("Windows-1252").GetString(buffer, 0, 4);
                var fieldSize = BitConverter.ToUInt32(buffer, 4);
                switch (fieldType)
                {
                    case "DELE":
                        bytesRead += bs.Read(buffer, 0, 4);
                        if (lastForm is not null)
                        {
                            lastForm.Deleted = BitConverter.ToUInt32(buffer);
                        }
                        else
                        {
                            Deleted = BitConverter.ToUInt32(buffer);
                        }
                        break;
                    case "NAME":
                        var nameData = ReadZString(bs);
                        bytesRead += nameData.Length + 1;
                        if (lastForm == null)
                        {
                            NAME = nameData;
                        }
                        else
                        {
                            lastForm.ObjectID = nameData;
                        }
                        break;
                    case "DATA":
                        if (lastForm == null)
                        {
                            bytesRead += bs.Read(buffer, 0, 12);
                            DATA.Flags = BitConverter.ToUInt32(buffer);
                            DATA.GridX = BitConverter.ToInt32(buffer, 4);
                            DATA.GridY = BitConverter.ToInt32(buffer, 8);
                        }
                        else
                        {
                            bytesRead += bs.Read(buffer, 0, 24);
                            lastForm.ReferencePosition = new DODT
                            {
                                PositionX = BitConverter.ToSingle(buffer),
                                PositionY = BitConverter.ToSingle(buffer, 4),
                                PositionZ = BitConverter.ToSingle(buffer, 8),
                                RotationX = BitConverter.ToSingle(buffer, 12),
                                RotationY = BitConverter.ToSingle(buffer, 16),
                                RotationZ = BitConverter.ToSingle(buffer, 20)
                            };
                        }
                        break;
                    case "RGNN":
                        RGNN = ReadZString(bs);
                        bytesRead += RGNN.Length + 1;
                        break;
                    case "NAM5":
                        bytesRead += bs.Read(buffer, 0, 4);
                        NAM5 = new RGB
                        {
                            Red = buffer[0],
                            Green = buffer[1],
                            Blue = buffer[2],
                        };
                        // Last byte is junk
                        break;
                    case "WHGT":
                        bytesRead += bs.Read(buffer, 0, 4);
                        WHGT = BitConverter.ToSingle(buffer);
                        break;
                    case "AMBI":
                        AMBI = new AMBI();
                        bytesRead += bs.Read(buffer, 0, 16);
                        AMBI.AmbientColor = new RGB
                        {
                            Red = buffer[0],
                            Green = buffer[1],
                            Blue = buffer[2]
                        };
                        // Last byte is junk
                        AMBI.SunColor = new RGB
                        {
                            Red = buffer[4],
                            Green = buffer[5],
                            Blue = buffer[6]
                        };
                        // Last byte is junk
                        AMBI.FogColor = new RGB
                        {
                            Red = buffer[8],
                            Green = buffer[9],
                            Blue = buffer[10]
                        };
                        // Last byte is junk
                        AMBI.FogDensity = BitConverter.ToSingle(buffer, 12);
                        break;
                    case "NAM0":
                        bytesRead += bs.Read(buffer, 0, 4);
                        NAM0 = BitConverter.ToUInt32(buffer);
                        break;
                    case "MVRF":
                        bytesRead += bs.Read(buffer, 0, 4);
                        var newMovedReference = new MovedReference
                        {
                            ID = BitConverter.ToUInt32(buffer)
                        };
                        MovedReferences.Add(newMovedReference);
                        lastMoved = newMovedReference;
                        movedLatest = true;
                        break;
                    case "CNAM":
                        // CNAM can be used for MVRF or FRMR, and contains different data for each
                        var cnamData = ReadZString(bs);
                        bytesRead += cnamData.Length + 1;
                     
                        if (movedLatest)
                        {
                            if (lastMoved is null)
                            {
                                throw new Exception("Internal error: CNAM marked for MVRF, but no MVRF available");
                            }
                            lastMoved.CellName = cnamData;
                        }
                        else 
                        {
                            if (lastForm is null)
                            {
                                throw new Exception("Internal error: CNAM marked for FRMR, but no FRMR available");
                            }
                            lastForm.FactionID = cnamData;
                        }
                        break;
                    case "CNDT":
                        bytesRead += bs.Read(buffer, 0, 8);
                        var previousMoved = lastMoved ?? throw new Exception($"CNDT before MVRF in CELL");
                        previousMoved.GridCoordinates = (BitConverter.ToInt32(buffer), BitConverter.ToInt32(buffer, 4));
                        break;
                    case "FRMR":
                        bytesRead += bs.Read(buffer, 0, 4);
                        var newFormReference = new FormReference
                        {
                            ReferenceID = BitConverter.ToUInt32(buffer)
                        };
                        if (lastMoved is not null && lastMoved.FormMoved is null)
                        {
                            lastMoved.FormMoved = newFormReference;
                            
                        }
                        else if (NAM0 is null)
                        {
                            PersistentChildren.Add(newFormReference);
                        }
                        else
                        {
                            TemporaryChildren.Add(newFormReference);
                        }
                        lastForm = newFormReference;
                        movedLatest = false;
                        break;
                    case "UNAM":
                        bytesRead += bs.Read(buffer, 0, 1);
                        (lastForm ?? throw new Exception($"UNAM before FRMR in CELL")).Blocked = buffer[0];
                        break;
                    case "XSCL":
                        bytesRead += bs.Read(buffer, 0, 4);
                        (lastForm ?? throw new Exception($"XSCL before FRMR in CELL")).Scale = BitConverter.ToSingle(buffer);
                        break;
                    case "ANAM":
                        if (lastForm is null)
                        {
                            throw new Exception($"ANAM before FRMR in CELL");
                        }
                        lastForm.NPCID = ReadZString(bs);
                        bytesRead += lastForm.NPCID.Length + 1;
                        break;
                    case "BNAM":
                        if (lastForm is null)
                        {
                            throw new Exception($"BNAM before FRMR in CELL");
                        }
                        lastForm.OwnershipGlobal = ReadZString(bs);
                        bytesRead += lastForm.OwnershipGlobal.Length + 1;
                        break;
                    case "INDX":
                        bytesRead += bs.Read(buffer, 0, 4);
                        (lastForm ?? throw new Exception($"INDX before FRMR in CELL")).FactionRank = BitConverter.ToUInt32(buffer);
                        break;
                    case "XSOL":
                        if (lastForm is null)
                        {
                            throw new Exception($"XSOL before FRMR in CELL");
                        }
                        lastForm.Soul = ReadZString(bs);
                        bytesRead += lastForm.Soul.Length + 1;
                        break;
                    case "XCHG":
                        bytesRead += bs.Read(buffer, 0, 4);
                        (lastForm ?? throw new Exception($"XCHG before FRMR in CELL")).EnchantCharge = BitConverter.ToSingle(buffer);
                        break;
                    case "INTV":
                        bytesRead += bs.Read(buffer, 0, 4);
                        if (lastForm is null)
                        {
                            // Nonsense INTVs appear before FRMRs in some very old esps. Seems harmless to ignore
                            continue;
                        }
                        lastForm.RemainingUsage = buffer[..4];
                        break;
                    case "NAM9":
                        bytesRead += bs.Read(buffer, 0, 4);
                        (lastForm ?? throw new Exception($"NAM9 before FRMR in CELL")).Value = BitConverter.ToUInt32(buffer);
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
                        (lastForm ?? throw new Exception($"DODT before FRMR in CELL")).Destinations.Add(newDestination);
                        break;
                    case "DNAM":
                        var existingDestination = (lastForm ?? throw new Exception($"DNAM before FRMR in CELL")).Destinations.LastOrDefault() ?? throw new Exception($"DNAM before DODT in FRMR");
                        existingDestination.Cell = ReadZString(bs);
                        bytesRead += existingDestination.Cell.Length + 1;
                        break;
                    case "FLTV":
                        bytesRead += bs.Read(buffer, 0, 4);
                        (lastForm ?? throw new Exception($"FLTV before FRMR in CELL")).LockDifficulty = BitConverter.ToUInt32(buffer);
                        break;
                    case "KNAM":
                        if (lastForm is null)
                        {
                            throw new Exception($"KNAM before FRMR in CELL");
                        }
                        lastForm.KeyName = ReadZString(bs);
                        bytesRead += lastForm.KeyName.Length + 1;
                        break;
                    case "TNAM":
                        if (lastForm is null)
                        {
                            throw new Exception($"TNAM before FRMR in CELL");
                        }
                        lastForm.TrapName = ReadZString(bs);
                        bytesRead += lastForm.TrapName.Length + 1;
                        break;
                    case "ZNAM":
                        bytesRead += bs.Read(buffer, 0, 1);
                        (lastForm ?? throw new Exception($"ZNAM before FRMR in CELL")).Disabled = buffer[0];
                        break;
                    default:
                        throw new Exception($"Unknown {GetType().Name} field \"{fieldType}\"");
                }
            }
        }

        // TODO: make async?
        public override void UpdateID(string oldID, string newID)
        {
            // TODO: set Updated here
            foreach (var movedReference in MovedReferences.Where(x => x.FormMoved is not null))
            {
                movedReference.FormMoved.UpdateID(oldID, newID);
            }
            foreach (var persistentChild in PersistentChildren)
            {
                persistentChild.UpdateID(oldID, newID);
            }
            foreach (var temporaryChild in TemporaryChildren)
            {
                temporaryChild.UpdateID(oldID, newID);
            }
        }

        public override void CalculateRecordSize()
        {
            base.CalculateRecordSize();
            RecordSize += (uint)(8 + NAME.Length + 1);
            RecordSize += CELL_DATA.StructSize + 8;
            if (RGNN is not null)
            {
                RecordSize += (uint)(8 + RGNN.Length + 1);
            }
            if (NAM5 is not null)
            {
                RecordSize += 12;
            }
            if (WHGT.HasValue)
            {
                RecordSize += 12;
            }
            if (AMBI is not null)
            {
                RecordSize += AMBI.StructSize + 8;
            }
            RecordSize += (uint)MovedReferences.Sum(x => x.GetByteSize());
            RecordSize += (uint)PersistentChildren.Sum(x => x.GetByteSize());
            if (NAM0.HasValue)
            {
                RecordSize += 12;
            }
            RecordSize += (uint)TemporaryChildren.Sum(x => x.GetByteSize());
        }

        public override void Write(FileStream ts)
        {
            base.Write(ts);
            ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("NAME"));
            ts.Write(BitConverter.GetBytes(NAME.Length + 1));
            ts.Write(EncodeZString(NAME));
            if (Deleted.HasValue)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("DELE"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(Deleted.Value));
            }
            DATA.Write(ts);
            if (RGNN is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("RGNN"));
                ts.Write(BitConverter.GetBytes(RGNN.Length + 1));
                ts.Write(EncodeZString(RGNN));
            }
            if (NAM5 is not null)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("NAM5"));
                ts.Write(BitConverter.GetBytes(4)); // RGB struct size
                NAM5.Write(ts, true);
            }
            if (WHGT.HasValue)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("WHGT"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(WHGT.Value));
            }
            if (AMBI is not null)
            {
                AMBI.Write(ts);
            }
            foreach (var movedReference in MovedReferences)
            {
                movedReference.Write(ts);
            }
            foreach (var persistentChild in PersistentChildren)
            {
                persistentChild.Write(ts);
            }
            if (NAM0.HasValue)
            {
                ts.Write(Encoding.GetEncoding("Windows-1252").GetBytes("NAM0"));
                ts.Write(BitConverter.GetBytes(4));
                ts.Write(BitConverter.GetBytes(NAM0.Value));
            }
            foreach (var temporaryChild in TemporaryChildren)
            {
                temporaryChild.Write(ts);
            }
        }
    }
}
