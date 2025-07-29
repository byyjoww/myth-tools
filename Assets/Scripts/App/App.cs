using ROTools.Mobs;
using ROTools.Skills;
using ROTools.UI;
using SFB;
using SLS.Core.Logging;
using System.IO;
using UnityEngine;

namespace ROTools.App
{
    public class App : MonoBehaviour
    {
        [SerializeField] private LogLevel logLevel = LogLevel.Debug;

        [Header("Views")]
        [SerializeField] private FileLoaderView fileLoaderView = default;
        [SerializeField] private MobListView mobListView = default;
        [SerializeField] private MobSkillListView mobSkillListView = default;
        [SerializeField] private SkillEditorView skillEditorView = default;
        [SerializeField] private InputPopupView inputPopupView = default;
        [SerializeField] private LoadingView loadingView = default;

        // view controllers
        private FileLoaderViewController fileLoaderViewController = default;
        private MobListViewController mobListViewController = default;
        private MobSkillListViewController mobSkillListViewController = default;
        private SkillEditorViewController skillEditorViewController = default;
        private InputPopupViewController inputPopupViewController = default;
        private LoadingViewController loadingViewController = default;

        // models
        private IUnityLogger logger = default;        
        private SkillDBParser skillDBParser = default;
        private MobProvider mobProvider = default;
        private SkillProvider skillProvider = default;
        private SkillEditor skillEditor = default;
        private FileLoader fileProvider = default;

        public void Start() => Init();

        public void OnApplicationQuit() => Terminate();

        private void Init()
        {
            logger = new UnityLogger(logLevel);

            skillDBParser = new SkillDBParser(logger);
            mobProvider = new MobProvider(logger);
            skillProvider = new SkillProvider(logger);
            skillEditor = new SkillEditor(logger, mobProvider, skillProvider);
            fileProvider = new FileLoader(logger, skillDBParser, skillDBParser, mobProvider, skillProvider, skillEditor);

            loadingViewController = new LoadingViewController(loadingView);
            loadingViewController?.Init();

            inputPopupViewController = new InputPopupViewController(inputPopupView);
            inputPopupViewController?.Init();

            fileLoaderViewController = new FileLoaderViewController(fileLoaderView, fileProvider, loadingViewController);
            fileLoaderViewController?.Init();

            mobListViewController = new MobListViewController(mobListView, mobProvider, inputPopupViewController);
            mobListViewController?.Init();

            mobSkillListViewController = new MobSkillListViewController(mobSkillListView, skillEditor, skillProvider, inputPopupViewController);
            mobSkillListViewController?.Init();

            skillEditorViewController = new SkillEditorViewController(skillEditorView, skillEditor, skillProvider);
            skillEditorViewController?.Init();

            mobListViewController.OnMobSelected += mobSkillListViewController.Show;
            mobSkillListViewController.OnMobSkillSelected += skillEditorViewController.Show;
        }

        private void Terminate()
        {
            mobListViewController.OnMobSelected -= mobSkillListViewController.Show;
            mobSkillListViewController.OnMobSkillSelected -= skillEditorViewController.Show;

            inputPopupViewController?.Dispose();
            fileLoaderViewController?.Dispose();
            mobListViewController?.Dispose();
            mobSkillListViewController?.Dispose();
            skillEditorViewController?.Dispose();
        }
    }
}