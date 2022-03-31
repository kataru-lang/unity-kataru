using UnityEngine;
using System.IO;

namespace Kataru
{
    /// <summary>
    /// Kataru Manager class.
    /// This class is in charge of initializing the Runner.
    /// Also includes examples of reacting to Runner's events.
    /// </summary>
    public class Manager : Handler
    {

        protected virtual void Awake()
        {
#if UNITY_EDITOR
            if (Runner.isInitialized) Debug.LogError(@"A Kataru Manager was already initialized.
                You should only have one Kataru Manager in your scene.");
#endif
            Runner.Init();
        }
    }
}