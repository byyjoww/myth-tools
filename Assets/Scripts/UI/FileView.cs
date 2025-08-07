using SLS.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ROTools.UI
{
    public class FileView : View
    {
        public struct PresenterModel
        {
            public string FileText { get; set; }
            public string FileValue { get; set; }
            public string LoadText { get; set; }
            public bool CanLoad { get; set; }
            public UnityAction OnLoad { get; set; }
        }

        [SerializeField] private TMP_Text fileText = default;
        [SerializeField] private TMP_InputField filePath = default;
        [SerializeField] private TMP_Text loadText = default;
        [SerializeField] private SLSButton load = default;

        public void Setup(PresenterModel model)
        {
            fileText.text = model.FileText;
            filePath.interactable = false;
            filePath.SetTextWithoutNotify(model.FileValue);
            loadText.text = model.LoadText;
            SetButtonAction(load, model.OnLoad, model.CanLoad);
        }
    }
}