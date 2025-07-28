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
        [SerializeField] private SkillEditorView skillEditorView = default;

        // view controllers
        private SkillEditorViewController skillEditorViewController = default;

        // models
        private IUnityLogger logger = default;
        private SkillEditor skillEditor = default;

        public void Start() => Init();

        public void OnApplicationQuit() => Terminate();

        private void Init()
        {
            logger = new UnityLogger(logLevel);

            var dbparser = new SkillDBParser(logger);
            skillEditor = new SkillEditor(logger, dbparser, dbparser);

            skillEditorViewController = new SkillEditorViewController(skillEditorView, skillEditor);
            skillEditorViewController?.Init();
        }

        private void Terminate()
        {
            skillEditorViewController?.Dispose();
        }
    }
}