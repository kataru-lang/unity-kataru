namespace Kataru
{
    /// <summary>
    /// Also includes listeners for other Runner events
    /// </summary>
    public class HandlerExtended : Handler
    {
        protected override void OnEnable()
        {
            Runner.OnChoices += OnChoices;
            Runner.OnDialogueEnd += OnDialogueEnd;
            Runner.OnInvalidChoice += OnInvalidChoice;

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            Runner.OnChoices -= OnChoices;
            Runner.OnDialogueEnd -= OnDialogueEnd;
            Runner.OnInvalidChoice -= OnInvalidChoice;

            base.OnDisable();
        }

        protected virtual void OnChoices(Choices choices) { }
        protected virtual void OnDialogueEnd() { }
        protected virtual void OnInvalidChoice() { }
    }
}
