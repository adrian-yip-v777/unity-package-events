using System;
using vz777.Foundation;

namespace vz777.Events
{
    public abstract class EventBase : DisposableBase, IDisposableEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime Timestamp { get; } = DateTime.Now;
    }
}