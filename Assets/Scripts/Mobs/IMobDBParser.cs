namespace ROTools.Mobs
{
    public interface IMobDBParser
    {
        (string[] headers, MobData[] data) Parse(string content);
    }
}