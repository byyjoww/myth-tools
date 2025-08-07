using SLS.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ROTools.UI
{
    public class FileSelectionView : View
    {
        public struct PresenterModel
        {
            public IEnumerable<FileView.PresenterModel> Files { get; set; }

            public string ConfirmText { get; set; }
            public bool CanConfirm { get; set; }
            public UnityAction OnConfirm { get; set; }

            public string AutofillText { get; set; }
            public bool CanAutofill { get; set; }
            public UnityAction OnAutofill { get; set; }

            public string BackText { get; set; }
            public bool CanBack { get; set; }
            public UnityAction OnBack { get; set; }
        }

        [SerializeField] private TMP_Text confirmText = default;
        [SerializeField] private SLSButton confirm = default;
        [SerializeField] private TMP_Text autofillText = default;
        [SerializeField] private SLSButton autofill = default;
        [SerializeField] private TMP_Text backText = default;
        [SerializeField] private SLSButton back = default;
        [SerializeField] private ViewElementPoolSpawner<FileView> fileViews = default;

        public void Setup(PresenterModel model)
        {
            confirmText.text = model.ConfirmText;
            SetButtonAction(confirm, model.OnConfirm, model.CanConfirm);

            autofillText.text = model.AutofillText;
            SetButtonAction(autofill, model.OnAutofill, model.CanAutofill);

            backText.text = model.BackText;
            SetButtonAction(back, model.OnBack, model.CanBack);

            LoadFiles(model.Files);
        }

        private void LoadFiles(IEnumerable<FileView.PresenterModel> files)
        {
            int numOfElements = files.Count();
            var views = fileViews.Set(numOfElements);
            for (int i = 0; i < numOfElements; i++)
            {
                var view = views.ElementAt(i);
                var data = files.ElementAt(i);
                view.Setup(data);
            }
        }
    }
}