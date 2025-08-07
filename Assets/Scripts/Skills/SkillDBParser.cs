using ROTools.Yaml;
using SLS.Core.Logging;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.YamlDotNet.Serialization;
using Unity.VisualScripting.YamlDotNet.Serialization.NamingConventions;

namespace ROTools.Skills
{
    public class SkillDBParser : ISkillDBParser
    {
        private IUnityLogger logger = default;

        public SkillDBParser(IUnityLogger logger)
        {
            this.logger = new UnityLoggerWrapper(logger);
        }

        public (string[] headers, SkillData[] data) Parse(string content)
        {
            var input = new StringReader(content);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new PascalCaseNamingConvention())
                .IgnoreUnmatchedProperties()
                .Build();

            var root = deserializer.Deserialize<Root<SkillData>>(input);
            if (root == null || root.Body == null)
            {
                return (new string[0], new SkillData[0]);
            }

            return (new string[0], root.Body.ToArray());
        }
    }
}