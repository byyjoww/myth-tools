namespace ROTools.Skills
{
    public interface ISkillDBParser
    {
        (string[] headers, MobSkillData[] data) Parse(string content);
    }
}