using SLS.Core.Extensions;
using SLS.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace ROTools.UI
{
    public class MobListView : View
    {
        public struct PresenterModel
        {
            public string AddMobText { get; set; }
            public bool CanAddMob { get; set; }
            public UnityAction OnAddMob { get; set; }

            public bool CanSearchMob { get; set; }
            public UnityAction<string> OnSearchMob { get; set; }

            public IEnumerable<MobDataView.PresenterModel> Mobs { get; set; }
        }        

        [SerializeField] private TMP_Text addMobText = default;
        [SerializeField] private SLSButton addMob = default;
        [SerializeField] private TMP_InputField search = default;
        [SerializeField] private ViewElementPoolSpawner<MobDataView> mobDataViewPool = default;

        private Dictionary<int, MobDataView> spawnedMobData = new Dictionary<int, MobDataView>();
        private MobDataView selectedMobData = default;

        public void Setup(PresenterModel model)
        {
            addMobText.text = model.AddMobText;
            SetButtonAction(addMob, model.OnAddMob, model.CanAddMob);

            SetupInputField(search, string.Empty, model.OnSearchMob, model.CanSearchMob);

            spawnedMobData.Clear();
            ClearSelectedMobData();

            LoadMobs(model.Mobs);
        }

        public void FilterMobs(Func<int, bool> evaluateFunc)
        {
            spawnedMobData.ForEach(kvp =>
            {
                int mobID = kvp.Key;
                bool isActive = evaluateFunc(mobID);
                kvp.Value.gameObject.SetActive(isActive);
            });
        }

        public void HighlightMobData(int id)
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

        private void SetupInputField(TMP_InputField input, string value, UnityAction<string> onValueChanged, bool canInteract)
        {
            input.interactable = canInteract;
            input.onValueChanged.RemoveAllListeners();
            input.text = value;
            input.onValueChanged.AddListener(onValueChanged);
        }
    }
}