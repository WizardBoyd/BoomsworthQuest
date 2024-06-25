using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Window
{
    public class WindowUILayer : AUILayer<IWindowController>
    {
        [SerializeField] private WindowParaLayer m_priorityParaLayer = null;
        
        public IWindowController CurrentWindow { get; private set; }

        private Queue<WindowHistoryEntry> m_windowQueue;
        private Stack<WindowHistoryEntry> m_windowHistory;
        private HashSet<IUIScreenController> m_screensTrasitioning;

        public event Action RequestScreenBlock;
        public event Action RequestScreenUnBlock;

        public bool IsScreenTransitionInProgress
        {
            get => m_screensTrasitioning.Count != 0;
        }

        public override void Initialize()
        {
            base.Initialize();
            m_registeredScreens = new Dictionary<string, IWindowController>();
            m_windowQueue = new Queue<WindowHistoryEntry>();
            m_windowHistory = new Stack<WindowHistoryEntry>();
            m_screensTrasitioning = new HashSet<IUIScreenController>();
        }

        protected override void ProcessScreenRegister(string screenID, IWindowController controller)
        {
            base.ProcessScreenRegister(screenID, controller);
            controller.InTransitionFinished += OnInAnimationFinished;
            controller.OutTransitionFinished += OnOutAnimationFinished;
            controller.CloseRequest += OnCloseRequestedByWindow;
        }

        private void OnCloseRequestedByWindow(IUIScreenController screen)
        {
            if(screen is IWindowController windowController)
                HideScreen(windowController);
        }

        private void OnOutAnimationFinished(IUIScreenController screen)
        {
            RemoveTransition(screen);
            if (screen is IWindowController window)
            {
                if(window.IsPopUp)
                    m_priorityParaLayer.RefreshDarkenBackground();
            }
        }

        private void OnInAnimationFinished(IUIScreenController screen) => RemoveTransition(screen);

        private void RemoveTransition(IUIScreenController screen)
        {
            m_screensTrasitioning.Remove(screen);
            if (!IsScreenTransitionInProgress)
            {
                RequestScreenUnBlock?.Invoke();
            }
        }

        protected override void ProcessScreenUnregister(string screenID, IWindowController controller)
        {
            base.ProcessScreenUnregister(screenID, controller);
            controller.InTransitionFinished -= OnInAnimationFinished;
            controller.OutTransitionFinished -= OnOutAnimationFinished;
            controller.CloseRequest -= OnCloseRequestedByWindow;
        }

        public override void ShowScreen(IWindowController screen) => ShowScreen<IScreenProperties>(screen,null);

        public override void ShowScreen<TProps>(IWindowController screen, TProps properties)
        {
            if (ShouldEnqueue(screen, properties))
                EnqueueWindow(screen, properties);
            else
                DoShow(screen, properties);
        }

        public override void HideScreen(IWindowController screen)
        {
            if (screen == CurrentWindow)
            {
                m_windowHistory.Pop();
                AddTransition(screen);
                screen.Hide();

                CurrentWindow = null;

                if (m_windowQueue.Count > 0)
                    ShowNextInQueue();
                else if (m_windowHistory.Count > 0)
                    ShowPreviousInHistory();
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError(
                    $"Hide requested on WindowId {screen.ScreenId} but that's not currently open one ({(CurrentWindow != null ? CurrentWindow.ScreenId : "Current Is Null")})");
            }
#endif
        }

        private void ShowNextInQueue()
        {
            if (m_windowQueue.Count > 0)
            {
                DoShow(m_windowQueue.Dequeue());
            }
        }

        private void AddTransition(IWindowController screen)
        {
            m_screensTrasitioning.Add(screen);
            RequestScreenBlock?.Invoke();
        }

        public override void HideAll(bool shouldAnimateWhenHiding = true)
        {
            base.HideAll(shouldAnimateWhenHiding);
            CurrentWindow = null;
            m_priorityParaLayer.RefreshDarkenBackground();
            m_windowHistory.Clear();
        }

        public override void ReparentScreen(IUIScreenController controller, Transform screenTransform)
        {
            if (controller is IWindowController window)
            {
                if (window.IsPopUp)
                {
                    m_priorityParaLayer.AddScreen(screenTransform);
                    return;
                }
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning($"Screen {screenTransform.name} is not a window");
            }
#endif
            base.ReparentScreen(controller, screenTransform);
        }

        private void EnqueueWindow<TProp>(IWindowController screen, TProp properties) where TProp : IScreenProperties => m_windowQueue.Enqueue(new WindowHistoryEntry(screen, properties));

        private bool ShouldEnqueue(IWindowController controller, IScreenProperties properties)
        {
            if (CurrentWindow == null && m_windowQueue.Count == 0)
                return false;
            if (properties != null && properties.SuppressPrefabProperties)
                return properties.Priority != ScreenPriority.ForceForeground;
            if (controller.ScreenPriority != ScreenPriority.ForceForeground)
                return true;

            return false;
        }

        private void ShowPreviousInHistory()
        {
            if (m_windowHistory.Count > 0)
            {
                DoShow(m_windowHistory.Pop());
            }
        }

        private void DoShow(IWindowController screen, IScreenProperties properties) => DoShow(new WindowHistoryEntry(screen, properties));

        private void DoShow(WindowHistoryEntry windowEntry)
        {
            if (CurrentWindow == windowEntry.Screen)
            {
#if UNITY_EDITOR
                    Debug.LogWarning(
                        $"The requested WindowId ({CurrentWindow.ScreenId}) is already open! This will add a duplicate " +
                        "to the history and might cause inconsistent behaviour. It is recommended that if you need " +
                        "to open the same screen multiple times (eg: when implementing a warning message pop-up), " +
                        "it closes itself upon the player input that triggers the continuation of the flow.");
#endif
            }
            else if(CurrentWindow != null && CurrentWindow.HideOnForegroundLost && !windowEntry.Screen.IsPopUp)
                CurrentWindow.Hide();
            
            m_windowHistory.Push(windowEntry);
            AddTransition(windowEntry.Screen);
            
            if(windowEntry.Screen.IsPopUp)
                m_priorityParaLayer.DarkenBackground();
            
            windowEntry.Show();

            CurrentWindow = windowEntry.Screen;
        }
    }
}