using SLS.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ROTools.UI
{
    public class ComboBox : MonoBehaviour
    {
        public struct PresenterModel
        {
            public string[] Options { get; set; }
            public int CurrentOptIndex { get; set; }
            public string CurrentOptValue { get; set; }
            public bool CanInteract { get; set; }
            public UnityAction<string> OnSelectedValueChanged { get; set; }
            public UnityAction<string> OnSearchValueChanged { get; set; }     
            public UnityAction<string, UnityAction<string[]>> OnUpdateOptionsForSearchValueChanged { get; set; }
        }

        [SerializeField] private TMP_InputField input = default;
        [SerializeField] private TMP_Dropdown dropdown = default;

        public void Setup(PresenterModel model)
        {
            var optIndexToValue = GetOptsDictionary(model.Options);

            input.interactable = model.CanInteract;
            input.onValueChanged.RemoveAllListeners();
            input.text = model.CurrentOptValue;
            input.onValueChanged.AddListener(model.OnSearchValueChanged);
            input.onValueChanged.AddListener((txt) =>
            {
                model.OnUpdateOptionsForSearchValueChanged?.Invoke(txt, (newOpts) =>
                {
                    ColapseDropdown(string.Empty);
                    dropdown.options = newOpts.Select(x => new TMP_Dropdown.OptionData { text = x }).ToList();
                    optIndexToValue = GetOptsDictionary(newOpts);
                    ExpandDropdown(string.Empty);
                    input.Select();
                });
            });

            input.onSubmit.RemoveAllListeners();
            input.onSubmit.AddListener((txt) =>
            {
                // int idx = dropdown.options.IndexOf(new TMP_Dropdown.OptionData { text = txt });
                // dropdown.SetValueWithoutNotify(idx);
                input.SetTextWithoutNotify(txt);
                input.Select();
            });

            input.onSelect.RemoveAllListeners();
            input.onSelect.AddListener(ExpandDropdown);
            // input.onEndEdit.RemoveAllListeners();
            // input.onEndEdit.AddListener(ColapseDropdown);

            dropdown.interactable = model.CanInteract;
            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.options = model.Options.Select(x => new TMP_Dropdown.OptionData { text = x }).ToList();
            dropdown.value = model.CurrentOptIndex;
            dropdown.onValueChanged.AddListener((idx) =>
            {
                if (optIndexToValue.TryGetValue(idx, out string value))
                {
                    model.OnSelectedValueChanged?.Invoke(value);
                }
                else
                {
                    Debug.Log($"Index {idx} not present in option dictionary");
                }
            });
        }

        private void ExpandDropdown(string _)
        {
            if (!dropdown.IsExpanded)
            {
                dropdown.Show();
            }
            //ExecuteEvents.Execute<IPointerClickHandler>(
            //    dropdown.gameObject,
            //    new PointerEventData(EventSystem.current),
            //    ExecuteEvents.pointerClickHandler
            //);
        }

        private void ColapseDropdown(string _)
        {            
            dropdown.Hide();
            //ExecuteEvents.Execute<IPointerClickHandler>(
            //    dropdown.gameObject,
            //    new PointerEventData(EventSystem.current),
            //    ExecuteEvents.pointerClickHandler
            //);
        }

        private Dictionary<int, string> GetOptsDictionary(IEnumerable<string> opts)
        {
            var optIndexToValue = new Dictionary<int, string>();
            opts.ForEach((x, i) => optIndexToValue.Add(i, x));            
            return optIndexToValue;
        }
    }
}