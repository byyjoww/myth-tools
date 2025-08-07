using ROTools.App;
using SLS.UI;

namespace ROTools.UI
{
    public class FileLoaderViewController : ViewController<FileLoaderView, FileLoader>
    {
        private FileSelectionViewController fileSelectionViewController = default;

        private FileSelectionViewController.File[] filesToLoad = new FileSelectionViewController.File[]
        {
            new FileSelectionViewController.File
            {
                Name = "Mob Skill DB:",
                Extension = "txt",
                ExpectedName = "mob_skill_db",
            },
            new FileSelectionViewController.File
            {
                Name = "Skill DB:",
                Extension = "txt",
                ExpectedName = "skill_db",
            },
            new FileSelectionViewController.File
            {
                Name = "Skill DB Ext:",
                Extension = "txt",
                ExpectedName = "skill_db_ext",
            },
            new FileSelectionViewController.File
            {
                Name = "Mob DB:",
                Extension = "yml",
                ExpectedName = "mob_db",
            },
            new FileSelectionViewController.File
            {
                Name = "Mob DB RE:",
                Extension = "yml",
                ExpectedName = "mob_db_re",
            },
            new FileSelectionViewController.File
            {
                Name = "Mob DB Elite:",
                Extension = "yml",
                ExpectedName = "mob_db_elite",
            },
            new FileSelectionViewController.File
            {
                Name = "Mob DB Rare:",
                Extension = "yml",
                ExpectedName = "mob_db_rare",
            },
            new FileSelectionViewController.File
            {
                Name = "Mob DB Mini:",
                Extension = "yml",
                ExpectedName = "mob_db_mini",
            },
            new FileSelectionViewController.File
            {
                Name = "Mob DB Boss:",
                Extension = "yml",
                ExpectedName = "mob_db_boss",
            },
        };

        protected override bool showOnInit => true;

        public FileLoaderViewController(FileLoaderView view, FileLoader model, FileSelectionViewController fileSelectionViewController) : base(view, model)
        {
            this.fileSelectionViewController = fileSelectionViewController;
        }

        protected override void OnShow()
        {

        }

        protected override void OnHide()
        {

        }

        protected override void OnInit()
        {
            view.Setup(new FileLoaderView.PresenterModel
            {
                LoadText = "Load",
                CanLoad = true,
                OnLoad = delegate
                {
                    fileSelectionViewController.Show(filesToLoad, (selected) =>
                    {
                        model.Load(new FileLoader.FilePaths
                        {
                            MobSkillDb = selected[filesToLoad[0]],
                            SkillDb = selected[filesToLoad[1]],
                            SkillDbExtended = selected[filesToLoad[2]],
                            MobDb = selected[filesToLoad[3]],
                            MobDbRE = selected[filesToLoad[4]],
                            MobDbElite = selected[filesToLoad[5]],
                            MobDbRare = selected[filesToLoad[6]],
                            MobDbMini = selected[filesToLoad[7]],
                            MobDbBoss = selected[filesToLoad[8]],
                        });
                    });
                },
                SaveText = "Save",
                CanSave = model.IsLoaded,
                OnSave = delegate
                {
                    model.SaveSkillDB();
                },
            });
        }

        protected override void OnDispose()
        {

        }
    }
}