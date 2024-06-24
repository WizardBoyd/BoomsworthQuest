using System;

namespace DependencyInjection
{
    public class NotFoundProviderException : Exception
    {
        public NotFoundProviderException(string message) : base(message)
        {
        }
    }

    public class InjectionFailedException : Exception
    {
        public InjectionFailedException(string message) : base(message){}
    }
}