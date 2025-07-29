using SLS.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ROTools.UI
{
    public class FileLoaderView : View
    {
        public struct PresenterModel
        {
            public string LoadText { get; set; }
            public bool CanLoad { get; set; }
            public UnityAction OnLoad { get; set; }

            public string SaveText { get; set; }
            public bool CanSave { get; set; }
            public UnityAction OnSave { get; set; }
        }

        [SerializeField] private TMP_Text loadText = default;
        [SerializeField] private SLSButton load = default;
        [SerializeField] private TMP_Text saveText = default;
        [SerializeField] private SLSButton save = default;

        public void Setup(PresenterModel model)
        {
            loadText.text = model.LoadText;
            SetButtonAction(load, model.OnLoad, model.CanLoad);

            saveText.text = model.SaveText;
            SetButtonAction(save, model.OnSave, model.CanSave);
        }
    }
}