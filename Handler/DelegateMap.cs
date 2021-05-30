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
                foreach (var pair in delegates)
                {
                    bool autoNext = pair.Value;
                    Delegate @delegate = pair.Key;

                    try
                    {
                        @delegate.DynamicInvoke(args);
                        if (autoNext)
                        {
                            Debug.Log($"AutoNext from '{name}'");
                            Runner.Next(auto: true);
                        }
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
            }
            else
            {
                Debug.LogError($"'{name}' had no listeners.");
            }
        }
    }
}
