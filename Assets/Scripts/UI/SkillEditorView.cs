using ROTools.Skills;
using SLS.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ROTools.UI
{
    public class SkillEditorView : View
    {
        public struct SetupPresenterModel
        {
            public string LoadText { get; set; }
            public bool CanLoad { get; set; }
            public UnityAction OnLoad { get; set; }
            public string SaveText { get; set; }
            public bool CanSave { get; set; }
            public UnityAction OnSave { get; set; }
        }

        public struct DBPresenterModel
        {
            public IEnumerable<MobDataView.PresenterModel> Mobs { get; set; }
        }

        public struct MobPresenterModel
        {
            public string NameText { get; set; }
            public string Name { get; set; }
            public IEnumerable<MobSkillDataView.PresenterModel> Skills { get; set; }
        }

        public struct MobSkillPresenterModel
        {
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

        [Header("Common")]
        [SerializeField] private Image loadingPanel = default;
        [SerializeField] private GameObject leftBar = default;
        [SerializeField] private GameObject rightArea = default;

        [Header("Mob List")]
        [SerializeField] private TMP_Text loadText = default;
        [SerializeField] private SLSButton load = default;
        [SerializeField] private TMP_Text saveText = default;
        [SerializeField] private SLSButton save = default;
        [SerializeField] private ViewElementPoolSpawner<MobDataView> mobDataViewPool = default;

        [Header("Mob Skill List")]
        [SerializeField] private ViewElementPoolSpawner<MobSkillDataView> mobSkillDataViewPool = default;

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

        private Dictionary<int, MobDataView> spawnedMobData = new Dictionary<int, MobDataView>();
        private MobDataView selectedMobData = default;

        private Dictionary<string, MobSkillDataView> spawnedMobSkillData = new Dictionary<string, MobSkillDataView>();
        private MobSkillDataView selectedMobSkillData = default;

        public void Setup(SetupPresenterModel model)
        {
            rightArea.SetActive(false);

            loadText.text = model.LoadText;
            SetButtonAction(load, model.OnLoad, model.CanLoad);

            saveText.text = model.SaveText;
            SetButtonAction(save, model.OnSave, model.CanSave);

            spawnedMobData.Clear();
            ClearSelectedMobData();

            spawnedMobSkillData.Clear();
            ClearSelectedMobSkillData();

            LoadMobs(new MobDataView.PresenterModel[0]);
            LoadSkills(new MobSkillDataView.PresenterModel[0]);
        }

        public void SelectSkillDB(DBPresenterModel model)
        {
            rightArea.SetActive(true);

            spawnedMobData.Clear();
            ClearSelectedMobData();

            spawnedMobSkillData.Clear();
            ClearSelectedMobSkillData();

            LoadMobs(model.Mobs);
        }

        public void SelectMob(MobPresenterModel model)
        {
            spawnedMobSkillData.Clear();
            ClearSelectedMobSkillData();

            mobName.Setup(model.NameText, model.Name);
            LoadSkills(model.Skills);
        }

        public void SelectMobSkill(MobSkillPresenterModel model)
        {
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

        public void UpdateMobSkillDataInList(string instanceID, string name)
        {
            if (spawnedMobSkillData.TryGetValue(instanceID, out MobSkillDataView view))
            {
                view.Setup(name);
            }
        }

        public void SetLoading(bool isLoading)
        {
            loadingPanel.gameObject.SetActive(isLoading);
        }

        public void OnSelectMobData(int id)
        {
            if (spawnedMobData.TryGetValue(id, out MobDataView view))
            {
                ClearSelectedMobData();
                view.SetSelected(true);
                selectedMobData = view;
            }
        }

        private void ClearSelectedMobData()
        {
            if (selectedMobData != null)
            {
                selectedMobData.SetSelected(false);
                selectedMobData = null;
            }
        }

        private void LoadMobs(IEnumerable<MobDataView.PresenterModel> mobs)
        {
            int numOfElements = mobs.Count();
            var views = mobDataViewPool.Set(numOfElements);
            for (int i = 0; i < numOfElements; i++)
            {
                var view = views.ElementAt(i);
                var data = mobs.ElementAt(i);
                view.Setup(data);

                // cache it for updating later
                spawnedMobData.Add(data.ID, view);
                if (data.IsSelected)
                {
                    ClearSelectedMobData();
                    selectedMobData = view;
                }
            }
        }

        public void OnSelectMobSkillData(string id)
        {
            if (spawnedMobSkillData.TryGetValue(id, out MobSkillDataView view))
            {
                ClearSelectedMobSkillData();
                view.SetSelected(true);
                selectedMobSkillData = view;
            }
        }

        private void ClearSelectedMobSkillData()
        {
            if (selectedMobSkillData != null)
            {
                selectedMobSkillData.SetSelected(false);
                selectedMobSkillData = null;
            }
        }

        private void LoadSkills(IEnumerable<MobSkillDataView.PresenterModel> skills)
        {
            int numOfElements = skills.Count();
            var views = mobSkillDataViewPool.Set(numOfElements);
            for (int i = 0; i < numOfElements; i++)
            {
                var view = views.ElementAt(i);
                var data = skills.ElementAt(i);
                view.Setup(data);

                // cache it for updating later
                spawnedMobSkillData.Add(data.InstanceID, view);
                if (data.IsSelected)
                {
                    ClearSelectedMobSkillData();
                    selectedMobSkillData = view;
                }
            }
        }
    }
}