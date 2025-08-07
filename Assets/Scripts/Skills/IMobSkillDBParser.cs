namespace ROTools.Skills
{
    public interface IMobSkillDBParser
    {
        (string[] headers, MobSkillData[] data) Parse(string content);
    }
}