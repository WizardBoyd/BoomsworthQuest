using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DependencyInjection.attributes;
using Misc.Singelton;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace DependencyInjection
{
    
    [DefaultExecutionOrder(-1000)]
    public class Injector : MonoBehaviorSingleton<Injector>
    {
        private const BindingFlags m_bindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly Dictionary<Type, object> m_registry
            = new Dictionary<Type, object>();

        
        static MonoBehaviour[] findMonoBehaviors() {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
        }

        private void Start()
        {
            PerformInjection();
        }

        public void PerformInjection()
        {
            var provides = findMonoBehaviors().OfType<IDependencyProvider>();
            foreach (IDependencyProvider provider in provides) {
                RegisterProvider(provider);
            }
            
            //Find all Injectable objects and inject their dependencies
            IEnumerable<MonoBehaviour> injectables = findMonoBehaviors().Where(IsInjectable);
            foreach (MonoBehaviour injectable in injectables)
            {
                Inject(injectable);
            }
        }

        public void PerformInjection(MonoBehaviour injectable)
        {
            Debug.Assert(IsInjectable(injectable), $"Object {injectable.name} is not Injectable");
            Inject(injectable);
        }

        private void Inject(object injectable)
        {
            Type type = injectable.GetType();
            IEnumerable<FieldInfo> injectableFields = type.GetFields(m_bindingFlags).Where(member => 
                Attribute.IsDefined(member, typeof(InjectAttribute)));
            
            foreach (FieldInfo injectableField in injectableFields)
            {
                Type fieldType = injectableField.FieldType;
                object resolvedInstance = Resolve(fieldType);
                if (resolvedInstance == null)
                {
                    throw new InjectionFailedException($"Failed to inject {fieldType.Name} into {type.Name}");
                }
                injectableField.SetValue(injectable, resolvedInstance);
#if UNITY_EDITOR
                //Debug.Log($"Field Injected {fieldType.Name} into {type.Name}");
#endif
            }

            IEnumerable<MethodInfo> injectableMethods = type.GetMethods(m_bindingFlags)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
            
            foreach (MethodInfo injectableMethod in injectableMethods)
            {
                Type[] requiredParameters = injectableMethod.GetParameters()
                    .Select(parameter => parameter.ParameterType).ToArray();
                object[] resolvedInstances = requiredParameters.Select(Resolve).ToArray();
                if (resolvedInstances.Any(resolvedInstance => resolvedInstance == null))
                {
                    throw new InjectionFailedException($"Failed to Inject {type.Name}.{injectableMethod.Name}");
                }

                injectableMethod.Invoke(injectable, resolvedInstances);
#if UNITY_EDITOR
                Debug.Log($"Method Injected {type.Name} . {injectableMethod.Name}");
#endif
            }
        }

        private object Resolve(Type fieldType)
        {
            m_registry.TryGetValue(fieldType, out object resolvedInstance);
            return resolvedInstance;
        }

        private void RegisterProvider(IDependencyProvider provider)
        {
            MethodInfo[] methods = provider.GetType().GetMethods(m_bindingFlags);

            foreach (MethodInfo method in methods)
            {
                if(!Attribute.IsDefined(method, typeof(ProvideAttribute)))
                    continue;
                Type returnType = method.ReturnType;
                var providedInstance = method.Invoke(provider, null);
                if (providedInstance != null)
                {
                    m_registry.TryAdd(returnType, providedInstance);
#if UNITY_EDITOR
                //Debug.Log($"Registered {returnType.Name} from {provider.GetType().Name}");    
#endif
                }
                else
                {
                    throw new NotFoundProviderException(
                        $"{provider.GetType().Name} returned null for {returnType.Name}");
                }
            }
        }

        static bool IsInjectable(MonoBehaviour obj)
        { 
            MemberInfo[] members = obj.GetType().GetMembers(m_bindingFlags);
            return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        }
    }
}