using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Misc.ExtensionMethods
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if(source == null)
                Debug.LogException(new NullReferenceException());
            if(action == null)
                Debug.LogException(new NullReferenceException());

            foreach(var element in source)
            {
                action(element);
            }
        }

        public static int GetNullSafeHashCode<T>(this T value) where T : class
        {
            return value?.GetHashCode() ?? 1;
        }

        public static int GetConcatenateHash(params string[] list)
        {
            int hash = 17;
            foreach (string s in list)
            {
                hash = hash * 23 + s.GetHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Concatenates multiple strings to see if a hash equals
        /// </summary>
        /// <returns>true if the same</returns>
        public static bool HashEqualsString(int hash, params string[] list)
        {
            int concatantedHash = GetConcatenateHash(list);
            return hash == concatantedHash;
        }

        public static HashSet<GameObject> FindObjectsOfTypes(params System.Type[] types)
        {
            HashSet<GameObject> objects = new HashSet<GameObject>();

            foreach (Type type in types)
            {
                if(!type.IsSubclassOf(typeof(Component)))
                    continue;
                Component[] foundComponents = Object.FindObjectsByType(type, FindObjectsInactive.Exclude, FindObjectsSortMode.None) as Component[];
                foreach (Component component in foundComponents)
                {
                    if (component != null)
                    {
                        objects.Add(component.gameObject);
                    }
                }
            }

            return objects;
        }

        public static void RemoveAllComponentsExcept(this GameObject gameObject, params Type[] typesToKeep)
        {
            Component[] components = gameObject.GetComponents<Component>();

            foreach (Component component in components)
            {
                //check if the component type is in the types to keep list
                bool keepComponent = false;
                foreach (Type type in typesToKeep)
                {
                    if (component.GetType() == type)
                    {
                        keepComponent = true;
                        break;
                    }
                }

                if (!keepComponent && !(component is Transform))
                {
                    Object.Destroy(component);
                }
            }
        }

        public static void RemoveAllComponentsFromChildrenAndSelf(this GameObject gameObject, params Type[] typesToKeep)
        {
            gameObject.RemoveAllComponentsExcept(typesToKeep);
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject childObj = gameObject.transform.GetChild(i).gameObject;
                childObj.RemoveAllComponentsFromChildrenAndSelf(typesToKeep);
            }
        }

        public static void AssignAllGameObjectAndChildrenToLayer(this GameObject gameObject,int layer)
        {
            gameObject.layer = layer;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.AssignAllGameObjectAndChildrenToLayer(layer);
            }
        }

        public static void AssignGameObjectToLayer(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
        }
    }
}