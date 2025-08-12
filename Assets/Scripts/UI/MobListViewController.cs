using ROTools.Mobs;
using ROTools.Utils;
using SLS.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ROTools.UI
{
    public class MobListViewController : ViewController<MobListView, IMobSkillDBMobProvider>
    {
        private InputPopupViewController popup = default;

        private Dictionary<Mob.EClass, string[]> classSearchOptsDict = default;
        private int selectedMobID = -1;

        public event UnityAction<int> OnMobSelected;

        public MobListViewController(MobListView view, IMobSkillDBMobProvider model, InputPopupViewController popup) : base(view, model)
        {
            this.popup = popup;
            classSearchOptsDict = CreateClassSearchOpts();
        }

        protected override void OnInit()
        {
            model.OnValueChanged += Reload;
        }

        protected override void OnDispose()
        {
            model.OnValueChanged -= Reload;
        }

        protected override void OnShow()
        {
            Reload();
        }

        protected override void OnHide()
        {

        }

        private void Reload()
        {
            selectedMobID = -1;

            Mob[] mobs = model.Mobs.Values
                .OrderBy(x => x.Id)
                .ToArray();

            if (mobs.Length > 0)
            {
                selectedMobID = mobs.FirstOrDefault().Id;
            }

            OnMobSelected?.Invoke(selectedMobID);

            view.Setup(new MobListView.PresenterModel
            {
                AddMobText = "+",
                CanAddMob = false,
                OnAddMob = delegate
                {
                    popup.Show("Mob ID:", TMPro.TMP_InputField.ContentType.IntegerNumber, "Add", hideOnSubmit: false, onSubmit: (val1) =>
                    {
                        popup.Show("Mob Name:", TMPro.TMP_InputField.ContentType.Standard, "Add", (val2) =>
                        {
                            int mobID = ParseInt(val1);
                            (model as MobProvider).AddMob(new MobData
                            {
                                Id = mobID,
                                Name = val2,
                            });
                        });
                    });
                },

                CanSearchMob = model.IsLoaded,
                OnSearchMob = (txt) =>
                {
                    view.FilterMobs((mobID) =>
                    {
                        return model.Mobs.TryGetValue(mobID, out var mob)
                            && ShouldInclude(mob, txt);
                    });
                },

                Mobs = mobs.Select(m => new MobDataView.PresenterModel
                {
                    ID = m.Id,
                    Name = m.GetDisplayName(),
                    OnSelect = delegate
                    {
                        selectedMobID = m.Id;
                        view.HighlightMobData(m.Id);
                        OnMobSelected?.Invoke(m.Id);
                    },
                    IsSelected = selectedMobID == m.Id,
                }).ToArray()
            });

            Show();
        }

        private int ParseInt(string value)
        {
            return int.TryParse(value, out var result)
                ? result
                : 0;
        }

        private bool ShouldInclude(Mob mob, string txt)
        {
            if (string.IsNullOrWhiteSpace(txt))
            {
                return true;
            }

            var searchOpts = GetSearchOpts(mob);
            return searchOpts.Any(x => x.ContainsText(txt));
        }

        private IEnumerable<string> GetSearchOpts(Mob mob)
        {
            var defaultSearchOpts = new string[]
            {
                mob.Name,
                mob.Id.ToString(),
            };

            return classSearchOptsDict.TryGetValue(mob.Class, out var classSearchOpts)
                ? defaultSearchOpts.Concat(classSearchOpts)
                : defaultSearchOpts;
        }

        private Dictionary<Mob.EClass, string[]> CreateClassSearchOpts()
        {
            return new Dictionary<Mob.EClass, string[]>()
            {
                { Mob.EClass.Unknown, new string[] { Mob.EClass.Unknown.ToString() } },
                { Mob.EClass.Normal, new string[] { Mob.EClass.Normal.ToString() } },
                { Mob.EClass.Elite, new string[] { Mob.EClass.Elite.ToString() } },
                { Mob.EClass.Rare, new string[] { Mob.EClass.Rare.ToString() } },
                { Mob.EClass.Mini, new string[] { Mob.EClass.Mini.ToString(), "miniboss", "boss" } },
                { Mob.EClass.Mvp, new string[] { Mob.EClass.Mvp.ToString(), "boss" } },
                { Mob.EClass.All, new string[] { Mob.EClass.All.ToString(), } },
                { Mob.EClass.Boss, new string[] { Mob.EClass.Boss.ToString(), } },
                { Mob.EClass.NonBoss, new string[] { Mob.EClass.NonBoss.ToString(), } },
            };
        }
    }
}