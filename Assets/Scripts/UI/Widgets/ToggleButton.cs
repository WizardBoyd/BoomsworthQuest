using System;
using Events.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Widgets
{
    [RequireComponent(typeof(Button))]
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private GameObject m_SwitchOnState;
        [SerializeField] private GameObject m_SwitchOffState;
        
        public Button Button { get; private set; }

        private void Awake()
        {
            Button = GetComponent<Button>();
        }

        public void OnToggle(bool toggleState)
        {
            if (toggleState)
            {
                m_SwitchOnState.SetActive(true);
                m_SwitchOffState.SetActive(false);
            }
            else
            {
                m_SwitchOnState.SetActive(false);
                m_SwitchOffState.SetActive(true);
            }
        }
    }
}