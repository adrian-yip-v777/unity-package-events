using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using vz777.Foundations;
using UnityEngine;

namespace vz777.Events
{
    /// <summary>
    /// A typical, simplest event bus that handles events at one place to lose couples for your system while using Observer Pattern.
    /// This event bus supports events with interfaces that empowers you to subscribe to interface for more controls.
    /// </summary>
    public class EventBus : DisposableBase
    {
        /// <summary>
        /// A list of events and their subscription types.
        /// </summary>
        private readonly ConcurrentDictionary<Type, List<(object Handler, int Priority)>> _handlers = new();

        /// <summary>
        /// Subscribe to a specific type or interface
        /// </summary>
        public void Subscribe<TEvent>(Action<TEvent> handler, Priority priority = Priority.Unset) where TEvent : IEvent
        {
            Subscribe(handler, (int)priority);
        }
        
        public void Subscribe<TEvent>(Action<TEvent> handler, int priority) where TEvent : IEvent
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            var subscriptionType = typeof(TEvent);

            _handlers.AddOrUpdate(
                key: subscriptionType,
                addValue: new List<(object, int)> { (handler, priority) },
                updateValueFactory: (_, existingHandlers) =>
                {
                    existingHandlers.Add((handler, priority));
                    return existingHandlers;
                });
        }

        /// <summary>
        /// Unsubscribe from a specific type or interface.
        /// </summary>
        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null) 
                throw new ArgumentNullException(nameof(handler));

            var subscriptionType = typeof(TEvent);
            
            if (!_handlers.TryGetValue(subscriptionType, out var events)) 
                return;
            
            events.RemoveAll(eventSet => (Action<TEvent>)eventSet.Handler == handler);

            if (events.Count == 0)
                _handlers.TryRemove(subscriptionType, out _);
        }

        /// <summary>
        /// Publish an event to its type and all interfaces.
        /// </summary>
        public void Publish<TEvent>(TEvent @event, bool autoDispose = true) where TEvent : IEvent
        {
            if (@event == null) 
                throw new ArgumentNullException(nameof(@event));
    
            var disposableEvent = @event as IDisposableEvent;
            if (disposableEvent?.IsDisposed == true) 
                throw new InvalidOperationException("Cannot publish a disposed event");

            var eventType = typeof(TEvent);

            var allTypes = eventType.GetInterfaces()
                .Where(i => typeof(IEvent).IsAssignableFrom(i))
                .Concat(new[] { eventType });

            var events = allTypes
                .Where(type => _handlers.ContainsKey(type))
                .SelectMany(type => _handlers[type])
                .OrderByDescending(eventSet => eventSet.Priority)
                .ToArray();

            if (!events.Any()) return;

            foreach (var eventSet in events)
            {
                if (disposableEvent?.IsDisposed == true) break;
                (eventSet.Handler as Action<TEvent>).Invoke(@event);
            }

            if (autoDispose) 
                disposableEvent?.Dispose();
        }
    }
}