using SLS.UI;
using UnityEngine;
using UnityEngine.Events;

namespace ROTools.UI
{
    public class SkillEditorView : View
    {
        public struct PresenterModel
        {
            public string NameText { get; set; }
            public string Name { get; set; }

            public string SkillText { get; set; }
            public int CurrentSkill { get; set; }
            public string[] SkillOptions { get; set; }
            public UnityAction<int> OnSkillOptionChanged { get; set; }

            public string SkillLevelText { get; set; }
            public string SkillLevel { get; set; }
            public UnityAction<string> OnSkillLevelChanged { get; set; }

            public string RateText { get; set; }
            public string Rate { get; set; }
            public UnityAction<string> OnSkillRateChanged { get; set; }

            public string CastTimeText { get; set; }
            public string CastTime { get; set; }
            public UnityAction<string> OnSkillCastTimeChanged { get; set; }

            public string DelayText { get; set; }
            public string Delay { get; set; }
            public UnityAction<string> OnSkillDelayChanged { get; set; }

            public string StateText { get; set; }
            public int CurrentState { get; set; }
            public string[] StateOptions { get; set; }
            public UnityAction<int> OnSkillStateOptionChanged { get; set; }

            public string TargetText { get; set; }
            public int CurrentTarget { get; set; }
            public string[] TargetOptions { get; set; }
            public UnityAction<int> OnSkillTargetOptionChanged { get; set; }

            public string ConditionText { get; set; }
            public int CurrentCondition { get; set; }
            public string[] ConditionOptions { get; set; }
            public UnityAction<int> OnSkillConditionOptionChanged { get; set; }

            public string ConditionValueText { get; set; }
            public string ConditionValue { get; set; }
            public UnityAction<string> OnSkillConditionValueChanged { get; set; }

            public string EmotionText { get; set; }
            public string Emotion { get; set; }
            public UnityAction<string> OnSkillEmotionChanged { get; set; }

            public string ChatText { get; set; }
            public string Chat { get; set; }
            public UnityAction<string> OnSkillChatChanged { get; set; }

            public (string Text, string Value)[] Values { get; set; }
            public UnityAction<int, string> OnSkillValueChanged { get; set; }

            public (string Text, string Value)[] Extras { get; set; }
            public UnityAction<int, string> OnSkillExtraChanged { get; set; }
        }

        [Header("Mob Skill Left")]
        [SerializeField] private InfoField mobName = default;
        [SerializeField] private DropdownField skill = default;
        [SerializeField] private InputField level = default;
        [SerializeField] private InputField rate = default;
        [SerializeField] private InputField castTime = default;
        [SerializeField] private InputField delay = default;
        [SerializeField] private DropdownField state = default;
        [SerializeField] private DropdownField target = default;
        [SerializeField] private DropdownField condition = default;
        [SerializeField] private InputField conditionValue = default;
        [SerializeField] private InputField emotion = default;
        [SerializeField] private InputField chat = default;

        [Header("Mob Skill Right")]
        [SerializeField] private InputField value1 = default;
        [SerializeField] private InputField value2 = default;
        [SerializeField] private InputField value3 = default;
        [SerializeField] private InputField value4 = default;
        [SerializeField] private InputField value5 = default;
        [SerializeField] private InputField extra1 = default;
        [SerializeField] private InputField extra2 = default;
        [SerializeField] private InputField extra3 = default;
        [SerializeField] private InputField extra4 = default;
        [SerializeField] private InputField extra5 = default;
        [SerializeField] private InputField extra6 = default;
        [SerializeField] private InputField extra7 = default;
        [SerializeField] private InputField extra8 = default;
        [SerializeField] private InputField extra9 = default;
        [SerializeField] private InputField extra10 = default;
        [SerializeField] private InputField extra11 = default;
        [SerializeField] private InputField extra12 = default;
        [SerializeField] private InputField extra13 = default;
        [SerializeField] private InputField extra14 = default;
        [SerializeField] private InputField extra15 = default;

