using System;

namespace vz777.Events
{
    public interface IEvent 
    {
        Guid Id { get; }
        DateTime Timestamp { get; }
    }
}