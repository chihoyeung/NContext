namespace NContext.Tests.Specs.EventHandling
{
    using System;

    using Machine.Specifications;

    using Telerik.JustMock;

    public class with_a_synchronous_event_handler : when_raising_an_event
    {
        Establish context = () =>
        {
            Mock.Arrange(() => ActivationProvider.CreateInstance<SynchronousEvent>(Arg.IsAny<Type>()))
                .Returns((Type handlerType) => HandlerFactory(handlerType));

            _Event = new SynchronousEvent("synchronous");
        };

        Because of = async () => await EventManager.Raise(_Event).Await().AsTask;

        It should_handle_event = () => HandledEvents.ShouldContain(_Event);

        private static SynchronousEvent _Event;
    }
}