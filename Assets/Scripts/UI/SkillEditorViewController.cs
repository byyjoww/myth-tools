using ROTools.Skills;
using SLS.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ROTools.UI
{
    public class SkillEditorViewController : ViewController<SkillEditorView, SkillEditor>
    {
        private SkillProvider skillProvider = default;

        private Guid selectedMobSkillInstanceID = Guid.Empty;
        private Dictionary<string, int> skillNameToIndex = default;

        public SkillEditorViewController(SkillEditorView view, SkillEditor model, SkillProvider skillProvider) : base(view, model)
        {
            this.skillProvider = skillProvider;
            skillNameToIndex = new Dictionary<string, int>();
        }

        protected override void OnShow()
        {

        }

        protected override void OnHide()
        {
            selectedMobSkillInstanceID = Guid.Empty;
        }

        protected override void OnInit()
        {
            model.OnMobSkillChanged += OnMobSkillChanged;
            model.OnMobSkillRemoved += OnMobSkillRemoved;
            model.OnMobSkillsCleared += OnMobSkillsCleared;
        }

        protected override void OnDispose()
        {
            model.OnMobSkillChanged -= OnMobSkillChanged;
            model.OnMobSkillRemoved -= OnMobSkillRemoved;
            model.OnMobSkillsCleared -= OnMobSkillsCleared;
        }

        public void Show(Guid mobSkillInstanceID)
        {
            selectedMobSkillInstanceID = mobSkillInstanceID;

            if (!model.MobSkillData.TryGetValue(mobSkillInstanceID, out var skill))
            {
                Hide();
                return;
            }

            var skillOpts = skillProvider.AllSkillOptions;
            var stateOpts = model.AllStateOptions;
            var targetOpts = model.AllTargetOptions;
            var conditionOpts = model.AllConditionOptions;
            var opt = skillOpts[skill.SkillID];
            skillNameToIndex = skillOpts.ToDictionary(x => x.Value.Skill.Name, y => y.Value.SkillIndex);

            view.Setup(new SkillEditorView.PresenterModel
            {
                NameText = "Mob:",
                Name = skill.GetDescriptionMobName(),

                SkillText = "Skill:",
                SkillModel = new ComboBox.PresenterModel
                {
                    CurrentOptIndex = opt.SkillIndex,
                    CurrentOptValue = opt.Skill.Name,
                    Options = new string[0],
                    CanInteract = true,
                    OnSelectedValueChanged = (name) =>
                    {
                        if (skillNameToIndex.TryGetValue(name, out int index))
                        {
                            model.UpdateSkillID(skill.InstanceID, index);
                        }
                        else
                        {
                            Debug.Log($"Name {name} not present in option dictionary");
                        }
                    },
                    OnSearchValueChanged = (val) => { },
                    OnUpdateOptionsForSearchValueChanged = (txt, updtFunc) =>
                    {
                        bool ShouldInclude(int skillID, string skillName)
                        {
                            if (string.IsNullOrWhiteSpace(txt))
                            {
                                return false;
                            }

                            return skillName.IndexOf(txt, StringComparison.OrdinalIgnoreCase) >= 0
                                || skillID.ToString().IndexOf(txt, StringComparison.OrdinalIgnoreCase) >= 0;
                        }

                        string[] filteredOpts = skillOpts
                            .Where(x => ShouldInclude(x.Value.Skill.Id, x.Value.Skill.Name))
                            .Take(5)
                            .Select(x => x.Value.Skill.Name)                            
                            .ToArray();

                        updtFunc(filteredOpts);
                    },
                },

                SkillLevelText = "Skill Level:",
                SkillLevel = $"{skill.SkillLevel}",
                OnSkillLevelChanged = (level) => { model.UpdateSkillLevel(skill.InstanceID, ParseInt(level)); },

                RateText = "Chance:",
                Rate = $"{skill.Rate}",
                OnSkillRateChanged = (rate) => { model.UpdateSkillRate(skill.InstanceID, ParseInt(rate)); },

                CastTimeText = "Cast Time:",
                CastTime = $"{skill.CastTime}",
                OnSkillCastTimeChanged = (castTime) => { model.UpdateSkillCastTime(skill.InstanceID, ParseInt(castTime)); },

                DelayText = "Cooldown:",
                Delay = $"{skill.Delay}",
                OnSkillDelayChanged = (delay) => { model.UpdateSkillDelay(skill.InstanceID, ParseInt(delay)); },

                StateText = "State:",
                CurrentState = stateOpts[skill.State].StateIndex,
                StateOptions = stateOpts.Values.Select(x => x.StateName).ToArray(),
                OnSkillStateOptionChanged = (index) => { model.UpdateSkillState(skill.InstanceID, index); },

                TargetText = "Target:",
                CurrentTarget = targetOpts[skill.Target].TargetIndex,
                TargetOptions = targetOpts.Values.Select(x => x.TargetName).ToArray(),
                OnSkillTargetOptionChanged = (index) => { model.UpdateSkillTarget(skill.InstanceID, index); },

                ConditionText = "Condition:",
                CurrentCondition = conditionOpts[skill.Condition].ConditionIndex,
                ConditionOptions = conditionOpts.Values.Select(x => x.ConditionName).ToArray(),
                OnSkillConditionOptionChanged = (index) => { model.UpdateSkillCondition(skill.InstanceID, index); },

                ConditionValueText = "Condition Value:",
                ConditionValue = $"{skill.ConditionValue}",
                OnSkillConditionValueChanged = (conditionValue) => { model.UpdateSkillConditionValue(skill.InstanceID, ParseInt(conditionValue)); },

                EmotionText = "Emotion:",
                Emotion = $"{skill.Emotion}",
                OnSkillEmotionChanged = (emotion) => { model.UpdateSkillEmotion(skill.InstanceID, ParseInt(emotion)); },

                ChatText = "Chat:",
                Chat = $"{skill.Chat}",
                OnSkillChatChanged = (chat) => { model.UpdateSkillChat(skill.InstanceID, ParseInt(chat)); },

                Values = skill.Values.Select((x, i) => ($"Value {i + 1}:", $"{x}")).ToArray(),
                OnSkillValueChanged = (idx, value) => { model.UpdateSkillValue(skill.InstanceID, idx, ParseInt(value)); },

                Extras = skill.Extras.Select((x, i) => ($"Extra {i + 1}:", $"{x}")).ToArray(),
                OnSkillExtraChanged = (idx, extra) => { model.UpdateSkillExtra(skill.InstanceID, idx, ParseInt(extra)); },
            });

            Show();
        }

        private void OnMobSkillChanged(Guid instanceID)
        {
            if (instanceID == selectedMobSkillInstanceID)
            {
                Show(selectedMobSkillInstanceID);
            }
        }

        private void OnMobSkillRemoved(Guid instanceID)
        {
            if (instanceID == selectedMobSkillInstanceID)
            {
                Hide();
            }
        }

        private void OnMobSkillsCleared(int mobID)
        {
            Hide();
        }

        private int ParseInt(string value)
        {
            return int.TryParse(value, out var result)
                ? result
                : 0;
        }
    }
}