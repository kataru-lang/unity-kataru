using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Kataru
{
    /// <summary>
    /// </summary>
    public class DelegateMap : Dictionary<string, ConcurrentDictionary<Delegate, byte>>
    {

        public void Add(string name, Delegate @delegate)
        {
            if (!ContainsKey(name))
            {
                this[name] = new ConcurrentDictionary<Delegate, byte>();
            }

            this[name].TryAdd(@delegate, 0);
        }

        public void Remove(string name, Delegate @delegate)
        {
            ConcurrentDictionary<Delegate, byte> delegates;
            if (TryGetValue(name, out delegates))
            {
                byte removed;
                delegates.TryRemove(@delegate, out removed);
            }
        }

        public void Invoke(string name, object[] args)
        {
            ConcurrentDictionary<Delegate, byte> delegates;
            if (TryGetValue(name, out delegates))
            {
                foreach (var @delegate in delegates.Keys)
                {
                    try
                    {
                        @delegate.DynamicInvoke(args);
                    }
                    catch (System.Reflection.TargetInvocationException e)
                    {
                        Debug.LogError($"Error calling '{name}': {e.InnerException}");
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
