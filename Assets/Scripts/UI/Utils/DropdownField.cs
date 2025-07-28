using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.Events;

namespace ROTools.UI
{
    [System.Serializable]
    public struct DropdownField
    {
        public TMP_Text Text;
        public TMP_Dropdown Value;

        public void Setup(string text, string[] options, int current, UnityAction<int> onValueChanged)
        {
            Text.text = text;

            Value.onValueChanged.RemoveAllListeners();
            Value.options = options.Select(x => new TMP_Dropdown.OptionData { text = x }).ToList();
            Value.value = current;
            Value.onValueChanged.AddListener(onValueChanged);
        }
    }
}