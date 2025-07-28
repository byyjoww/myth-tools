using SLS.Core.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace ROTools.UI
{
    [System.Serializable]
    public class ViewElementSpawner<T> where T : Component
    {
        [SerializeField] protected bool useScenePrefab = default;
        [SerializeField] protected T prefab = default;
        [ConditionalField("useScenePrefab", true)]
        [SerializeField] protected Transform parent = default;

        protected List<T> spawned = new List<T>();
        public IEnumerable<T> Spawned => spawned;

        public virtual IEnumerable<T> Spawn(int quantity)
        {
            Clear();
            return Create(quantity);
        }

        protected virtual IEnumerable<T> Create(int quantity)
        {
            prefab.gameObject.SetActive(true);

            Transform p = useScenePrefab
                ? prefab.transform.parent
                : parent;

            List<T> created = new List<T>();
            for (int i = 0; i < quantity; i++)
            {
                T obj = UnityEngine.Object.Instantiate(prefab, p);
                spawned.Add(obj);
                created.Add(obj);
            }

            prefab.gameObject.SetActive(false);
            return created;
        }

        protected virtual void Clear()
        {
            foreach (var building in spawned)
            {
                UnityEngine.Object.Destroy(building.gameObject);
            }

            spawned.Clear();
        }
    }
}