        public void Setup(PresenterModel model)
        {            
            mobName.Setup(model.NameText, model.Name);
            skill.Setup(model.SkillText, model.SkillOptions, model.CurrentSkill, model.OnSkillOptionChanged);
            level.Setup(model.SkillLevelText, model.SkillLevel, model.OnSkillLevelChanged);
            rate.Setup(model.RateText, model.Rate, model.OnSkillRateChanged);
            castTime.Setup(model.CastTimeText, model.CastTime, model.OnSkillCastTimeChanged);
            delay.Setup(model.DelayText, model.Delay, model.OnSkillDelayChanged);
            state.Setup(model.StateText, model.StateOptions, model.CurrentState, model.OnSkillStateOptionChanged);
            target.Setup(model.TargetText, model.TargetOptions, model.CurrentTarget, model.OnSkillTargetOptionChanged);
            condition.Setup(model.ConditionText, model.ConditionOptions, model.CurrentCondition, model.OnSkillConditionOptionChanged);
            conditionValue.Setup(model.ConditionValueText, model.ConditionValue, model.OnSkillConditionValueChanged);
            emotion.Setup(model.EmotionText, model.Emotion, model.OnSkillEmotionChanged);
            chat.Setup(model.ChatText, model.Chat, model.OnSkillChatChanged);
            value1.Setup(model.Values[0].Text, model.Values[0].Value, val => model.OnSkillValueChanged?.Invoke(0, val));
            value2.Setup(model.Values[1].Text, model.Values[1].Value, val => model.OnSkillValueChanged?.Invoke(1, val));
            value3.Setup(model.Values[2].Text, model.Values[2].Value, val => model.OnSkillValueChanged?.Invoke(2, val));
            value4.Setup(model.Values[3].Text, model.Values[3].Value, val => model.OnSkillValueChanged?.Invoke(3, val));
            value5.Setup(model.Values[4].Text, model.Values[4].Value, val => model.OnSkillValueChanged?.Invoke(4, val));
            extra1.Setup(model.Extras[0].Text, model.Extras[0].Value, val => model.OnSkillExtraChanged?.Invoke(0, val));
            extra2.Setup(model.Extras[1].Text, model.Extras[1].Value, val => model.OnSkillExtraChanged?.Invoke(1, val));
            extra3.Setup(model.Extras[2].Text, model.Extras[2].Value, val => model.OnSkillExtraChanged?.Invoke(2, val));
            extra4.Setup(model.Extras[3].Text, model.Extras[3].Value, val => model.OnSkillExtraChanged?.Invoke(3, val));
            extra5.Setup(model.Extras[4].Text, model.Extras[4].Value, val => model.OnSkillExtraChanged?.Invoke(4, val));
            extra6.Setup(model.Extras[5].Text, model.Extras[5].Value, val => model.OnSkillExtraChanged?.Invoke(5, val));
            extra7.Setup(model.Extras[6].Text, model.Extras[6].Value, val => model.OnSkillExtraChanged?.Invoke(6, val));
            extra8.Setup(model.Extras[7].Text, model.Extras[7].Value, val => model.OnSkillExtraChanged?.Invoke(7, val));
            extra9.Setup(model.Extras[8].Text, model.Extras[8].Value, val => model.OnSkillExtraChanged?.Invoke(8, val));
            extra10.Setup(model.Extras[9].Text, model.Extras[9].Value, val => model.OnSkillExtraChanged?.Invoke(9, val));
            extra11.Setup(model.Extras[10].Text, model.Extras[10].Value, val => model.OnSkillExtraChanged?.Invoke(10, val));
            extra12.Setup(model.Extras[11].Text, model.Extras[11].Value, val => model.OnSkillExtraChanged?.Invoke(11, val));
            extra13.Setup(model.Extras[12].Text, model.Extras[12].Value, val => model.OnSkillExtraChanged?.Invoke(12, val));
            extra14.Setup(model.Extras[13].Text, model.Extras[13].Value, val => model.OnSkillExtraChanged?.Invoke(13, val));
            extra15.Setup(model.Extras[14].Text, model.Extras[14].Value, val => model.OnSkillExtraChanged?.Invoke(14, val));
        }
    }
}