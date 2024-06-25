using System;
using System.Collections.Generic;
using Factory;
using UnityEngine;

namespace UI{

    /// <summary>
    /// Template for a UI. you can rig the prefab for the UI frame itself and all the screens that should
    /// be instanced and registered upon instantiating a new UI frame
    /// </summary>
    [CreateAssetMenu(fileName = "UISettings", menuName = "UI/UI Settings")]
    public class UISettings : ScriptableObject
    {
        [Tooltip("Prefab for the UI frame structure itself")] [SerializeField]
        private UIFrame m_templateUIPrefab = null;

        [Tooltip(
            "Prefabs for all the screens (both Panels and Windows) that are to be instanced and registered when the UI in instantiated")]
        [SerializeField]
        private List<GameObject> m_screenToRegister = null;

        [Tooltip(
            "In Case a screen prefab is not deactivated, should the system automatically deactivate its GameObject upon instantiation?" +
            " if false the screen will be at a visible state upon instantiation")]
        [SerializeField]
        private bool m_deactivateScreenGos = true;
        private void OnValidate()
        {
            List<GameObject> objectsToRemove = new List<GameObject>();
            for (int i = 0; i < m_screenToRegister.Count; i++)
            {
                IUIScreenController screenCt1 = m_screenToRegister[i].GetComponent<IUIScreenController>();
                if(screenCt1 == null)
                    objectsToRemove.Add(m_screenToRegister[i]);
            }

            if (objectsToRemove.Count > 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Some GameObjects that were added to the screen prefab list did not have ScreenControllers attached to them Removing!");
#endif
                foreach (GameObject gameObject in objectsToRemove)
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"Removed {gameObject.name} from {name} as it has no screen controller Attached");
#endif
                    m_screenToRegister.Remove(gameObject);
                }
            }
        }
        
        public class UIFrameBuilder : IFactory<UIFrame>
        {
            private bool instanceAndRegisterScreens = true;
            private UISettings m_settings;

            public UIFrameBuilder(UISettings settings)
            {
                this.m_settings = settings;
            }
            
            // public UIFrameBuilder SetTemplatePrefab(UIFrame prefab)
            // {
            //     this.m_settings.m_templateUIPrefab = prefab;
            //     return this;
            // }
            
            public UIFrameBuilder SetInstanceAndRegister(bool instanceRegister = true)
            {
                this.instanceAndRegisterScreens = instanceRegister;
                return this;
            }

            public UIFrame Create()
            {
                UIFrame newUi = Instantiate(m_settings.m_templateUIPrefab);
                if (instanceAndRegisterScreens)
                {
                    foreach (GameObject screen in m_settings.m_screenToRegister)
                    {
                        GameObject screenInstance = Instantiate(screen);
                        IUIScreenController screenController = screenInstance.GetComponent<IUIScreenController>();
                        if (screenController != null)
                        {
                            newUi.RegisterScreen(screen.name, screenController, screenInstance.transform);
                            if(m_settings.m_deactivateScreenGos && screenInstance.activeSelf)
                                screenInstance.SetActive(false);
                        }
#if UNITY_EDITOR
                        else
                        {
                            Debug.LogError($"Screen does not contain a Screen Controller Skipping {screen.name}");
                        }        
#endif
                    }
                }
                return newUi;
            }
        }
    }
}