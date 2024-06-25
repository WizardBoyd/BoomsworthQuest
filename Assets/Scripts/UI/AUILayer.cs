using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public abstract class AUILayer<TScreen> : MonoBehaviour where TScreen : IUIScreenController
    {
        protected Dictionary<string, TScreen> m_registeredScreens;

        /// <summary>
        /// Shows a screen
        /// </summary>
        /// <param name="screen">The SceneController to Show</param>
        public abstract void ShowScreen(TScreen screen);

        /// <summary>
        /// Shows a screen passing in properties
        /// </summary>
        /// <param name="screen">The SceneController to Show</param>
        /// <param name="properties">The data payload</param>
        /// <typeparam name="TProps">the type of the data payload</typeparam>
        public abstract void ShowScreen<TProps>(TScreen screen, TProps properties) where TProps : IScreenProperties;

        /// <summary>
        /// Hides a screen
        /// </summary>
        /// <param name="screen">The ScreenController to be hidden</param>
        public abstract void HideScreen(TScreen screen);

        /// <summary>
        /// Initialize this layer
        /// </summary>
        public virtual void Initialize() => m_registeredScreens = new Dictionary<string, TScreen>();

        /// <summary>
        /// Reparents the screen to this layer's transform
        /// </summary>
        /// <param name="controller">The screen controller</param>
        /// <param name="screenTransform">The Screen Transform</param>
        public virtual void ReparentScreen(IUIScreenController controller, Transform screenTransform) => screenTransform.SetParent(transform,false);

        /// <summary>
        /// Register a ScreenController to a specific ScreenId
        /// </summary>
        /// <param name="screenID">Target ScreenId</param>
        /// <param name="controller">Screen Controller to be registered</param>
        public void RegisterScreen(string screenID, TScreen controller)
        {
            if (!m_registeredScreens.ContainsKey(screenID))
                ProcessScreenRegister(screenID, controller);
#if UNITY_EDITOR
            else
                Debug.LogWarning($"Screen Controller already registered for id {screenID}");
#endif
        }
        

        /// <summary>
        /// Attempts to find a registed screen that matches the id
        /// </summary>
        /// <param name="screenID"></param>
        /// <param name="controller"></param>
        public void UnregisterScreen(string screenID, TScreen controller)
        {
            if (m_registeredScreens.ContainsKey(screenID))
                ProcessScreenUnregister(screenID, controller);
#if UNITY_EDITOR
            else
                Debug.LogWarning($"Screen Controller not registered for id {screenID}");
#endif
        }

        /// <summary>
        /// Attempts to find a registered screen that matches the id
        /// and show it
        /// </summary>
        /// <param name="screenID">The desired Screen Id</param>
        public void ShowScreenById(string screenID)
        {
            TScreen ct1;
            if (m_registeredScreens.TryGetValue(screenID, out ct1))
                ShowScreen(ct1);
#if UNITY_EDITOR
            else 
                Debug.LogWarning($"Screen ID {screenID} not registered to this layer");
#endif
        }

        /// <summary>
        /// Attempts to find a registered screen that matches the id
        /// and show it, passing a data payload
        /// </summary>
        /// <param name="screenID">The desired screen ID (by default its the name of the prefab)</param>
        /// <param name="properties">The data payload for this screen to use</param>
        public void ShowScreenById<TProps>(string screenID, TProps properties) where TProps : IScreenProperties
        {
            TScreen ct1;
            if (m_registeredScreens.TryGetValue(screenID, out ct1))
                ShowScreen(ct1, properties);
#if UNITY_EDITOR
            else
                Debug.LogWarning($"Screen ID {screenID} not registered to this layer");
#endif
        }

        /// <summary>
        /// Attempts to find a registered screen that matches the id
        /// and hide it
        /// </summary>
        /// <param name="screenID">The desired screen ID to hide</param>
        public void HideScreenById(string screenID)
        {
            TScreen ct1;
            if (m_registeredScreens.TryGetValue(screenID, out ct1))
                HideScreen(ct1);
#if UNITY_EDITOR
            Debug.LogWarning($"Could not hide Screen ID {screenID} not registered to this layer");
#endif
        }

        /// <summary>
        /// Checks if a screen is registered to this UI layer
        /// </summary>
        /// <param name="screenID">The desired screen ID (by default its the name of the prefab)</param>
        /// <returns>True if screen is registered, false if not</returns>
        public bool IsScreenRegistered(string screenID) => m_registeredScreens.ContainsKey(screenID);

        /// <summary>
        /// Hide all screens registered to this layer
        /// </summary>
        /// <param name="shouldAnimateWhenHiding">should the screen animate while hiding</param>
        public virtual void HideAll(bool shouldAnimateWhenHiding = true)
        {
            foreach (var (id, screen) in m_registeredScreens)
            {
                screen.Show();
            }
        }
        
        protected virtual void ProcessScreenRegister(string screenID, TScreen controller)
        {
            controller.ScreenId = screenID;
            m_registeredScreens.Add(screenID, controller);
            controller.ScreenDestroyed += OnScreenDestroyed;
        }
        

        protected virtual void ProcessScreenUnregister(string screenID, TScreen controller)
        {
            controller.ScreenDestroyed -= OnScreenDestroyed;
            m_registeredScreens.Remove(screenID);
        }
        
        private void OnScreenDestroyed(IUIScreenController screen)
        {
            if(!string.IsNullOrWhiteSpace(screen.ScreenId) && m_registeredScreens.ContainsKey(screen.ScreenId))
                UnregisterScreen(screen.ScreenId, (TScreen)screen);
        }
    }
}