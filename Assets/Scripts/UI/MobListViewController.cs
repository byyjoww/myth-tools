using ROTools.Mobs;
using SLS.UI;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ROTools.UI
{
    public class MobListViewController : ViewController<MobListView, MobProvider>
    {
        private InputPopupViewController popup = default;

        private int selectedMobID = -1;

        public event UnityAction<int> OnMobSelected;

        public MobListViewController(MobListView view, MobProvider model, InputPopupViewController popup) : base(view, model)
        {
            this.popup = popup;
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
                .OrderBy(x => x.ID)
                .ToArray();

            if (mobs.Length > 0)
            {
                selectedMobID = mobs.FirstOrDefault().ID;
            }

            OnMobSelected?.Invoke(selectedMobID);

            view.Setup(new MobListView.PresenterModel
            {
                AddMobText = "+",
                CanAddMob = model.IsLoaded,
                OnAddMob = delegate
                {
                    popup.Show("Mob ID:", TMPro.TMP_InputField.ContentType.IntegerNumber, "Add", hideOnSubmit: false, onSubmit: (val1) =>
                    {
                        popup.Show("Mob Name:", TMPro.TMP_InputField.ContentType.Standard, "Add", (val2) =>
                        {
                            int mobID = ParseInt(val1);
                            model.AddMob(mobID, val2);
                        });
                    });
                },

                CanSearchMob = model.IsLoaded,
                OnSearchMob = (txt) =>
                {
                    bool Evaluate(int mobID, string mobName)
                    {
                        if (string.IsNullOrWhiteSpace(txt))
                        {
                            return true;
                        }

                        return mobName.IndexOf(txt, StringComparison.OrdinalIgnoreCase) >= 0
                            || mobID.ToString().IndexOf(txt, StringComparison.OrdinalIgnoreCase) >= 0;
                    }

                    view.FilterMobs(Evaluate);
                },

                Mobs = mobs.Select(m => new MobDataView.PresenterModel
                {
                    ID = m.ID,
                    Name = m.GetDisplayName(),
                    OnSelect = delegate
                    {
                        selectedMobID = m.ID;
                        view.HighlightMobData(m.ID);
                        OnMobSelected?.Invoke(m.ID);
                    },
                    IsSelected = selectedMobID == m.ID,
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
    }
}