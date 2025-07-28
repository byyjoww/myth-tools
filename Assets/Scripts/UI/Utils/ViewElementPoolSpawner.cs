using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ROTools.UI
{
    [System.Serializable]
    public class ViewElementPoolSpawner<T> : ViewElementSpawner<T> where T : Component
    {
        private IEnumerable<T> inactive => spawned.Where(x => !x.gameObject.activeSelf);

        public IEnumerable<T> Set(int quantity)
        {
            Create(quantity - spawned.Count);

            for (int i = 0; i < spawned.Count; i++)
            {
                if (!IsNullOrBeingDestroyed(spawned[i]))
                {
                    spawned[i]?.gameObject.SetActive(i < quantity);
                }
            }
            return spawned;
        }

        public override IEnumerable<T> Spawn(int quantity)
        {
            int numOfElementsFromPool = Mathf.Min(quantity, inactive.Count());
            int numOfElementsToCreate = quantity - inactive.Count();
            var created = Create(numOfElementsToCreate);
            var pool = inactive.Take(numOfElementsFromPool);
            return created.Concat(pool).ToList();
        }

        public void Return(List<T> objects)
        {
            objects.ForEach(x =>
            {
                if (!IsNullOrBeingDestroyed(x))
                {
                    x?.gameObject.SetActive(false);
                }
            });
        }

        public T Spawn()
        {
            return Spawn(1).FirstOrDefault();
        }

        public void Return(T _object)
        {
            Return(new List<T>() { _object });
        }

        private bool IsNullOrBeingDestroyed(T spawned)
        {
            return !spawned ||
                !spawned?.gameObject ||
                spawned is null ||
                spawned.gameObject is null ||
                object.ReferenceEquals(spawned, null) ||
                object.ReferenceEquals(spawned.gameObject, null);
        }
    }
}