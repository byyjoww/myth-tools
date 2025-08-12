using ROTools.Skills;
using SLS.UI;
using System;
using System.Linq;
using UnityEngine.Events;

namespace ROTools.UI
{
    public class MobSkillListViewController : ViewController<MobSkillListView, SkillEditor>
    {
        private SkillProvider skillProvider = default;
        private InputPopupViewController popup = default;

        private int selectedMobID = -1;
        private Guid selectedMobSkillInstanceID = Guid.Empty;

        public event UnityAction<Guid> OnMobSkillSelected;

        public MobSkillListViewController(MobSkillListView view, SkillEditor model, SkillProvider skillProvider, InputPopupViewController popup) : base(view, model)
        {
            this.skillProvider = skillProvider;
            this.popup = popup;
        }

        protected override void OnInit()
        {
            model.OnMobSkillAdded += OnMobSkillAdded;
            model.OnMobSkillChanged += OnMobSkillChanged;
            model.OnMobSkillRemoved += OnMobSkillRemoved;
            model.OnMobSkillsCleared += OnMobSkillsCleared;
            model.OnValueChanged += OnSkillEditorValueChanged;
        }

        protected override void OnDispose()
        {
            model.OnMobSkillAdded -= OnMobSkillAdded;
            model.OnMobSkillChanged -= OnMobSkillChanged;
            model.OnMobSkillRemoved -= OnMobSkillRemoved;
            model.OnMobSkillsCleared -= OnMobSkillsCleared;
            model.OnValueChanged -= OnSkillEditorValueChanged;
        }

        protected override void OnShow()
        {
            
        }

        protected override void OnHide()
        {
            selectedMobID = -1;
        }

        public void Show(int mobID)
        {
            MobSkillData[] skills = model.GetMobSkillData(mobID);
            Guid mobSkillInstanceID = GetSelectedMobSkillInstanceID(skills);
            Show(mobID, mobSkillInstanceID, skills);
        }

        private void Show(int mobID, Guid mobSkillInstanceID)
        {
            MobSkillData[] skills = model.GetMobSkillData(mobID);
            Show(mobID, mobSkillInstanceID, skills);
        }

        private void Show(int mobID, Guid mobSkillInstanceID, MobSkillData[] skills)
        {
            selectedMobID = mobID;
            selectedMobSkillInstanceID = mobSkillInstanceID;
            OnMobSkillSelected?.Invoke(selectedMobSkillInstanceID);
            
            view.Setup(new MobSkillListView.PresenterModel
            {
                Skills = skills.Select(s => new MobSkillDataView.PresenterModel
                {
                    InstanceID = s.InstanceID.ToString(),
                    Name = GetMobSkillDataDisplayNameInList(s),
                    OnSelect = delegate
                    {
                        selectedMobSkillInstanceID = s.InstanceID;
                        view.HighlightMobSkillData(selectedMobSkillInstanceID.ToString());
                        OnMobSkillSelected?.Invoke(selectedMobSkillInstanceID);
                    },
                    IsSelected = selectedMobSkillInstanceID == s.InstanceID,
                }).ToArray(),

                AddSkillText = "Add",
                CanAddSkill = true,
                OnAddSkill = delegate
                {
                    popup.Show("Skill ID:", TMPro.TMP_InputField.ContentType.IntegerNumber, "Add", hideOnSubmit: false, onSubmit: (val1) =>
                    {
                        int skillID = ParseInt(val1);
                        if (!skillProvider.AllSkillOptions.ContainsKey(skillID))
                        {
                            popup.Show("Skill Name:", TMPro.TMP_InputField.ContentType.Standard, "Add", (val2) =>
                            {
                                skillProvider.AddSkill(new SkillData
                                {
                                    Id = skillID,
                                    Name = val2,
                                    TargetType = SkillData.ETargetType.Unknown.ToString(),
                                });
                                model.AddMobSkillData(mobID, skillID);
                            });
                        }
                        else
                        {
                            model.AddMobSkillData(mobID, skillID);
                            popup.Hide();
                        }
                    });
                },

                CopySkillText = "Copy",
                CanCopySkill = false,
                OnCopySkill = delegate
                {

                },

                DeleteSkillText = "Delete",
                CanDeleteSkill = selectedMobSkillInstanceID != Guid.Empty,
                OnDeleteSkill = delegate
                {
                    model.DeleteSkill(selectedMobSkillInstanceID);
                },

                ClearSkillsText = "Clear",
                CanClearSkills = skills.Length > 0,
                OnClearSkills = delegate
                {
                    model.ClearSkills(mobID);
                },
            });

            Show();
        }

        private Guid GetSelectedMobSkillInstanceID(MobSkillData[] skills)
        {
            return skills.Length > 0 
                ? skills.FirstOrDefault().InstanceID 
                : Guid.Empty;
        }

        private void OnMobSkillChanged(Guid instanceID)
        {
            if (model.MobSkillData.TryGetValue(instanceID, out var data) && data.MobID == selectedMobID)
            {                
                view.UpdateMobSkillDataInList(instanceID.ToString(), GetMobSkillDataDisplayNameInList(data));
            }
        }

        private void OnMobSkillAdded(Guid instanceID)
        {
            if (model.MobSkillData.TryGetValue(instanceID, out var data) && data.MobID == selectedMobID)
            {
                Show(data.MobID);
            }
        }

        private void OnMobSkillRemoved(Guid instanceID)
        {
            if (model.MobSkillData.TryGetValue(instanceID, out var data) && data.MobID == selectedMobID)
            {
                Show(data.MobID);
            }
        }

        private void OnMobSkillsCleared(int mobID)
        {
            if (mobID == selectedMobID)
            {
                Show(mobID);
            }
        }

        private void OnSkillEditorValueChanged()
        {
            if (selectedMobSkillInstanceID == Guid.Empty)
            {
                Show(selectedMobID);
            }
            else
            {
                Show(selectedMobID, selectedMobSkillInstanceID);
            }
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
    }
}