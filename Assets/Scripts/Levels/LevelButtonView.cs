using System;
using BaseClasses;
using Events.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Levels
{
    public class LevelButtonView : BaseView<LevelButtonModel, LevelButtonController>
    {
        [Header("Stars")] 
        [SerializeField] private RectTransform LeftStar;
        [SerializeField] private RectTransform MiddleStar;
        [SerializeField] private RectTransform RightStar;

        [Header("Misc")] 
        [SerializeField] private RectTransform Lock;
        [SerializeField] private TMP_Text numberText;

        private Button button;
        

        [Header("Broadcasting On Event")] 
        [SerializeField] private LevelLoadEventChannel On_LevelClicked;

        protected override void Awake()
        {
            base.Awake();
            button = GetComponentInChildren<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(On_ButtonClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(On_ButtonClicked);
        }
        
        private void UpdateButtonView()
        {
            UpdateStarView();
            if (Controller.IsLevelLocked())
            {
                Lock.gameObject.SetActive(true);
                //deactivate the button intractable state
                button.interactable = false;
            }
            else
            {
                Lock.gameObject.SetActive(false);
                //Activate the button intractable state
                button.interactable = true;
            }
        }
        
        private void UpdateStarView()
        {
            if (Model.LevelState == LevelState.Completed)
            {
                //Reset the active state
                LeftStar.gameObject.SetActive(false);
                MiddleStar.gameObject.SetActive(false);
                RightStar.gameObject.SetActive(false);
                
                if(Model.StarCount <= 0)
                    Debug.LogWarning("Star Count is lower or equal to 0 even after completing level");

                if (Model.StarCount == 1)
                {
                    LeftStar.gameObject.SetActive(true);
                }

                if (Model.StarCount == 2)
                {
                    LeftStar.gameObject.SetActive(true);
                    MiddleStar.gameObject.SetActive(true);
                }
                
                if (Model.StarCount >= 3)
                {
                    LeftStar.gameObject.SetActive(true);
                    MiddleStar.gameObject.SetActive(true);
                    RightStar.gameObject.SetActive(true);
                }
            }
            else
            {
                //The level has not been completed so don't show any star details at all
                LeftStar.gameObject.SetActive(false);
                MiddleStar.gameObject.SetActive(false);
                RightStar.gameObject.SetActive(false);
            }
        }

        private void On_ButtonClicked()
        {
            Controller.LoadLevel(On_LevelClicked);
        }
    }
}