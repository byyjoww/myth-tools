namespace ROTools.Skills
{
    public interface ISkillDBBuilder
    {
        string Build(string[] headers, MobSkillData[] data);
    }
}