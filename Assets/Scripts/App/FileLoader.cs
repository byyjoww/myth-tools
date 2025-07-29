using ROTools.Mobs;
using ROTools.Skills;
using SLS.Core.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Events;

namespace ROTools.App
{
    public class FileLoader
    {
        private IUnityLogger logger = default;
        private ISkillDBParser parser = default;
        private ISkillDBBuilder builder = default;
        private MobProvider mobProvider = default;
        private SkillProvider skillProvider = default;
        private SkillEditor skillEditor = default;

        // old file data
        private string[] headers = default;
        private string path = default;

        public bool IsLoaded { get; private set; }

        public event UnityAction OnLoaded;

        public FileLoader(IUnityLogger logger, ISkillDBParser parser, ISkillDBBuilder builder, MobProvider mobProvider, SkillProvider skillProvider, SkillEditor skillEditor)
        {
            this.logger = new UnityLoggerWrapper(logger);
            this.parser = parser;
            this.builder = builder;
            this.mobProvider = mobProvider;
            this.skillProvider = skillProvider;
            this.skillEditor = skillEditor;
        }

        public void LoadSkillDB(string[] paths)
        {
            path = string.Empty;
            mobProvider.Clear();
            skillProvider.Clear();
            skillEditor.Clear();

            if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
            {
                path = paths[0];

                string content = File.ReadAllText(path);
                var parsedContent = parser.Parse(content);
                headers = parsedContent.headers;
                MobSkillData[] skillData = parsedContent.data;
                var mobs = new Dictionary<int, string>();
                var skills = new Dictionary<int, string>();
                var mobSkills = new List<MobSkillData>();
                foreach (MobSkillData msd in skillData)
                {
                    mobs.TryAdd(msd.MobID, msd.GetDescriptionMobName());
                    skills.TryAdd(msd.SkillID, msd.GetDescriptionSkillName());
                    mobSkills.Add(msd);
                }

                mobProvider.AddMobs(mobs.ToArray());
                skillProvider.AddSkills(skills.ToArray());
                skillEditor.AddMobSkillData(mobSkills);

                logger.LogInfo($"Loaded content from path {path}:\n{content}");
            }

            OnLoaded?.Invoke();
        }

        public void SaveSkillDB()
        {
            string directory = Path.GetDirectoryName(path);
            string oldFileName = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);

            string newFilePath;
            int counter = 1;

            do
            {
                string newFilename = $"{oldFileName}({counter}){extension}";
                newFilePath = Path.Combine(directory, newFilename);
                counter++;
            }
            while (File.Exists(newFilePath));

            MobSkillData[] data = skillEditor.MobSkillData.Values
                .OrderBy(x => x.MobID)
                .ThenBy(x => x.SkillID)
                .ThenBy(x => (int)x.State)
                .ThenBy(x => x.InstanceID)
                .ToArray();

            string content = builder.Build(headers, data);
            File.WriteAllText(newFilePath, content);

            logger.LogInfo($"Saving content at path {newFilePath}:\n{content}");
        }
    }
}
