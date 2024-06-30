using System;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Tests.Gameplay
{
    public class InputReader
    {

        public GameInput m_playerInput;

        public InputReader()
        {
            m_playerInput = new GameInput();
            m_playerInput.Enable();
            m_playerInput.GenericActions.TapAction.started += TapActionOnstarted;
            m_playerInput.GenericActions.TapAction.performed += TapActionperformed;
            m_playerInput.GenericActions.TapAction.started += TapActionCanceled;
        }

        private void TapActionCanceled(InputAction.CallbackContext obj)
        {
            Debug.Log($"Tap Action Canceled {obj.ReadValue<Vector2>()}");
        }

        private void TapActionperformed(InputAction.CallbackContext obj)
        {
            Debug.Log($"Tap Action Performed {obj.ReadValue<Vector2>()}");
        }

        private void TapActionOnstarted(InputAction.CallbackContext obj)
        {
            Debug.Log($"Tap Action Started {obj.ReadValue<Vector2>()}");
        }

        ~InputReader()
        {
            m_playerInput.Dispose();
        }
        
        public void UpdateValues()
        {
            if(!EnhancedTouchSupport.enabled)
                return;
            foreach (Finger activeFinger in Touch.activeFingers)
            {
                Debug.Log($"These are the active finger {activeFinger.index}");
            }
        }

        private void ProcessActiveTouch(Touch touch)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    break;
                case TouchPhase.Moved:
                    break;
                case TouchPhase.Ended:
                    break;
                case TouchPhase.Canceled:
                    break;
                case TouchPhase.Stationary:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ProcessBeginPhaseTouch(Touch touch)
        {
            
        }
    }
}