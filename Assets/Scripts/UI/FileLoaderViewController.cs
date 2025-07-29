using ROTools.App;
using SFB;
using SLS.UI;

namespace ROTools.UI
{
    public class FileLoaderViewController : ViewController<FileLoaderView, FileLoader>
    {
        private const string FILE_SEARCH_WINDOW_TITLE = "Select Skill DB";
        private const string FILE_SEARCH_WINDOW_EXTENSION = "txt";

        private LoadingViewController loadingViewController = default;

        protected override bool showOnInit => true;

        public FileLoaderViewController(FileLoaderView view, FileLoader model, LoadingViewController loadingViewController) : base(view, model)
        {
            this.loadingViewController = loadingViewController;
        }

        protected override void OnShow()
        {
            model.OnLoaded += OnLoaded;
        }

        protected override void OnHide()
        {
            model.OnLoaded -= OnLoaded;
        }

        protected override void OnInit()
        {
            view.Setup(new FileLoaderView.PresenterModel
            {
                LoadText = "Load",
                CanLoad = true,
                OnLoad = delegate
                {
                    loadingViewController.Show();
                    string[] paths = StandaloneFileBrowser.OpenFilePanel(FILE_SEARCH_WINDOW_TITLE, "", FILE_SEARCH_WINDOW_EXTENSION, false);
                    model.LoadSkillDB(paths);
                },
                SaveText = "Save",
                CanSave = model.IsLoaded,
                OnSave = delegate
                {
                    model.SaveSkillDB();
                },
            });
        }

        private void OnLoaded()
        {
            loadingViewController.HideWithFadeTime();
        }

        protected override void OnDispose()
        {
            
        }
    }
}