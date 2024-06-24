using System;

namespace DependencyInjection.attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProvideAttribute : Attribute
    {
        public ProvideAttribute() {}
    }
}