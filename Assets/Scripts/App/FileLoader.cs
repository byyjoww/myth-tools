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
        public struct FilePaths
        {
            public string MobSkillDb { get; set; }
            public string SkillDb { get; set; }
            public string SkillDbExtended { get; set; }
            public string MobDb { get; set; }
            public string MobDbRE { get; set; }
            public string MobDbElite { get; set; }
            public string MobDbRare { get; set; }
            public string MobDbMini { get; set; }
            public string MobDbBoss { get; set; }
        }

        private IUnityLogger logger = default;

        // parsers
        private IMobSkillDBParser mobSkillDbParser = default;
        private IMobSkillDBBuilder mobSkillDbBuilder = default;
        private IMobDBParser mobDbParser = default;
        private ISkillDBParser skillDbParser = default;

        // providers
        private MobProvider mobProvider = default;
        private SkillProvider skillProvider = default;
        private SkillEditor skillEditor = default;

        // old file data
        private string[] mobSkillDbHeaders = default;        
        private FilePaths paths = default;

        public bool IsLoaded { get; private set; }

        public event UnityAction OnLoaded;

        public FileLoader(IUnityLogger logger, IMobSkillDBParser mobSkillDbParser, IMobSkillDBBuilder mobSkillDbBuilder, IMobDBParser mobDbParser, ISkillDBParser skillDbParser, 
            MobProvider mobProvider, SkillProvider skillProvider, SkillEditor skillEditor)
        {
            this.logger = new UnityLoggerWrapper(logger);
            this.mobSkillDbParser = mobSkillDbParser;
            this.mobSkillDbBuilder = mobSkillDbBuilder;
            this.mobDbParser = mobDbParser;
            this.skillDbParser = skillDbParser;
            this.mobProvider = mobProvider;
            this.skillProvider = skillProvider;
            this.skillEditor = skillEditor;
        }

        public void Load(FilePaths paths)
        {
            this.paths = default;
            mobProvider.Clear();
            skillProvider.Clear();
            skillEditor.Clear();

            if (HasAllValidPaths(paths))
            {
                this.paths = paths;

                var mobs = new List<MobData>();
                mobs.AddRange(LoadMobDB(paths.MobDb));
                mobs.AddRange(LoadMobDB(paths.MobDbRE));
                mobs.AddRange(LoadMobDB(paths.MobDbElite));
                mobs.AddRange(LoadMobDB(paths.MobDbRare));
                mobs.AddRange(LoadMobDB(paths.MobDbMini));
                mobs.AddRange(LoadMobDB(paths.MobDbBoss));

                foreach (var grp in mobs.GroupBy(x => x.Id).Where(x => x.Count() > 1).ToArray())
                {
                    logger.LogDebug($"Duplicate entry for mob id {grp.Key}");
                }                

                var distinctMobs = mobs
                    .GroupBy(x => x.Id)
                    .Select(x => x.First())
                    .ToDictionary(x => x.Id, y => y.Name)
                    .ToArray();
                mobProvider.AddMobs(distinctMobs);

                var skills = new List<SkillData>();
                skills.AddRange(LoadSkillDB(paths.SkillDb));
                skills.AddRange(LoadSkillDB(paths.SkillDbExtended));

                foreach (var grp in skills.GroupBy(x => x.Id).Where(x => x.Count() > 1).ToArray())
                {
                    logger.LogDebug($"Duplicate entry for skill id {grp.Key}");
                }

                var distinctSkills = skills
                    .GroupBy(x => x.Id)
                    .Select(x => x.First())
                    .ToDictionary(x => x.Id, y => y.Name)
                    .ToArray();
                skillProvider.AddSkills(distinctSkills);

                var mobSkills = LoadMobSkillDB(paths.MobSkillDb);
                skillEditor.AddMobSkillData(mobSkills);
            }

            OnLoaded?.Invoke();
        }

        public void LoadSkillDB(string[] paths)
        {
            this.paths = default;
            mobProvider.Clear();
            skillProvider.Clear();
            skillEditor.Clear();

            if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
            {
                this.paths = new FilePaths
                {
                    MobSkillDb = paths[0],
                };

                string content = File.ReadAllText(this.paths.MobSkillDb);
                var parsedContent = mobSkillDbParser.Parse(content);
                mobSkillDbHeaders = parsedContent.headers;
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

                logger.LogInfo($"Loaded content from path {this.paths.MobSkillDb}:\n{content}");
            }

            OnLoaded?.Invoke();
        }

        public void SaveSkillDB()
        {
            if (string.IsNullOrEmpty(paths.MobSkillDb))
            {
                return;
            }

            string directory = Path.GetDirectoryName(paths.MobSkillDb);
            string oldFileName = Path.GetFileNameWithoutExtension(paths.MobSkillDb);
            string extension = Path.GetExtension(paths.MobSkillDb);

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

            string content = mobSkillDbBuilder.Build(mobSkillDbHeaders, data);
            File.WriteAllText(newFilePath, content);

            logger.LogInfo($"Saving content at path {newFilePath}:\n{content}");
        }

        private IEnumerable<MobData> LoadMobDB(string path)
        {
            logger.LogDebug($"Loading mob db content at path {path}");
            string content = File.ReadAllText(path);
            var parsedContent = mobDbParser.Parse(content);
            logger.LogInfo($"Loaded mob db content from path {path}:\n{content}");
            return parsedContent.data;
        }

        private IEnumerable<SkillData> LoadSkillDB(string path)
        {
            logger.LogDebug($"Loading skill db content at path {path}");
            string content = File.ReadAllText(path);
            var parsedContent = skillDbParser.Parse(content);            
            logger.LogInfo($"Loaded skill db content from path {path}:\n{content}");
            return parsedContent.data;
        }

        private IEnumerable<MobSkillData> LoadMobSkillDB(string path)
        {
            logger.LogDebug($"Loading mob skill db content at path {path}");
            string content = File.ReadAllText(path);
            var parsedContent = mobSkillDbParser.Parse(content);
            mobSkillDbHeaders = parsedContent.headers;
            var data = parsedContent.data;
            logger.LogInfo($"Loaded mob skill db content from path {path}:\n{content}");
            return data;
        }

        private bool HasAllValidPaths(FilePaths paths)
        {
            return !string.IsNullOrEmpty(paths.MobSkillDb)
                && !string.IsNullOrEmpty(paths.SkillDb)
                && !string.IsNullOrEmpty(paths.SkillDbExtended)
                && !string.IsNullOrEmpty(paths.MobDb)
                && !string.IsNullOrEmpty(paths.MobDbRE)
                && !string.IsNullOrEmpty(paths.MobDbElite)
                && !string.IsNullOrEmpty(paths.MobDbRare)
                && !string.IsNullOrEmpty(paths.MobDbMini)
                && !string.IsNullOrEmpty(paths.MobDbBoss);
        }
    }
}
