using System.Linq;
using System.Reflection;
using paramore.brighter.commandprocessor;
using paramore.brighter.commandprocessor.logging.Handlers;
using paramore.brighter.commandprocessor.policy.Handlers;
using TinyIoC;

namespace Brighter.AzureExtensions.Functions
{
    internal class HandlerConfig
    {
        private readonly MessageMapperFactory _mapperFactory;
        private readonly HandlerFactory _handlerFactory;

        public IAmASubscriberRegistry Subscribers => _handlerFactory;
        public IAmAHandlerFactory HandlerFactory => _handlerFactory;
        public IAmAMessageMapperRegistry MessageMapperRegistry => _mapperFactory;

        public HandlerConfiguration HandlerConfiguration => new HandlerConfiguration(Subscribers, _handlerFactory);

        public HandlerConfig()
        {
            _mapperFactory = new MessageMapperFactory();
            _handlerFactory = new HandlerFactory();
        }

        public void RegisterDefaultHandlers()
        {
            TinyIoCContainer.Current.Register(typeof(RequestLoggingHandler<>));
            TinyIoCContainer.Current.Register(typeof(ExceptionPolicyHandler<>));
        }

        public void Register<TRequest, THandler, TMapper>()
            where TRequest : class, IRequest
            where THandler : class, IHandleRequests<TRequest>
            where TMapper : class, IAmAMessageMapper<TRequest>
        {
            _handlerFactory.Register<TRequest, THandler>();
            _mapperFactory.Register<TRequest, TMapper>();
        }

        public void Register<TRequest, THandler>()
            where TRequest : class, IRequest
            where THandler : class, IHandleRequests<TRequest>
        {
            _handlerFactory.Register<TRequest, THandler>();
        }

        public void RegisterSubscribersFromAssembly(Assembly assembly)
        {
            var subscribers =
                from t in assembly.GetExportedTypes()
                where t.IsClass && !t.IsAbstract && !t.IsInterface
                from i in t.GetInterfaces()
                where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHandleRequests<>)
                select new { Request = i.GetGenericArguments().First(), Handler = t };

            foreach (var subscriber in subscribers)
            {
                _handlerFactory.Register(subscriber.Request, subscriber.Handler);
            }
        }

        public void RegisterMappersFromAssembly(Assembly assembly)
        {
            var candidateTypes = assembly.GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.ContainsGenericParameters && !t.IsInterface)
                .ToList();

            var mappers =
                from t in candidateTypes
                from i in t.GetInterfaces()
                where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAmAMessageMapper<>)
                select new { Message = i.GetGenericArguments().First(), Mapper = t };

            foreach (var mapper in mappers)
            {
                _mapperFactory.Register(mapper.Message, mapper.Mapper);
            }
        }
    }
}