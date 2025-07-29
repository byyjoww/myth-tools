using SLS.UI;
using TMPro;
using UnityEngine.Events;

namespace ROTools.UI
{
    public class InputPopupViewController : ViewController<InputPopupView>
    {
        private string inputValue = default;

        public InputPopupViewController(InputPopupView view) : base(view)
        {

        }

        public void Show(string description, TMP_InputField.ContentType contentType, string submitText, UnityAction<string> onSubmit, bool hideOnSubmit = true)
        {
            view.Setup(new InputPopupView.PresenterModel
            {
                Description = description,
                ContentType = contentType,
                OnValueChanged = (txt) => { inputValue = txt; },
                SubmitText = submitText,
                OnSubmit = delegate
                {
                    onSubmit(inputValue);
                    if (hideOnSubmit) { Hide(); }
                },
                CancelText = "Cancel",
                OnCancel = delegate { Hide(); },
            });

            base.Show();
        }

        protected override void OnShow()
        {
            inputValue = string.Empty;
        }

        protected override void OnHide()
        {
            inputValue = string.Empty;
        }

        protected override void OnInit()
        {

        }

        protected override void OnDispose()
        {

        }
    }
}