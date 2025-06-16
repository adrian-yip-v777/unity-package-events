# Changelog
## 0.4.0
- Support parameterless handler subscription
- Fixed an issue on type matching
- Fixed an event not published for the base types.

## 0.3.2
- Added back the link.xml.meta

## 0.3.1
- Prevent any type being stripped during build time.

## 0.3.0
- Support PublishAsync (experimental feature)
- Simply return boolean on Publish function to indicate if there is any subscribed handlers to the published event.

## 0.2.2
- Fixed event unsubscription not working with event wrapper.
- Add basic unit tests for the event bus.

## 0.2.1
- Fixed highest priority not working properly.

## 0.2.0
- Added priority on event subscription.

## 0.1.0
- Added an event bus (supports interface and thread-safe)
- **IEvent**, **EventBase**, **ScriptableObjectEvent** are what you need for creating an event. 