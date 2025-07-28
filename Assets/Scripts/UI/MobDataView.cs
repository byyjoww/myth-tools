using SLS.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ROTools.UI
{
    public class MobDataView : View
    {
        public struct PresenterModel
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public UnityAction OnSelect { get; set; }
            public bool IsSelected { get; set; }
        }

        [SerializeField] private TMP_Text mobName = default;
        [SerializeField] private SLSButton select = default;
        [SerializeField] private Image highlight = default;

        public void Setup(PresenterModel model)
        {
            mobName.text = model.Name;
            SetButtonAction(select, model.OnSelect, !model.IsSelected);
            SetSelected(model.IsSelected);
        }

        public void SetSelected(bool isHighlighted)
        {
            select.interactable = !isHighlighted;
            highlight.color = isHighlighted 
                ? Color.yellow 
                : Color.white;
        }
    }
}