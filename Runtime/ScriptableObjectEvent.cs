using System;
using UnityEngine;

namespace vz777.Events
{
    /// <summary>
    /// Scriptable object event for you to inherit.
    /// </summary>
    public abstract class ScriptableObjectEvent : ScriptableObject, IEvent
    {
        public Guid Id { get; private set; }
        public DateTime Timestamp { get; private set; }

        protected virtual void Awake()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.Now;
        }
    }
}