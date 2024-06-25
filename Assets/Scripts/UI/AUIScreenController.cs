using System;
using UI.ScreenTransitions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace UI
{
    public abstract class AUIScreenController<TProps> : MonoBehaviour, IUIScreenController
    where TProps : IScreenProperties
    {
        [Header("Screen Animations")]
        [SerializeField]
        private ATransitionComponent m_animIn;

        [SerializeField] 
        private ATransitionComponent m_animOut;

        [Header("Screen Properties")]
        [SerializeField]
        private TProps m_properties;

        /// <summary>
        /// Unique identifier for this ID, if using the default system, it should be the same name as the screen's prefab
        /// </summary>
        public string ScreenId { get; set; }

        /// <summary>
        /// Transition component for the showing up animation
        /// </summary>
        public ATransitionComponent AnimIn
        {
            get => m_animIn;
            set => m_animIn = value;
        }
        
        /// <summary>
        /// Transition component for the hiding animation
        /// </summary>
        public ATransitionComponent AnimOut
        {
            get => m_animOut;
            set => m_animOut = value;
        }
        
        /// <summary>
        /// Occurs when "in" transition is finished
        /// </summary>
        public Action<IUIScreenController> InTransitionFinished { get; set; }
        /// <summary>
        /// Occurs when "out" transition is finished
        /// </summary>
        public Action<IUIScreenController> OutTransitionFinished { get; set; }
        /// <summary>
        /// Screen can fire this event to request its responsible layer to close it
        /// </summary>
        public Action<IUIScreenController> CloseRequest { get; set; }
        /// <summary>
        /// if this screen is destroyed for some reason, it must warn its layer
        /// </summary>
        public Action<IUIScreenController> ScreenDestroyed { get; set; }
        
        /// <summary>
        /// Is the screen currently visible?
        /// </summary>
        public bool IsVisible { get; private set; }

        /// <summary>
        /// The properties of this screen. Can
        /// contain serialized values, or passed in private values
        /// </summary>
        protected TProps Properties
        {
            get => m_properties;
            set => m_properties = value;
        }
        
        public abstract ScreenPriority ScreenPriority { get; }

        protected virtual void Awake()
        {
            AddListeners();
        }

        protected virtual void OnDestroy()
        {
            if (ScreenDestroyed != null)
                ScreenDestroyed(this);

            InTransitionFinished = null;
            OutTransitionFinished = null;
            CloseRequest = null;
            ScreenDestroyed = null;
            RemoveListeners();
        }

        /// <summary>
        /// For setting up all the listeners for events/message, by Default called on Awake()
        /// </summary>
        protected abstract void AddListeners();

        /// <summary>
        /// For removing all the listeners for events/messages. by default, called onDestroy() 
        /// </summary>
        protected abstract void RemoveListeners();

        /// <summary>
        /// When Properties are set for this screen, this method is called
        /// At this point, can safely access properties
        /// </summary>
        protected abstract void OnPropertiesSet();

        /// <summary>
        /// When the screen animates out, this is called
        /// immediately
        /// </summary>
        protected abstract void WhileHiding();

        /// <summary>
        /// When setting the properties, this method is called,
        /// this way, can extend the usage of the properties by certain
        /// conditions
        /// </summary>
        /// <param name="props">Properties</param>
        protected virtual void SetProperties(TProps props)
        {
            Properties = props;
        }

        /// <summary>
        /// In case the screen has any special behavior to be called
        /// when the hierarchy is adjusted
        /// </summary>
        protected abstract void HierarchyFixOnShow();
        
        /// <summary>
        /// Show this screen with the specified properties
        /// </summary>
        /// <param name="properties">The data for the screen</param>
        public void Show(IScreenProperties properties = null)
        {
            if (properties != null)
            {
                if (properties is TProps)
                {
                    SetProperties((TProps)properties);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError(
                        $"Properties passed have wrong type ({properties.GetType().Name} instead of {typeof(TProps).Name})");
#endif
                    return;
                }
                
                HierarchyFixOnShow();
                OnPropertiesSet();

                if (!gameObject.activeSelf)
                    DoAnimation(m_animIn, OnTransitionInFinished , true);
                else if (InTransitionFinished != null)
                    InTransitionFinished(this);
            }
        }

        /// <summary>
        /// Hides the screen
        /// </summary>
        /// <param name="animate">should the animation be played?</param>
        public void Hide(bool animate = true)
        {
            throw new NotImplementedException();
        }

        private void DoAnimation(ATransitionComponent caller, Action callback, bool isVisible)
        {
            if (caller == null)
            {
                gameObject.Serialize(isVisible);
                callback?.Invoke();
            }
            else
            {
                if(isVisible && !gameObject.activeSelf)
                    gameObject.SetActive(true);
                caller.Animate(transform, callback);
            }
        }

        private void OnTransitionInFinished()
        {
            IsVisible = true;
            InTransitionFinished?.Invoke(this);
        }

        private void OnTransitionOutFinished()
        {
            IsVisible = false;
            gameObject.SetActive(false);
            OutTransitionFinished?.Invoke(this);
        }
    }
}