using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class FADT
    {
        public static readonly uint StructSize = 240;
        public uint Attribute1 { get; set; }
        public uint Attribute2 { get; set; }
        public List<RankData> RankData { get; set; } = new List<RankData>();
        public int Skill1 { get; set; } = -1;
        public int Skill2 { get; set; } = -1;
        public int Skill3 { get; set; } = -1;
        public int Skill4 { get; set; } = -1;
        public int Skill5 { get; set; } = -1;
        public int Skill6 { get; set; } = -1;
        public int Skill7 { get; set; } = -1;
        public uint Flags { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("FADT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Attribute1));
            ts.Write(BitConverter.GetBytes(Attribute2));
            foreach (var rankData in RankData)
            {
                rankData.Write(ts);
            }
            ts.Write(BitConverter.GetBytes(Skill1));
            ts.Write(BitConverter.GetBytes(Skill2));
            ts.Write(BitConverter.GetBytes(Skill3));
            ts.Write(BitConverter.GetBytes(Skill4));
            ts.Write(BitConverter.GetBytes(Skill5));
            ts.Write(BitConverter.GetBytes(Skill6));
            ts.Write(BitConverter.GetBytes(Skill7));
            ts.Write(BitConverter.GetBytes(Flags));
        }
    }
}
