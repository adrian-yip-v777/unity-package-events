using System;
using vz777.Events;
using NUnit.Framework;

// Define test interfaces and classes within the test namespace
public interface ITestEvent : IEvent
{
    string Message { get; }
}

public class TestEvent : EventBase, ITestEvent
{
    public TestEvent(string message) => Message = message;
    public string Message { get; }
}

[TestFixture]
public class EventBusTests
{
    private EventBus _eventBus;

    [SetUp]
    public void SetUp()
    {
        _eventBus = new EventBus();
    }

    [TearDown]
    public void TearDown()
    {
        // Dispose if DisposableBase requires it; otherwise, just nullify
        _eventBus = null;
    }

    [Test]
    [TestCase(1)]
    [TestCase(5)]
    public void Subscribe_To_Event_Should_Call_Handler(int numberOfSubscriptions)
    {
        var callCount = 0;
        Action<TestEvent> handler = _ => callCount++;

        for (var i = 0; i < numberOfSubscriptions; i++)
            _eventBus.Subscribe(handler);
        
        _eventBus.Publish(new TestEvent("Hello, EventBus!"));

        Assert.AreEqual(numberOfSubscriptions, callCount, $"Expected {numberOfSubscriptions} call, got {callCount}.");
        TestContext.WriteLine($"Subscription: Handler called {numberOfSubscriptions} as expected.");
    }

    [Test]
    [TestCase(1)]
    [TestCase(5)]
    public void Unsubscribe_From_Event_Should_Not_Call_Handler(int numberOfSubscriptions)
    {
        var callCount = 0;
        Action<TestEvent> handler = _ => callCount++;

        for (var i = 0; i < numberOfSubscriptions; i++)
            _eventBus.Subscribe(handler);
            
        _eventBus.Publish(new TestEvent("First publish"));
        
        for (var i = 0; i < numberOfSubscriptions; i++)
            _eventBus.Unsubscribe(handler);
        
        _eventBus.Publish(new TestEvent("Second publish"));

        Assert.AreEqual(numberOfSubscriptions, callCount, $"Expected {numberOfSubscriptions} call, got {callCount}.");
        TestContext.WriteLine($"Unsubscription: Handler called {numberOfSubscriptions} as expected.");
    }

    [Test]
    public void Subscribe_With_Higher_Priority_Should_Be_Called_First()
    {
        var result = "";
        Action<TestEvent> lowPriority = _ => result += "Low ";
        Action<TestEvent> highPriority = _ => result += "High ";

        // Low priority
        _eventBus.Subscribe(lowPriority, priority: 0); 
        // High priority
        _eventBus.Subscribe(highPriority, priority: 10); 
        _eventBus.Publish(new TestEvent("Priority test"));

        Assert.AreEqual("High Low ", result, $"Expected 'High Low ', got '{result}'.");
        TestContext.WriteLine("Priority Ordering: Handlers executed in correct order (High -> Low).");
    }

    [Test]
    public void Subscribe_To_Interface_Should_Call_Handler()
    {
        var interfaceHandlerCalled = false;
        Action<ITestEvent> handler = e =>
        {
            interfaceHandlerCalled = true;
            TestContext.WriteLine($"Interface Subscription: {e.Message}");
        };

        _eventBus.Subscribe(handler);
        _eventBus.Publish(new TestEvent("Interface event"));

        Assert.IsTrue(interfaceHandlerCalled, "Interface handler was not called.");
    }

    [Test]
    public void Publish_Disposed_Event_Should_Throw_Error ()
    {
        var callCount = 0;
        Action<TestEvent> handler = _ => callCount++;

        _eventBus.Subscribe(handler);
        var disposableEvent = new TestEvent("Disposable event");
        _eventBus.Publish(disposableEvent, autoDispose: false);
        disposableEvent.Dispose();

        Assert.Throws<InvalidOperationException>(() => _eventBus.Publish(disposableEvent), "Expected exception for disposed event, but none thrown.");
        Assert.AreEqual(1, callCount, $"Expected 1 call, got {callCount}.");
        TestContext.WriteLine("Disposable Event: Correctly threw exception for disposed event.");
    }

    [Test]
    public void Publish_Event_Should_Dispose_It_By_Default()
    {
        Action<TestEvent> handler = _ => { };
        _eventBus.Subscribe(handler);
        
        var testEvent = new TestEvent("Hello, EventBus!");
        _eventBus.Publish(testEvent);
        
        Assert.IsTrue(testEvent.IsDisposed);
    }

    [Test]
    public void Publish_Event_With_Auto_Dispose_Off_Should_Not_Dispose_It()
    {
        Action<TestEvent> handler = _ => { };
        _eventBus.Subscribe(handler);
        
        var testEvent = new TestEvent("Hello, EventBus!");
        _eventBus.Publish(testEvent, autoDispose: false);
        
        Assert.IsFalse(testEvent.IsDisposed);
    }

    [Test]
    public void Unsubscribe_From_Event_In_Multiple_Subscriptions_Should_Not_Call_Handler()
    {
        var callCount = 0;
        Action<TestEvent> handler1 = _ => callCount++;
        Action<TestEvent> handler2 = _ => callCount++;
        Action<TestEvent> handler3 = _ => callCount++;

        _eventBus.Subscribe(handler1);
        _eventBus.Subscribe(handler2);
        _eventBus.Subscribe(handler3);
            
        _eventBus.Publish(new TestEvent("First publish"));
        _eventBus.Unsubscribe(handler3);
        _eventBus.Publish(new TestEvent("Second publish"));
        _eventBus.Unsubscribe(handler2);
        _eventBus.Publish(new TestEvent("Third publish"));
        _eventBus.Unsubscribe(handler1);
        _eventBus.Publish(new TestEvent("Last publish"));

        Assert.AreEqual(6, callCount, $"Expected {6} call, got {callCount}.");
        TestContext.WriteLine($"Unsubscription: Handler called 6 as expected.");
    }
}