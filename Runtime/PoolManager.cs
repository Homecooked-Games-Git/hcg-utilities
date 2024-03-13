using System.Collections.Generic;
using DG.Tweening;
using HCG.Extensions;
using UnityEngine;

namespace HCGames.Utilities
{

    public class PoolManager : Singleton<PoolManager>
    {
        [SerializeField] private GameObject[] initObjects;
        private readonly Dictionary<string, object> _pools = new();


        protected override void Awake()
        {
            base.Awake();
            foreach (var go in initObjects)
            {
                var instantiatedGo = Instantiate(go, Vector3.zero, Quaternion.identity, transform);
                instantiatedGo.SetActive(true);
                DOVirtual.DelayedCall(0.25f, () =>
                {
                    instantiatedGo.SetActive(false);
                    Destroy(instantiatedGo);
                });
            }
        }

        public T Get<T>(T prefab, Vector3 pos, Quaternion rotation, Transform parent, bool setActive = true)
            where T : Object
        {
            var key = typeof(T).Name + prefab.name;
            _pools.TryAdd(key, new GenericPool<T>(prefab));
            return ((GenericPool<T>)_pools[key]).Get(pos, rotation, parent, setActive);
        }

        public void Release<T>(T obj, float delay = 0) where T : Object
        {
            var key = typeof(T).Name + obj.name;
            if (!_pools.ContainsKey(key)) return;
            ((GenericPool<T>)_pools[key]).Release(obj, delay);
        }

        public void DestroyPool<T>(T obj) where T : Object // Don't use! Not reliable since objects that are not in the pool are not going to be destroyed resulting in a leak! Could use event system. 
        {
            var key = typeof(T).Name + obj.name;
            _pools.TryGetValue(key, out var pool);
            if (pool == null) return;
            ((GenericPool<T>)pool).Destroy();
            _pools.Remove(key);
        }

        private class GenericPool<T> where T : Object
        {
            private readonly Stack<T> _pool = new();
            private readonly T _prefab;

            public GenericPool(T prefab)
            {
                _prefab = prefab;
            }

            public T Get(Vector3 pos, Quaternion rotation, Transform parent, bool setActive = true)
            {
                T obj;
                if (_pool.Count == 0)
                {
                    obj = Instantiate(_prefab);
                    obj.name = _prefab.name;
                }
                else
                {
                    obj = _pool.Pop();
                }

                var go = obj switch
                {
                    GameObject gameObject => gameObject,
                    Component comp => comp.gameObject,
                    _ => null
                };
                if (go != null)
                {
                    var objTransform = go.transform;
                    objTransform.SetParent(parent);
                    objTransform.position = pos;
                    objTransform.rotation = rotation;
                    go.SetActive(setActive);
                }

                return obj;
            }

            public void Destroy()
            {
                foreach (var obj in _pool)
                {
                    var go = obj switch
                    {
                        GameObject gameObject => gameObject,
                        Component comp => comp.gameObject,
                        _ => null
                    };
                    if(go == null) continue;
                    Object.Destroy(go);
                }
            }

            public void Release(T obj, float delay)
            {
                var go = obj switch
                {
                    GameObject gameObject => gameObject,
                    Component comp => comp.gameObject,
                    _ => null
                };
                if (delay == 0)
                {
                    OnRelease(obj,go);
                    return;
                }
                DOVirtual.DelayedCall(delay, () => OnRelease(obj,go)).SetUpdate(true).SetLink(go,LinkBehaviour.KillOnDestroy);
            }

            private void OnRelease(T obj, GameObject go)
            {
                if (go != null)
                {
                    go.SetActive(false);
                    go.transform.SetParent(instance.transform);
                }

                _pool.Push(obj);
            }
        }
    }
}