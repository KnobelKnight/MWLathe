using MWLathe.Records;
using System.Text;

namespace MWLathe.Helpers
{
    public class RADT
    {
        public static readonly uint StructSize = 140;
        public int Skill1 { get; set; }
        public int Bonus1 { get; set; }
        public int Skill2 { get; set; }
        public int Bonus2 { get; set; }
        public int Skill3 { get; set; }
        public int Bonus3 { get; set; }
        public int Skill4 { get; set; }
        public int Bonus4 { get; set; }
        public int Skill5 { get; set; }
        public int Bonus5 { get; set; }
        public int Skill6 { get; set; }
        public int Bonus6 { get; set; }
        public int Skill7 { get; set; }
        public int Bonus7 { get; set; }
        public uint MaleStrength { get; set; }
        public uint FemaleStrength { get; set; }
        public uint MaleIntelligence { get; set; }
        public uint FemaleIntelligence { get; set; }
        public uint MaleWillpower { get; set; }
        public uint FemaleWillpower { get; set; }
        public uint MaleAgility { get; set; }
        public uint FemaleAgility { get; set; }
        public uint MaleSpeed { get; set; }
        public uint FemaleSpeed { get; set; }
        public uint MaleEndurance { get; set; }
        public uint FemaleEndurance { get; set; }
        public uint MalePersonality { get; set; }
        public uint FemalePersonality { get; set; }
        public uint MaleLuck { get; set; }
        public uint FemaleLuck { get; set; }
        public float MaleHeight { get; set; }
        public float FemaleHeight { get; set; }
        public float MaleWeight { get; set; }
        public float FemaleWeight { get; set; }
        public uint Flags { get; set; }

        public void Write(FileStream ts)
        {
            ts.Write(Record.Encoding.GetBytes("RADT"));
            ts.Write(BitConverter.GetBytes(StructSize));
            ts.Write(BitConverter.GetBytes(Skill1));
            ts.Write(BitConverter.GetBytes(Bonus1));
            ts.Write(BitConverter.GetBytes(Skill2));
            ts.Write(BitConverter.GetBytes(Bonus2));
            ts.Write(BitConverter.GetBytes(Skill3));
            ts.Write(BitConverter.GetBytes(Bonus3));
            ts.Write(BitConverter.GetBytes(Skill4));
            ts.Write(BitConverter.GetBytes(Bonus4));
            ts.Write(BitConverter.GetBytes(Skill5));
            ts.Write(BitConverter.GetBytes(Bonus5));
            ts.Write(BitConverter.GetBytes(Skill6));
            ts.Write(BitConverter.GetBytes(Bonus6));
            ts.Write(BitConverter.GetBytes(Skill7));
            ts.Write(BitConverter.GetBytes(Bonus7));
            ts.Write(BitConverter.GetBytes(MaleStrength));
            ts.Write(BitConverter.GetBytes(FemaleStrength));
            ts.Write(BitConverter.GetBytes(MaleIntelligence));
            ts.Write(BitConverter.GetBytes(FemaleIntelligence));
            ts.Write(BitConverter.GetBytes(MaleWillpower));
            ts.Write(BitConverter.GetBytes(FemaleWillpower));
            ts.Write(BitConverter.GetBytes(MaleAgility));
            ts.Write(BitConverter.GetBytes(FemaleAgility));
            ts.Write(BitConverter.GetBytes(MaleSpeed));
            ts.Write(BitConverter.GetBytes(FemaleSpeed));
            ts.Write(BitConverter.GetBytes(MaleEndurance));
            ts.Write(BitConverter.GetBytes(FemaleEndurance));
            ts.Write(BitConverter.GetBytes(MalePersonality));
            ts.Write(BitConverter.GetBytes(FemalePersonality));
            ts.Write(BitConverter.GetBytes(MaleLuck));
            ts.Write(BitConverter.GetBytes(FemaleLuck));
            ts.Write(BitConverter.GetBytes(MaleHeight));
            ts.Write(BitConverter.GetBytes(FemaleHeight));
            ts.Write(BitConverter.GetBytes(MaleWeight));
            ts.Write(BitConverter.GetBytes(FemaleWeight));
            ts.Write(BitConverter.GetBytes(Flags));
        }
    }
}
