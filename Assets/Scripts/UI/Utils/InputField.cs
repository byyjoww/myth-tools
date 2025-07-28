using TMPro;
using UnityEngine.Events;

namespace ROTools.UI
{
    [System.Serializable]
    public struct InputField
    {
        public TMP_Text Text;
        public TMP_InputField Value;

        public void Setup(string text, string value, UnityAction<string> onValueChanged)
        {
            Text.text = text;
            Value.onValueChanged.RemoveAllListeners();
            Value.text = value;
            Value.onValueChanged.AddListener(onValueChanged);
        }
    }
}