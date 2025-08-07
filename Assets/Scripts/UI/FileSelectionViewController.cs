using SFB;
using SLS.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ROTools.UI
{
    public class FileSelectionViewController : ViewController<FileSelectionView>
    {
        public struct File
        {
            public string Name { get; set; }
            public string Extension { get; set; }
            public string ExpectedName { get; set; }
        }

        private LoadingViewController loadingViewController = default;

        private Dictionary<File, string> selectedPaths = default;

        public FileSelectionViewController(FileSelectionView view, LoadingViewController loadingViewController) : base(view)
        {
            this.loadingViewController = loadingViewController;
            selectedPaths = new Dictionary<File, string>();
        }

        public void Show(IEnumerable<File> requiredFiles, UnityAction<Dictionary<File, string>> onConfirm)
        {
            base.Show();
            view.Setup(new FileSelectionView.PresenterModel
            {
                Files = requiredFiles.Select(file => new FileView.PresenterModel
                {
                    FileText = $"{file.Name}",
                    FileValue = selectedPaths.TryGetValue(file, out string path) ? path : string.Empty,
                    LoadText = "Load",
                    CanLoad = true,
                    OnLoad = delegate
                    {
                        loadingViewController.Show();
                        string[] paths = StandaloneFileBrowser.OpenFilePanel($"Select {file.Name}", "", file.Extension, false);
                        if (paths.Length > 0)
                        {
                            selectedPaths.TryAdd(file, paths.First());
                            loadingViewController.HideWithFadeTime();
                            Show(requiredFiles, onConfirm);
                        }
                    },
                }),
                
                ConfirmText = "Confirm",
                CanConfirm = selectedPaths.Count == requiredFiles.Distinct().Count(),
                OnConfirm = delegate
                {
                    onConfirm?.Invoke(selectedPaths);
                    Hide();
                },

                AutofillText = "Autofill",
                CanAutofill = selectedPaths.Count > 0,
                OnAutofill = delegate
                {
                    loadingViewController.Show();

                    var selected = selectedPaths.First();
                    string directory = Path.GetDirectoryName(selected.Value);

                    foreach (var file in requiredFiles)
                    {
                        if (selectedPaths.ContainsKey(file)) { continue; }

                        string filename = $"{file.ExpectedName}.{file.Extension}";
                        string filepath = Path.Combine(directory, filename);
                        if (System.IO.File.Exists(filepath))
                        {
                            selectedPaths.TryAdd(file, filepath);
                        }
                    }

                    loadingViewController.HideWithFadeTime();
                    Show(requiredFiles, onConfirm);
                },

                BackText = "Back",
                CanBack = true,
                OnBack = delegate
                {
                    Hide();
                },
            });
        }

        protected override void OnShow()
        {

        }

        protected override void OnHide()
        {
            selectedPaths.Clear();
        }

        protected override void OnInit()
        {

        }

        protected override void OnDispose()
        {

        }
    }
}