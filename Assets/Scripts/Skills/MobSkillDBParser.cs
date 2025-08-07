using SLS.Core.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ROTools.Skills
{
    public class MobSkillDBParser : IMobSkillDBParser, IMobSkillDBBuilder
    {
        private IUnityLogger logger = default;

        public MobSkillDBParser(IUnityLogger logger)
        {
            this.logger = new UnityLoggerWrapper(logger);
        }

        public (string[] headers, MobSkillData[] data) Parse(string content)
        {
            string[] lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var mobData = new List<MobSkillData>();
            var headers = new List<string>();
            bool isHeaderRow = true;

            for (int i = 0; i < lines.Length; i++)
            {
                int index = i;
                string line = lines[i];
                MobSkillData data = ParseLine(index, line);

                // add headers until we get the first skill row
                if (data == null && isHeaderRow)
                {
                    headers.Add(line);
                    continue;
                }

                if (data != null)
                {
                    isHeaderRow = false;
                    mobData.Add(data);
                }
            }

            // remove all empty lines between headers and skill rows
            while (headers.Count > 0 && string.IsNullOrEmpty(headers[^1]))
            {
                headers.RemoveAt(headers.Count - 1);
            }

            return (headers.ToArray(), mobData.ToArray());
        }

        public string Build(string[] headers, MobSkillData[] data)
        {
            var sb = new StringBuilder();

            foreach (var header in headers)
            {
                sb.AppendLine(header);
            }

            sb.AppendLine();

            foreach (var skill in data)
            {
                if (skill == null)
                {
                    continue;
                }

                string line = skill.BuildLine();
                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        private MobSkillData ParseLine(int index, string line)
        {
            if (line.StartsWith("//"))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                return null;
            }

            string[] parts = line.Split(',');
            if (parts.Length < MobSkillData.NUM_OF_ARGS)
            {
                logger.LogError($"Malformed line at index {index}: {line}");
                return null;
            }

            var data = new MobSkillData
            {                
                MobID = ParseInt(parts[0]),
                Description = parts[1],
                State = ParseEnum<MobSkillData.MobState>(parts[2]),
                SkillID = ParseInt(parts[3]),
                SkillLevel = ParseInt(parts[4]),
                Rate = ParseInt(parts[5]),
                CastTime = ParseInt(parts[6]),
                Delay = ParseInt(parts[7]),
                Cancelable = ParseBool(parts[8]),
                Target = ParseEnum<MobSkillData.MobTarget>(parts[9]),
                Condition = ParseEnum<MobSkillData.SkillCondition>(parts[10]),
                ConditionValue = ParseInt(parts[11]),
                Values = new int[]
                {
                    ParseInt(parts[12]),
                    ParseInt(parts[13]),
                    ParseInt(parts[14]),
                    ParseInt(parts[15]),
                    ParseInt(parts[16]),
                },
                Emotion = ParseInt(parts[17]),
                Chat = ParseInt(parts[18]),
                Extras = new int[]
                {
                    ParseInt(parts[19]),
                    ParseInt(parts[20]),
                    ParseInt(parts[21]),
                    ParseInt(parts[22]),
                    ParseInt(parts[23]),
                    ParseInt(parts[24]),
                    ParseInt(parts[25]),
                    ParseInt(parts[26]),
                    ParseInt(parts[27]),
                    ParseInt(parts[28]),
                    ParseInt(parts[29]),
                    ParseInt(parts[30]),
                    ParseInt(parts[31]),
                    ParseInt(parts[32]),
                    ParseInt(parts[33]),
                    ParseInt(parts[34]),
                },
            };

            data.InstanceID = data.GetGuid();
            return data;
        }

        private int ParseInt(string text)
        {
            return int.TryParse(text, out int result)
                ? result
                : 0;
        }

        private bool ParseBool(string text)
        {
            return bool.TryParse(text, out bool result)
                ? result
                : false;
        }

        private T ParseEnum<T>(string text) where T : struct
        {
            return Enum.TryParse(value: text, ignoreCase: true, out T result)
                ? result
                : default;
        }
    }
}