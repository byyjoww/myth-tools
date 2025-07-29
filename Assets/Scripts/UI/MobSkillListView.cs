using SLS.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ROTools.UI
{
    public class MobSkillListView : View
    {
        public struct PresenterModel
        {
            public IEnumerable<MobSkillDataView.PresenterModel> Skills { get; set; }

            public string AddSkillText { get; set; }
            public bool CanAddSkill { get; set; }
            public UnityAction OnAddSkill { get; set; }

            public string CopySkillText { get; set; }
            public bool CanCopySkill { get; set; }
            public UnityAction OnCopySkill { get; set; }

            public string DeleteSkillText { get; set; }
            public bool CanDeleteSkill { get; set; }
            public UnityAction OnDeleteSkill { get; set; }

            public string ClearSkillsText { get; set; }
            public bool CanClearSkills { get; set; }
            public UnityAction OnClearSkills { get; set; }
        }

        [SerializeField] private TMP_Text addSkillText = default;
        [SerializeField] private SLSButton addSkill = default;
        [SerializeField] private TMP_Text copySkillText = default;
        [SerializeField] private SLSButton copySkill = default;
        [SerializeField] private TMP_Text deleteSkillText = default;
        [SerializeField] private SLSButton deleteSkill = default;
        [SerializeField] private TMP_Text clearSkillsText = default;
        [SerializeField] private SLSButton clearSkills = default;
        [SerializeField] private ViewElementPoolSpawner<MobSkillDataView> mobSkillDataViewPool = default;

        private Dictionary<string, MobSkillDataView> spawnedMobSkillData = new Dictionary<string, MobSkillDataView>();
        private MobSkillDataView selectedMobSkillData = default;

        public void Setup(PresenterModel model)
        {
            spawnedMobSkillData.Clear();
            ClearSelectedMobSkillData();

            addSkillText.text = model.AddSkillText;
            SetButtonAction(addSkill, model.OnAddSkill, model.CanAddSkill);

            copySkillText.text = model.CopySkillText;
            SetButtonAction(copySkill, model.OnCopySkill, model.CanCopySkill);

            deleteSkillText.text = model.DeleteSkillText;
            SetButtonAction(deleteSkill, model.OnDeleteSkill, model.CanDeleteSkill);

            clearSkillsText.text = model.ClearSkillsText;
            SetButtonAction(clearSkills, model.OnClearSkills, model.CanClearSkills);

            LoadSkills(model.Skills);
        }

        public void UpdateMobSkillDataInList(string instanceID, string name)
        {
            if (spawnedMobSkillData.TryGetValue(instanceID, out MobSkillDataView view))
            {
                view.Setup(name);
            }
        }

        public void HighlightMobSkillData(string skillInstanceID)
        {
            if (spawnedMobSkillData.TryGetValue(skillInstanceID, out MobSkillDataView view))
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