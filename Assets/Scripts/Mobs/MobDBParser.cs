using ROTools.Yaml;
using SLS.Core.Logging;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.YamlDotNet.Serialization;
using Unity.VisualScripting.YamlDotNet.Serialization.NamingConventions;

namespace ROTools.Mobs
{
    public class MobDBParser : IMobDBParser
    {
        private IUnityLogger logger = default;

        public MobDBParser(IUnityLogger logger)
        {
            this.logger = new UnityLoggerWrapper(logger);
        }

        public (string[] headers, MobData[] data) Parse(string content)
        {
            var input = new StringReader(content);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new PascalCaseNamingConvention())
                .IgnoreUnmatchedProperties()
                .Build();

            var root = deserializer.Deserialize<Root<MobData>>(input);
            if (root == null || root.Body == null)
            {
                return (new string[0], new MobData[0]);
            }

            return (new string[0], root.Body.ToArray());
        }
    }
}