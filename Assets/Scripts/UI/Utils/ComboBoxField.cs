using TMPro;

namespace ROTools.UI
{
    [System.Serializable]
    public struct ComboBoxField
    {
        public TMP_Text Text;
        public ComboBox Value;

        public void Setup(string text, ComboBox.PresenterModel model)
        {
            Text.text = text;
            Value.Setup(model);
        }
    }
}