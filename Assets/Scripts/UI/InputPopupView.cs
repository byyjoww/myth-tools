using SLS.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ROTools.UI
{
    public class InputPopupView : View
    {
        public class PresenterModel
        {
            public string Description { get; set; }
            public TMP_InputField.ContentType ContentType { get; set; }
            public UnityAction<string> OnValueChanged { get; set; }
            public string SubmitText { get; set; }            
            public UnityAction OnSubmit { get; set; }
            public string CancelText { get; set; }
            public UnityAction OnCancel { get; set; }
        }

        [SerializeField] private TMP_Text description = default;
        [SerializeField] private TMP_InputField input = default;
        [SerializeField] private TMP_Text submitText = default;
        [SerializeField] private SLSButton submit = default;
        [SerializeField] private TMP_Text cancelText = default;
        [SerializeField] private SLSButton cancel = default;

        public void Setup(PresenterModel model)
        {
            description.text = model.Description;

            submitText.text = model.SubmitText;
            SetButtonAction(submit, model.OnSubmit, true);

            cancelText.text = model.CancelText;
            SetButtonAction(cancel, model.OnCancel, true);

            SetupInputField(input, model.ContentType, string.Empty, model.OnValueChanged, true);
        }

        private void SetupInputField(TMP_InputField input, TMP_InputField.ContentType contentType, string value, UnityAction<string> onSearch, bool canSearch)
        {
            input.interactable = canSearch;
            input.contentType = contentType;
            input.onValueChanged.RemoveAllListeners();
            input.text = value;
            input.onValueChanged.AddListener(onSearch);
        }
    }
}