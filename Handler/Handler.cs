using System;
using UnityEngine;

/*
The Runner owns dictionary of delegates.
All annotated functions are added to the delegates.
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
        // Keep local copies of delegates so they may be removed on Disable.
        protected DelegateMap CommandDelegates = new DelegateMap();
        protected DelegateMap CharacterDelegates = new DelegateMap();

        /// <summary>
        /// Adds listeners for all attributed methods in this subclass.
        /// </summary>
        protected virtual void OnEnable()
        {
            Runner.OnChoices += OnChoices;
            Runner.OnDialogueEnd += OnDialogueEnd;
            Runner.OnInvalidChoice += OnInvalidChoice;

            foreach (var namedDelegate in GetActionsForAttribute<CommandHandler>())
            {
                CommandDelegates.Add(namedDelegate.name, namedDelegate.@delegate, namedDelegate.autoNext);
                Runner.CommandDelegates.Add(namedDelegate.name, namedDelegate.@delegate, namedDelegate.autoNext);
            }

            foreach (var namedDelegate in GetActionsForAttribute<CharacterHandler>())
            {
                CharacterDelegates.Add(namedDelegate.name, namedDelegate.@delegate, namedDelegate.autoNext);
                Runner.CharacterDelegates.Add(namedDelegate.name, namedDelegate.@delegate, namedDelegate.autoNext);
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
                foreach (var @delegate in pair.Value.Keys)
                {
                    Runner.CommandDelegates.Remove(pair.Key, @delegate);
                }
            }

            foreach (var pair in CharacterDelegates)
            {
                foreach (var @delegate in pair.Value.Keys)
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
