using System;
using System.Collections.Generic;
using DependencyInjection;
using DependencyInjection.attributes;
using Misc.Singelton;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Pool;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Input
{
    public class TouchInputHandler : MonoBehaviorSingleton<TouchInputHandler>, IDependencyProvider
    {
        private TouchInputReader m_touchInputReader;
        private UnityEngine.Camera m_gameCamera
        {
            get => UnityEngine.Camera.main;
        }
        
        //private GameObject m_selectedObject;

        protected override void Awake()
        {
            base.Awake();
            m_touchInputReader = new TouchInputReader();
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        private void Update()
        {
            Process();
            //Debug.Log($"selected object: {m_selectedObject}");
        }

        private void LateUpdate()
        {
            //Set the previous frame position of the primary finger and others
            if(!EnsurePreconditions())
                return;
            if (m_touchInputReader.PrimaryFingerPhase == TouchPhase.Moved)
            {
                m_touchInputReader.SetPrimaryFingerPreviousFramePosition(m_touchInputReader.PrimaryFingerPosition);
            }
            if (Touch.activeTouches.Count == 2 && m_touchInputReader.SecondaryFingerPhase == TouchPhase.Moved)
            {
                m_touchInputReader.SetSecondaryFingerPreviousFramePosition(m_touchInputReader.SecondaryFingerPosition);
            }
        }

        private bool EnsurePreconditions()
        {
            if (m_gameCamera == null)
            {
                return false;
            }
            
            if(Touch.activeTouches.Count <= 0)
                return false;

            return true;
        }

        private void Process()
        {
            //Make sure we have a camera to work with and we have at least one touch
            if(!EnsurePreconditions())
                return;
            Touch primaryTouch = Touch.activeTouches[0];

            ProcessPrimaryFinger(primaryTouch);
            if(Touch.activeTouches.Count == 2)
            {
                Touch secondaryTouch = Touch.activeTouches[1];
                ProcessSecondaryFinger(secondaryTouch);
            }
            
            //Invoke the touch update event
            m_touchInputReader.OnTouchUpdate?.Invoke();
        }

        private void ProcessPrimaryFinger(Touch primaryTouch)
        {
                m_touchInputReader.SetPrimaryFingerPhase(primaryTouch.phase);
                if (primaryTouch.phase == TouchPhase.Began)
                {
                    //Check if we are touching an interactable object
                    ProcessPrimaryFingerBegin(primaryTouch);
                }
                else if (primaryTouch.phase == TouchPhase.Moved)
                {
                    if (!m_touchInputReader.IsTouchingInteractable)
                    {
                        m_touchInputReader.SetPrimaryFingerPosition(primaryTouch.screenPosition);
                    }
                }
                else if (primaryTouch.phase == TouchPhase.Ended || primaryTouch.phase == TouchPhase.Canceled)
                {
                    m_touchInputReader.SetIsTouchingInteractable(false);
                    //m_selectedObject = null;
                }
        }
        private void ProcessPrimaryFingerBegin(Touch primaryFinger)
        {
            //Check if we under our first touch is an interactable object
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Is touching UI element");
                m_touchInputReader.SetIsTouchingInteractable(true);
                return;
            }
            m_touchInputReader.SetPrimaryFingerPreviousFramePosition(primaryFinger.screenPosition);
            
            // Vector2 touchWorldPosition = m_gameCamera.ScreenToWorldPoint(primaryFinger.screenPosition);
            // RaycastHit2D hit = Physics2D.Raycast(touchWorldPosition, Vector2.zero);
            // if (hit.collider != null)
            // {
            //     if (BubbleUpGameObjectsImplementsEventHandler(hit.collider.gameObject))
            //     {
            //         m_selectedObject = hit.collider.gameObject;
            //         m_touchInputReader.SetIsTouchingInteractable(true);
            //     }
            // }
        }
        
        private void ProcessSecondaryFinger(Touch secondaryTouch)
        {
            if (!EventSystem.current.IsPointerOverGameObject(secondaryTouch.touchId))
            {
                m_touchInputReader.SetSecondaryFingerPhase(secondaryTouch.phase);
                if (secondaryTouch.phase == TouchPhase.Began)
                {
                    m_touchInputReader.SetSecondaryFingerPreviousFramePosition(secondaryTouch.screenPosition);
                }
                else if (secondaryTouch.phase == TouchPhase.Moved)
                {
                    m_touchInputReader.SetSecondaryFingerPosition(secondaryTouch.screenPosition);
                }
            }
        }


        private bool BubbleUpGameObjectsImplementsEventHandler(GameObject gameObject)
        {
            GameObject currentProcessingObj = gameObject;
            bool isParentNull = false;
            while (isParentNull == false)
            {
                if(DoesObjectImplementEventHandler(currentProcessingObj))
                    return true;

                if(currentProcessingObj.transform.parent == null)
                    isParentNull = true;
                else
                    currentProcessingObj = currentProcessingObj.transform.parent.gameObject;
            }
            return false;
        }
        
        private bool DoesObjectImplementEventHandler(GameObject gameObject)
        {
            List<Component> components = ListPool<Component>.Get();
            gameObject.GetComponents(components);

            int componentCount = components.Count;
            for (int i = 0; i < componentCount; i++)
            {
                if (components[i] is IEventSystemHandler)
                {
                    ListPool<Component>.Release(components);
                    return true;
                }
            }
            ListPool<Component>.Release(components);
            return false;
        }

        #region DP
        [Provide]
        private TouchInputReader ProvideTouchInputReader()
        {
            return m_touchInputReader;
        }
        #endregion

    }
}