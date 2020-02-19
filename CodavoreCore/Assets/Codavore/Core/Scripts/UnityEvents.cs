// <copyright file="UnityEvents.cs" company="Codavore, LLC">
//     Copyright (c) Codavore, LLC. All rights reserved.
// </copyright>

namespace Codavore.Core
{
    /// <summary>
    /// Provides a series of event hooks to listen for key Unity events.
    /// </summary>
    public interface IUnityEvents
    {
        /// <summary>
        /// Allows a hook to be placed for the Awake event.
        /// </summary>
        /// <param name="listener">The event or action that will react to the event.</param>
        /// <param name="wireup">If false, this will unwire an event call.</param>
        void ListenForAwake(System.Action listener, bool wireup = true);

        /// <summary>
        /// Allows a hook to be placed for the Start event.
        /// </summary>
        /// <param name="listener">The event or action that will react to the event.</param>
        /// <param name="wireup">If false, this will unwire an event call.</param>
        void ListenForStart(System.Action listener, bool wireup = true);

        /// <summary>
        /// Allows a hook to be placed for the Update event.
        /// </summary>
        /// <param name="listener">The event or action that will react to the event.</param>
        /// <param name="wireup">If false, this will unwire an event call.</param>
        void ListenForUpdate(System.Action listener, bool wireup = true);
    }

    /// <summary>
    /// Provides Methods to trigger Unity Events.
    /// </summary>
    public interface IUnityEventsCaller
    {
        /// <summary>
        /// Calls the Start event.
        /// </summary>
        void Start();

        /// <summary>
        /// Calls the Awake event;
        /// </summary>
        void Awake();

        /// <summary>
        /// Calls the Update event;
        /// </summary>
        void Update();
    }

    /// <summary>
    /// Service for listening to events.
    /// </summary>
    public class UnityEvents : IUnityEvents, IUnityEventsCaller
    {
        /// <summary>
        /// Action to tie start handlers to.
        /// </summary>
        private System.Action onStart = () => { };

        /// <summary>
        /// Action to tie awake handlers to.
        /// </summary>
        private System.Action onAwake = () => { };

        /// <summary>
        /// Action to tie update handlers to.
        /// </summary>
        private System.Action onUpdate = () => { };

        /// <summary>
        /// Calls the Awake events
        /// </summary>
        public void Awake()
        {
            this.onAwake();
        }

        /// <summary>
        /// Wires up an awake listener
        /// </summary>
        /// <param name="listener">The event to call on awake.</param>
        /// <param name="wireup">When false, will unwire this listener.</param>
        public void ListenForAwake(System.Action listener, bool wireup = true)
        {
            if (wireup)
            {
                this.onAwake += listener;
            }
            else
            {
                this.onAwake -= listener;
            }
        }

        /// <summary>
        /// Wires up an start listener
        /// </summary>
        /// <param name="listener">The event to call on start.</param>
        /// <param name="wireup">When false, will unwire this listener.</param>
        public void ListenForStart(System.Action listener, bool wireup = true)
        {
            if (wireup)
            {
                this.onStart += listener;
            }
            else
            {
                this.onStart -= listener;
            }
        }

        /// <summary>
        /// Wires up an update listener
        /// </summary>
        /// <param name="listener">The event to call on update.</param>
        /// <param name="wireup">When false, will unwire this listener.</param>
        public void ListenForUpdate(System.Action listener, bool wireup = true)
        {
            if (wireup)
            {
                this.onUpdate += listener;
            }
            else
            {
                this.onUpdate -= listener;
            }
        }

        /// <summary>
        /// Calls the start events
        /// </summary>
        public void Start()
        {
            this.onStart();
        }

        /// <summary>
        /// Calls the update events
        /// </summary>
        public void Update()
        {
            this.onUpdate();
        }
    }
}
