using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class CLDT
    {
        public static readonly uint StructSize = 60;
        public uint PrimaryAttribute1 { get; set; }
        public uint PrimaryAttribute2 { get; set; }
        public uint Specialization { get; set; }
        public uint MinorSkill1 { get; set; }
        public uint MajorSkill1 { get; set; }
        public uint MinorSkill2 { get; set; }
        public uint MajorSkill2 { get; set; }
        public uint MinorSkill3 { get; set; }
        public uint MajorSkill3 { get; set; }
        public uint MinorSkill4 { get; set; }
        public uint MajorSkill4 { get; set; }
        public uint MinorSkill5 { get; set; }
        public uint MajorSkill5 { get; set; }
        public uint Playable { get; set; }
        public uint Flags { get; set; }
        public uint AutocalcFlags { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("CLDT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(PrimaryAttribute1));
            ts.Write(BitConverter.GetBytes(PrimaryAttribute2));
            ts.Write(BitConverter.GetBytes(Specialization));
            ts.Write(BitConverter.GetBytes(MinorSkill1));
            ts.Write(BitConverter.GetBytes(MajorSkill1));
            ts.Write(BitConverter.GetBytes(MinorSkill2));
            ts.Write(BitConverter.GetBytes(MajorSkill2));
            ts.Write(BitConverter.GetBytes(MinorSkill3));
            ts.Write(BitConverter.GetBytes(MajorSkill3));
            ts.Write(BitConverter.GetBytes(MinorSkill4));
            ts.Write(BitConverter.GetBytes(MajorSkill4));
            ts.Write(BitConverter.GetBytes(MinorSkill5));
            ts.Write(BitConverter.GetBytes(MajorSkill5));
            ts.Write(BitConverter.GetBytes(Flags));
            ts.Write(BitConverter.GetBytes(AutocalcFlags));
        }
    }
}
