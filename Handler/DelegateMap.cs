using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Kataru
{
    /// <summary>
    /// </summary>
    public class DelegateMap : Dictionary<string, ConcurrentDictionary<Delegate, bool>>
    {

        public void Add(string name, Delegate @delegate, bool autoNext)
        {
            if (!ContainsKey(name))
            {
                this[name] = new ConcurrentDictionary<Delegate, bool>();
            }

            this[name].TryAdd(@delegate, autoNext);
        }

        public void Remove(string name, Delegate @delegate)
        {
            ConcurrentDictionary<Delegate, bool> delegates;
            if (TryGetValue(name, out delegates))
            {
                bool removed;
                delegates.TryRemove(@delegate, out removed);
            }
        }

        public void Invoke(string name, object[] args)
        {
            ConcurrentDictionary<Delegate, bool> delegates;
            if (TryGetValue(name, out delegates))
            {
                bool autoNext = true;
                foreach (var pair in delegates)
                {
                    autoNext &= pair.Value;
                    Delegate @delegate = pair.Key;

                    try
                    {
                        Debug.Log($"Del name: '{@delegate.Method.Name}'");
                        @delegate.DynamicInvoke(args);
                    }
                    catch (System.Reflection.TargetInvocationException e)
                    {
                        Debug.LogError($"Error calling '{name}'.");
                        throw e.InnerException;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error calling '{name}'. Make sure all listener arguments match the definition in Kataru: {e}");
                    }
                }
                if (autoNext)
                {
                    Debug.Log($"AutoNext from '{name}'");
                    Runner.Next(auto: true);
                }
            }
            else
            {
                Debug.LogError($"'{name}' had no listeners.");
            }
        }
    }
}
