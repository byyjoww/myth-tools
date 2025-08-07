namespace ROTools.Skills
{
    public interface IMobSkillDBBuilder
    {
        string Build(string[] headers, MobSkillData[] data);
    }
}