using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace Kataru
{
    /// <summary>
    /// Generic map of named events.
    /// In example, to represent the mapping of character names to the methods to handle their dialogues,
    /// we create an EventMap of where T = Dialogue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventMap<T> : Dictionary<string, UnityEvent<T>>
    {
        public void Add(string name, UnityAction<T> listener)
        {
            UnityEvent<T> @event = null;
            if (TryGetValue(name, out @event))
            {
                @event.AddListener(listener);
            }
            else
            {
                @event = new UnityEvent<T>();
                @event.AddListener(listener);
                Add(name, @event);
            }
        }

        public void Add(UnityAction<T> listener) => Add(listener.Method.Name, listener);

        public void Remove(string name, UnityAction<T> listener)
        {
            UnityEvent<T> @event = null;
            if (TryGetValue(name, out @event))
            {
                @event.RemoveListener(listener);
            }
        }

        public void Remove(UnityAction<T> listener) => Remove(listener.Method.Name, listener);

        public void Invoke(string name, T args)
        {
            UnityEvent<T> @event = null;
            if (TryGetValue(name, out @event))
            {
                @event.Invoke(args);
            }
            else
            {
                Debug.LogError($"'{name}' had no listeners.");
            }
        }
    }

    /// <summary>
    /// Generic map of named actions.
    /// Thsi class is used to keep track of owned handler actions in Handler classes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ActionMap<T> : Dictionary<string, UnityAction<T>>
    {
        public void Add(UnityAction<T> action) => Add(action.Method.Name, action);
        public ActionMap() : base() { }
    }
}
