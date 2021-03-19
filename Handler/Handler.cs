using System;
using System.Collections.Generic;
using UnityEngine;

/*
The Runner owns dictionary of delegates.
All annotated functions are added to the delegates.
When the runner 
*/
namespace Kataru
{
    /// <summary>
    /// The Handler class automatically manages subscription and disposal of listeners to the Runner.
    /// All MonoBehaviors that need to access data from Kataru should inherit from Handler and
    /// register their methods using CommandHandler and CharacterHandler attributes.
    /// </summary>
    public class Handler : Attributed
    {
        [SerializeField] protected Runner Runner;

        // Keep local copies of delegates so they may be removed on Disable.
        protected DelegateMap CommandDelegates = new DelegateMap();
        protected DelegateMap CharacterDelegates = new DelegateMap();

        /// <summary>
        /// Adds listeners for all attributed methods in this subclass.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (Runner == null) throw new NullReferenceException($"Kataru Runner was null for game object '{gameObject.name}'");

            Runner.OnChoices += OnChoices;
            Runner.OnDialogueEnd += OnDialogueEnd;
            Runner.OnInvalidChoice += OnInvalidChoice;

            foreach (var namedDelegate in GetActionsForAttribute<CommandHandler>())
            {
                CommandDelegates.Add(namedDelegate.name, namedDelegate.@delegate);
                Runner.CommandDelegates.Add(namedDelegate.name, namedDelegate.@delegate);
            }

            foreach (var namedDelegate in GetActionsForAttribute<CharacterHandler>())
            {
                CharacterDelegates.Add(namedDelegate.name, namedDelegate.@delegate);
                Runner.CharacterDelegates.Add(namedDelegate.name, namedDelegate.@delegate);
            }
        }

        /// <summary>
        /// Removes listeners for all methods registered in OnEnable.
        /// </summary>
        protected virtual void OnDisable()
        {
            Runner.OnChoices -= OnChoices;
            Runner.OnDialogueEnd -= OnDialogueEnd;
            Runner.OnInvalidChoice -= OnInvalidChoice;

            foreach (var pair in CommandDelegates)
            {
                foreach (var @delegate in pair.Value)
                {
                    Runner.CommandDelegates.Remove(pair.Key, @delegate);
                }
            }

            foreach (var pair in CharacterDelegates)
            {
                foreach (var @delegate in pair.Value)
                {
                    Runner.CharacterDelegates.Remove(pair.Key, @delegate);
                }
            }

            CommandDelegates.Clear();
            CharacterDelegates.Clear();
        }

        protected virtual void OnChoices(Choices choices) { }
        protected virtual void OnDialogueEnd() { }
        protected virtual void OnInvalidChoice() { }
    }
}
