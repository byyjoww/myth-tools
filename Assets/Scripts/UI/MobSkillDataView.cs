using SLS.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ROTools.UI
{
    public class MobSkillDataView : View
    {
        public struct PresenterModel
        {
            public string InstanceID { get; set; }
            public string Name { get; set; }
            public UnityAction OnSelect { get; set; }
            public bool IsSelected { get; set; }
        }

        [SerializeField] private TMP_Text mobSkillName = default;
        [SerializeField] private SLSButton select = default;
        [SerializeField] private Image highlight = default;

        public void Setup(PresenterModel model)
        {
            mobSkillName.text = model.Name;
            SetButtonAction(select, model.OnSelect, !model.IsSelected);
            SetSelected(model.IsSelected);
        }

        public void Setup(string name)
        {
            mobSkillName.text = name;
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