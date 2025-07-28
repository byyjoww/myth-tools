using ROTools.Skills;
using SFB;
using SLS.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ROTools.UI
{
    public class SkillEditorViewController : ViewController<SkillEditorView, SkillEditor>
    {
        private const string FILE_SEARCH_WINDOW_TITLE = "Select Skill DB";
        private const string FILE_SEARCH_WINDOW_EXTENSION = "txt";

        private int selectedMobID = -1;
        private int selectedMobSkillID = -1;

        protected override bool showOnInit => true;

        public SkillEditorViewController(SkillEditorView view, SkillEditor model) : base(view, model)
        {

        }

        protected override void OnShow()
        {
            Reload();
        }

        protected override void OnHide()
        {

        }

        protected override void OnInit()
        {
            model.OnSkillDBLoaded += Reload;
            model.OnSkillChanged += ReloadSkillList;
        }

        protected override void OnDispose()
        {
            model.OnSkillDBLoaded -= Reload;
            model.OnSkillChanged -= ReloadSkillList;
        }

        private void Reload()
        {
            view.StopAllCoroutines();
            view.StartCoroutine(StopLoadingRoutine());

            Mob[] mobs = model.AllMobs;
            if (!model.IsLoaded || mobs.Length < 1)
            {
                Setup();
                return;
            }

            Mob selectedMob = mobs.FirstOrDefault();
            MobSkillData[] skills = model.GetMobSkills(selectedMob.ID);
            if (skills.Length < 1)
            {
                return;
            }

            MobSkillData selectedSkill = skills.FirstOrDefault();
            selectedMobID = selectedMob.ID;
            selectedMobSkillID = selectedSkill.SkillID;

            SelectSkillDB(mobs);
            SelectMob(selectedMob, skills);
            SelectMobSkill(selectedSkill);
        }

        private void ReloadSkillList(Guid instanceID)
        {
            if (model.AllMobData.TryGetValue(instanceID, out var data))
            {
                view.UpdateMobSkillDataInList(instanceID.ToString(), GetMobSkillDataDisplayNameInList(data));
            }            
        }

        private void Setup()
        {
            view.Setup(new SkillEditorView.SetupPresenterModel
            {
                LoadText = "Load",
                CanLoad = true,
                OnLoad = delegate
                {
                    view.SetLoading(true);
                    string[] paths = StandaloneFileBrowser.OpenFilePanel(FILE_SEARCH_WINDOW_TITLE, "", FILE_SEARCH_WINDOW_EXTENSION, false);
                    model.Load(paths);
                },
                SaveText = "Save",
                CanSave = model.IsLoaded,
                OnSave = delegate
                {
                    model.Save();
                },
            });
        }

        private void SelectSkillDB(Mob[] mobs)
        {
            Setup();
            view.SelectSkillDB(new SkillEditorView.DBPresenterModel
            {
                Mobs = mobs.Select(m => new MobDataView.PresenterModel
                {
                    ID = m.ID,
                    Name = m.GetDisplayName(),
                    OnSelect = delegate { OnSelectMob(m); },
                    IsSelected = selectedMobID == m.ID,
                }).ToArray(),
            });
        }

        private void SelectMob(Mob mob, MobSkillData[] skills)
        {
            view.SelectMob(new SkillEditorView.MobPresenterModel
            {
                NameText = "Mob:",
                Name = mob.GetDisplayName(),
                Skills = skills.Select(s => new MobSkillDataView.PresenterModel
                {
                    InstanceID = s.InstanceID.ToString(),
                    Name = GetMobSkillDataDisplayNameInList(s),
                    OnSelect = delegate { OnSelectMobSkill(s); },
                    IsSelected = selectedMobSkillID == s.SkillID,
                }).ToArray(),
            });
        }

        private void SelectMobSkill(MobSkillData selectedSkill)
        {
            var skillOpts = model.AllSkillOptions;
            var stateOpts = model.AllStateOptions;
            var targetOpts = model.AllTargetOptions;
            var conditionOpts = model.AllConditionOptions;

            view.SelectMobSkill(new SkillEditorView.MobSkillPresenterModel
            {
                SkillText = "Skill:",
                CurrentSkill = skillOpts[selectedSkill.SkillID].SkillIndex,
                SkillOptions = skillOpts.Values.Select(x => x.SkillName).ToArray(),
                OnSkillOptionChanged = (index) => { model.UpdateSkillID(selectedSkill.InstanceID, index); },

                SkillLevelText = "Skill Level:",
                SkillLevel = $"{selectedSkill.SkillLevel}",
                OnSkillLevelChanged = (level) => { model.UpdateSkillLevel(selectedSkill.InstanceID, ParseInt(level)); },

                RateText = "Chance:",
                Rate = $"{selectedSkill.Rate}",
                OnSkillRateChanged = (rate) => { model.UpdateSkillRate(selectedSkill.InstanceID, ParseInt(rate)); },

                CastTimeText = "Cast Time:",
                CastTime = $"{selectedSkill.CastTime}",
                OnSkillCastTimeChanged = (castTime) => { model.UpdateSkillCastTime(selectedSkill.InstanceID, ParseInt(castTime)); },

                DelayText = "Cooldown:",
                Delay = $"{selectedSkill.Delay}",
                OnSkillDelayChanged = (delay) => { model.UpdateSkillDelay(selectedSkill.InstanceID, ParseInt(delay)); },

                StateText = "State:",
                CurrentState = stateOpts[selectedSkill.State].StateIndex,
                StateOptions = stateOpts.Values.Select(x => x.StateName).ToArray(),
                OnSkillStateOptionChanged = (index) => { model.UpdateSkillState(selectedSkill.InstanceID, index); },

                TargetText = "Target:",
                CurrentTarget = targetOpts[selectedSkill.Target].TargetIndex,
                TargetOptions = targetOpts.Values.Select(x => x.TargetName).ToArray(),
                OnSkillTargetOptionChanged = (index) => { model.UpdateSkillTarget(selectedSkill.InstanceID, index); },

                ConditionText = "Condition:",
                CurrentCondition = conditionOpts[selectedSkill.Condition].ConditionIndex,
                ConditionOptions = conditionOpts.Values.Select(x => x.ConditionName).ToArray(),
                OnSkillConditionOptionChanged = (index) => { model.UpdateSkillCondition(selectedSkill.InstanceID, index); },

                ConditionValueText = "Condition Value:",
                ConditionValue = $"{selectedSkill.ConditionValue}",
                OnSkillConditionValueChanged = (conditionValue) => { model.UpdateSkillConditionValue(selectedSkill.InstanceID, ParseInt(conditionValue)); },

                EmotionText = "Emotion:",
                Emotion = $"{selectedSkill.Emotion}",
                OnSkillEmotionChanged = (emotion) => { model.UpdateSkillEmotion(selectedSkill.InstanceID, ParseInt(emotion)); },

                ChatText = "Chat:",
                Chat = $"{selectedSkill.Chat}",
                OnSkillChatChanged = (chat) => { model.UpdateSkillChat(selectedSkill.InstanceID, ParseInt(chat)); },

                Values = selectedSkill.Values.Select((x, i) => ($"Value {i + 1}:", $"{x}")).ToArray(),
                OnSkillValueChanged = (idx, value) => { model.UpdateSkillValue(selectedSkill.InstanceID, idx, ParseInt(value)); },

                Extras = selectedSkill.Extras.Select((x, i) => ($"Extra {i + 1}:", $"{x}")).ToArray(),
                OnSkillExtraChanged = (idx, extra) => { model.UpdateSkillExtra(selectedSkill.InstanceID, idx, ParseInt(extra)); },
            });
        }

        private void OnSelectMob(Mob mob)
        {
            MobSkillData[] skills = model.GetMobSkills(mob.ID);
            MobSkillData selectedSkill = skills.FirstOrDefault();

            selectedMobID = mob.ID;
            selectedMobSkillID = selectedSkill.SkillID;

            view.OnSelectMobData(mob.ID);
            SelectMob(mob, skills);
            SelectMobSkill(selectedSkill);
        }

        private void OnSelectMobSkill(MobSkillData skill)
        {
            selectedMobSkillID = skill.SkillID;

            view.OnSelectMobSkillData(skill.InstanceID.ToString());
            SelectMobSkill(skill);
        }

        private string GetMobSkillDataDisplayNameInList(MobSkillData data)
        {
            return $"[{data.State}] {data.GetDescriptionSkillName()} (Lv. {data.SkillLevel})";
        }

        private int ParseInt(string value)
        {
            return int.TryParse(value, out var result)
                ? result
                : 0;
        }

        private IEnumerator StopLoadingRoutine()
        {
            yield return new WaitForSeconds(0.2f);
            view.SetLoading(false);
            yield return null;
        }
    }
}