using UnityEngine;

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

        protected ActionMap<Command> CommandActions = new ActionMap<Command>();
        protected ActionMap<Dialogue> CharacterActions = new ActionMap<Dialogue>();

        /// <summary>
        /// Adds listeners for all attributed methods in this subclass.
        /// </summary>
        protected virtual void OnEnable()
        {
            Runner.OnChoices += OnChoices;
            Runner.OnDialogueEnd += OnDialogueEnd;

            foreach (var namedAction in GetActionsForAttribute<Command, CommandHandler>())
            {
                CommandActions.Add(namedAction.name, namedAction.action);
                Runner.Commands.Add(namedAction.name, namedAction.action);
            }

            foreach (var namedAction in GetActionsForAttribute<Dialogue, CharacterHandler>())
            {
                CharacterActions.Add(namedAction.name, namedAction.action);
                Runner.Characters.Add(namedAction.name, namedAction.action);
            }
        }

        /// <summary>
        /// Removes listeners for all methods registered in OnEnable.
        /// </summary>
        protected virtual void OnDisable()
        {
            Runner.OnChoices -= OnChoices;
            Runner.OnDialogueEnd -= OnDialogueEnd;

            foreach (var pair in CommandActions)
            {
                Runner.Commands.Remove(pair.Key, pair.Value);
            }

            foreach (var pair in CharacterActions)
            {
                Runner.Characters.Remove(pair.Key, pair.Value);
            }
        }

        protected virtual void OnChoices(Choices choices) { }
        protected virtual void OnDialogueEnd() { }
    }
}
