using TMPro;

namespace ROTools.UI
{
    [System.Serializable ]
    public struct InfoField
    {
        public TMP_Text Text;
        public TMP_Text Value;

        public void Setup(string text, string value)
        {
            Text.text = text;
            Value.text = value;
        }
    }
}