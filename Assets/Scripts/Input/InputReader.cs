using System;
using BaseClasses;
using Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
    public class InputReader : DescriptionBaseSO, GameInput.IGenericActionsActions
    {
        [Space]
        [SerializeField] private GameStateSO _gameStateManager;
        
        // Assign delegate{} to events to initialise them with an empty delegate
        // so we can skip the null check when we use them
        
        public event UnityAction TouchPress = delegate { };

        private GameInput _gameInput;

        private void OnEnable()
        {
            if (_gameInput == null)
            {
                _gameInput = new GameInput();
                
                _gameInput.GenericActions.SetCallbacks(this);
            }
        }

        private void OnDisable()
        {
            DisableAllInput();
        }
        
        public void DisableAllInput()
        {
            _gameInput.GenericActions.Disable();
            // _gameInput.Gameplay.Disable();
            // _gameInput.Menus.Disable();
            // _gameInput.LevelSelect.Disable();
        }

        // public void EnableGameplayInput()
        // {
        //     _gameInput.Menus.Disable();
        //     _gameInput.LevelSelect.Disable();
        //     _gameInput.Gameplay.Enable();
        // }
        //
        // public void EnableLevelSelectInput()
        // {
        //     _gameInput.Menus.Disable();
        //     _gameInput.LevelSelect.Enable();
        //     _gameInput.Gameplay.Disable();
        // }
        //
        // public void EnableMenuInput()
        // {
        //     _gameInput.Menus.Enable();
        //     _gameInput.LevelSelect.Disable();
        //     _gameInput.Gameplay.Disable();
        // }


        public void OnTouchButton(InputAction.CallbackContext context)
        {
            Debug.Log("Button Touch Pressed");
        }
    }
}