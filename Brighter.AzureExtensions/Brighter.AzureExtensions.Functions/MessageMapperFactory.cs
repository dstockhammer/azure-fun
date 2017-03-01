using System;
using paramore.brighter.commandprocessor;
using TinyIoC;

namespace Brighter.AzureExtensions.Functions
{
    internal class MessageMapperFactory : IAmAMessageMapperFactory, IAmAMessageMapperRegistry
    {
        public IAmAMessageMapper Create(Type messageMapperType)
        {
            return (IAmAMessageMapper)TinyIoCContainer.Current.Resolve(messageMapperType);
        }

        public IAmAMessageMapper<T> Get<T>() where T : class, IRequest
        {
            return TinyIoCContainer.Current.Resolve<IAmAMessageMapper<T>>();
        }

        public void Register<TRequest, TMessageMapper>()
            where TRequest : class, IRequest
            where TMessageMapper : class, IAmAMessageMapper<TRequest>
        {
            TinyIoCContainer.Current.Register<IAmAMessageMapper<TRequest>, TMessageMapper>();
        }

        public void Register(Type message, Type mapper)
        {
            TinyIoCContainer.Current.Register(typeof(IAmAMessageMapper<>).MakeGenericType(message), mapper);
        }
    }
}