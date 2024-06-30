using System;
using Misc.Singelton;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Tests.Gameplay
{

    public class GameplayManager : MonoBehaviorSingleton<GameplayManager>
    {
        private InputReader m_inputReader;
        
        protected override void Awake()
        {
            base.Awake();
            EnhancedTouchSupport.Enable();
            m_inputReader = new InputReader();
        }

        private void OnDestroy()
        {
            EnhancedTouchSupport.Disable();
        }

        private void Update()
        {
            //m_inputReader.UpdateValues();
        }
    }
}