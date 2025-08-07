namespace ROTools.Skills
{
    public interface ISkillDBParser
    {
        (string[] headers, SkillData[] data) Parse(string content);
    }
}