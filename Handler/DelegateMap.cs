using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace Kataru
{
    /// <summary>
    /// </summary>
    public class DelegateMap : Dictionary<string, List<Delegate>>
    {
        public void Add(string name, Delegate @delegate)
        {
            if (!ContainsKey(name))
            {
                this[name] = new List<Delegate>();
            }

            this[name].Add(@delegate);
        }

        public void Remove(string name, Delegate @delegate)
        {
            List<Delegate> delegates;
            if (TryGetValue(name, out delegates))
            {
                delegates.Remove(@delegate);
            }
        }

        public void Invoke(string name, object[] args)
        {
            List<Delegate> delegates;
            if (TryGetValue(name, out delegates))
            {
                foreach (var @delegate in delegates)
                {
                    @delegate.DynamicInvoke(args);
                }
            }
            else
            {
                Debug.LogError($"'{name}' had no listeners.");
            }
        }
    }
}
