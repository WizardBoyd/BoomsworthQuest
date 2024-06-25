using System;
using UI.Panel;
using UI.Window;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// This is the centralized access point for all things UI
    /// All the calls should be directed at this
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class UIFrame : MonoBehaviour
    {
        /// <summary>
        /// Should the UI frame initialize itself on the awake callback
        /// </summary>
        [SerializeField]
        private bool InitializeOnAwake = true;

        private PanelUILayer m_panelLayer;
        private WindowUILayer m_windowLayer;

        private Canvas m_maincanvas;
        private GraphicRaycaster m_graphicRaycaster;

        public Canvas MainCanvas
        {
            get
            {
                if (m_maincanvas == null)
                    m_maincanvas = GetComponent<Canvas>();
                return m_maincanvas;
            }
        }

        public Camera UICamera
        {
            get => MainCanvas.worldCamera;
        }

        private void Awake()
        {
            if (InitializeOnAwake)
                Initialize();
        }

        /// <summary>
        /// Initializes this UI Frame. Initialization consists of initializing both the panel and window layers
        /// although generally all cases are covered by the "window and panel" approach this has been made virtual in case
        /// it ever needs additional UI layers or another special Initialization
        /// </summary>
        public virtual void Initialize()
        {
            if (m_panelLayer == null)
            {
                m_panelLayer = gameObject.GetComponentInChildren<PanelUILayer>(true);
                if (m_panelLayer == null)
                {
#if UNITY_EDITOR
                Debug.LogError($"UI Frame lacks Panel Layer");    
#endif
                }
                else
                {
                    m_panelLayer.Initialize();
                }
            }
            
            if (m_windowLayer == null)
            {
                m_windowLayer = gameObject.GetComponentInChildren<WindowUILayer>(true);
                if (m_panelLayer == null)
                {
#if UNITY_EDITOR
                    Debug.LogError($"UI Frame lacks Window Layer");    
#endif
                }
                else
                {
                    m_windowLayer.Initialize();
                    m_windowLayer.RequestScreenBlock += OnRequestScreenBlock;
                    m_windowLayer.RequestScreenUnBlock += OnRequestScreenUnBlock;
                }
            }
            m_graphicRaycaster = MainCanvas.GetComponent<GraphicRaycaster>();
        }

        
        /// <summary>
        /// Shows a panel by its id, passing no Properties
        /// </summary>
        /// <param name="screenID">Panel Id</param>
        public void ShowPanel(string screenID) => m_panelLayer.ShowScreenById(screenID);

        /// <summary>
        /// Shows a panel by its id, passing parameters
        /// </summary>
        /// <param name="screenID">Identifier</param>
        /// <param name="properties">Properties</param>
        /// <typeparam name="T">The type of properties passed in</typeparam>
        public void ShowPanel<T>(string screenID, T properties) where T : IScreenProperties => m_panelLayer.ShowScreenById<T>(screenID, properties);

        /// <summary>
        /// Hides the panel with the given id
        /// </summary>
        /// <param name="screenID">Identifier</param>
        public void HidePanel(string screenID) => m_panelLayer.HideScreenById(screenID);

        /// <summary>
        /// Opens a window with the given Id, with no properties
        /// </summary>
        /// <param name="screenId"></param>
        public void OpenWindow(string screenId) => m_windowLayer.ShowScreenById(screenId);

        /// <summary>
        /// Closes the window with the given Id, with no properties
        /// </summary>
        /// <param name="screenId"></param>
        public void CloseWindow(string screenId) => m_windowLayer.HideScreenById(screenId);

        /// <summary>
        /// Closes the currently open window, if any is open
        /// </summary>
        public void CloseCurrentWindow()
        {
            if(m_windowLayer.CurrentWindow != null)
                CloseWindow(m_windowLayer.CurrentWindow.ScreenId);
        }

        /// <summary>
        /// Opens the window with the given id, passing in Properties
        /// </summary>
        /// <param name="screenId">Identifier</param>
        /// <param name="properties">Properties</param>
        /// <typeparam name="T">The type of properties to be passed in</typeparam>
        public void OpenWindow<T>(string screenId, T properties) where T : IScreenProperties => m_windowLayer.ShowScreenById<T>(screenId, properties);

        /// <summary>
        /// Searches for the given id among the layers, opens the screen if it finds it
        /// </summary>
        /// <param name="screenId">The screen id</param>
        public void ShowScreen(string screenId)
        {
            Type type;
            if (IsScreenRegistered(screenId, out type))
            {
                //Assume the default will be a panel but it could be window so have that first
                if(type == typeof(IWindowController))
                    OpenWindow(screenId);
                else if(type == typeof(IUIScreenController))
                    ShowPanel(screenId);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError($"Tried to open Screen id {screenId} but its not registered as a window or panel");
            }
#endif
        }

        /// <summary>
        /// Registers a screen if transform is passed, the layer will
        /// re-parent it to itself. Screens can only be shown after being registered
        /// </summary>
        /// <param name="screenID">Screen Identifier</param>
        /// <param name="controller">Controller</param>
        /// <param name="screenTransform">Screen Transform. if not null, will be re-parented to proper layer</param>
        public void RegisterScreen(string screenID, IUIScreenController controller, Transform screenTransform)
        {
            if (controller is IWindowController window)
            {
                m_windowLayer.RegisterScreen(screenID, window);
                if (screenTransform != null)
                    m_windowLayer.ReparentScreen(controller, screenTransform);
                return;
            }

            if (controller != null)
            {
                m_panelLayer.RegisterScreen(screenID, controller);
                if (screenTransform != null)
                    m_panelLayer.ReparentScreen(controller, screenTransform);
            }
        }

        /// <summary>
        /// Registers the panel. Panels can only be shown after they're registered
        /// </summary>
        /// <param name="screenId">Screen Identifier</param>
        /// <param name="controller">Controller</param>
        public void RegisterPanel<TPanel>(string screenId, TPanel controller) where TPanel : IUIScreenController => m_panelLayer.RegisterScreen(screenId, controller);

        /// <summary>
        /// Unregisters the panel
        /// </summary>
        /// <param name="screenId">Screen Identifier</param>
        /// <param name="controller">Controller</param>
        public void UnregisterPanel<TPanel>(string screenId, TPanel controller) where TPanel : IUIScreenController => m_panelLayer.UnregisterScreen(screenId, controller);

        /// <summary>
        /// Registers the Window. Windows can only be opened after they're registered
        /// </summary>
        /// <param name="screenId">Screen Identifier</param>
        /// <param name="controller">Controller</param>
        public void RegisterWindow<TWindow>(string screenId, TWindow controller) where TWindow : IWindowController => m_windowLayer.RegisterScreen(screenId, controller);

        /// <summary>
        /// Unregisters the Window
        /// </summary>
        /// <param name="screenId">Screen Identifier</param>
        /// <param name="controller">Controller</param>
        /// <typeparam name="TWindow"></typeparam>
        public void UnregisterWindow<TWindow>(string screenId, TWindow controller) where TWindow : IWindowController => m_windowLayer.UnregisterScreen(screenId, controller);

        /// <summary>
        /// Checks if a given Panel is open
        /// </summary>
        /// <param name="panelId">Panel identifier</param>
        /// <returns></returns>
        public bool IsPanelOpen(string panelId) => m_panelLayer.IsPanelVisible(panelId);

        /// <summary>
        /// Hides all screen
        /// </summary>
        /// <param name="animate">Defines if the screens should animate out or not</param>
        public void HideAll(bool animate = true)
        {
            CloseAllWindows(animate);
            HideAllPanels(animate);
        }

        /// <summary>
        /// Hides all the screens on the Panel Layer
        /// </summary>
        /// <param name="animate"></param>
        public void HideAllPanels(bool animate = true) => m_panelLayer.HideAll(animate);

        /// <summary>
        /// Hides all the screens on the Windows layer
        /// </summary>
        /// <param name="animate"></param>
        public void CloseAllWindows(bool animate = true) => m_windowLayer.HideAll(animate);

        /// <summary>
        /// Checks if a given screen id is registered to either the Window or Panel layers
        /// </summary>
        /// <param name="screenId">The Id to check</param>
        public bool IsScreenRegistered(string screenId)
        {
            if (m_windowLayer.IsScreenRegistered(screenId))
                return true;
            if (m_panelLayer.IsScreenRegistered(screenId))
                return true;
            return false;
        }

        /// <summary>
        /// Checks if a given screen is registered to either the window or panel layers
        /// also returning the the screen type
        /// </summary>
        /// <param name="screenId">The Id to Check</param>
        /// <param name="type">the type of the screen</param>
        public bool IsScreenRegistered(string screenId, out Type type)
        {
            if (m_windowLayer.IsScreenRegistered(screenId))
            {
                type = typeof(IWindowController);
                return true;
            }

            if (m_panelLayer.IsScreenRegistered(screenId))
            {
                type = typeof(IUIScreenController);
                return true;
            }

            type = null;
            return false;
        }

        private void OnRequestScreenBlock()
        {
            if (m_graphicRaycaster != null)
            {
                m_graphicRaycaster.enabled = false;
            }
        }

        private void OnRequestScreenUnBlock()
        {
            if (m_graphicRaycaster != null)
            {
                m_graphicRaycaster.enabled = true;
            }
        }
    }
}