using System;

namespace vz777.Events
{
    public interface IDisposableEvent : IEvent, IDisposable
    {
        bool IsDisposed { get; }
    }
}