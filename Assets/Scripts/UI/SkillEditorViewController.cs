using ROTools.Skills;
using SLS.UI;
using System;
using System.Linq;

namespace ROTools.UI
{
    public class SkillEditorViewController : ViewController<SkillEditorView, SkillEditor>
    {
        private SkillProvider skillProvider = default;

        private Guid selectedMobSkillInstanceID = Guid.Empty;        

        public SkillEditorViewController(SkillEditorView view, SkillEditor model, SkillProvider skillProvider) : base(view, model)
        {
            this.skillProvider = skillProvider;           
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
                        
            view.Setup(new SkillEditorView.PresenterModel
            {
                NameText = "Mob:",
                Name = skill.GetDescriptionMobName(),

                SkillText = "Skill:",
                CurrentSkill = skillOpts[skill.SkillID].SkillIndex,
                SkillOptions = skillOpts.Values.Select(x => x.Skill.Name).ToArray(),
                OnSkillOptionChanged = (index) => { model.UpdateSkillID(skill.InstanceID, index); },

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