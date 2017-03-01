using System;
using System.Collections.Generic;
using paramore.brighter.commandprocessor;
using TinyIoC;

namespace Brighter.AzureExtensions.Functions
{
    internal class HandlerFactory : IAmAHandlerFactory, IAmASubscriberRegistry
    {
        private readonly SubscriberRegistry _registry;

        public HandlerFactory()
        {
            _registry = new SubscriberRegistry();
        }

        public IHandleRequests Create(Type handlerType)
        {
            return (IHandleRequests)TinyIoCContainer.Current.Resolve(handlerType);
        }

        public void Release(IHandleRequests handler)
        {
            // nothing to do
        }

        public IEnumerable<Type> Get<T>() where T : class, IRequest
        {
            return _registry.Get<T>();
        }

        public void Register<TRequest, TImplementation>()
            where TRequest : class, IRequest
            where TImplementation : class, IHandleRequests<TRequest>
        {
            TinyIoCContainer.Current.Register<TImplementation>();
            _registry.Register<TRequest, TImplementation>();
        }

        public void Register(Type request, Type handler)
        {
            TinyIoCContainer.Current.Register(handler);
            _registry.Add(request, handler);
        }
    }
}