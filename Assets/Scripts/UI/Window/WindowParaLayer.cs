using System.Collections.Generic;
using UnityEngine;

namespace UI.Window
{
    /// <summary>
    /// This is a "helper" layer so Windows with higher priority can be displayed.
    /// By default, it contains any window tagged as a Popup. It is controlled by the WindowUILayer.
    /// </summary>
    public class WindowParaLayer : MonoBehaviour
    {
        [SerializeField] private GameObject m_darkenBackgroundObject = null;

        private List<GameObject> m_containedScreens = new List<GameObject>();

        public void AddScreen(Transform screenRectTransform)
        {
            screenRectTransform.SetParent(transform, false);
            m_containedScreens.Add(screenRectTransform.gameObject);
        }

        public void RefreshDarkenBackground()
        {
            for (int i = 0; i < m_containedScreens.Count; i++)
            {
                if (m_containedScreens[i] != null)
                {
                    if (m_containedScreens[i].activeSelf)
                    {
                        m_darkenBackgroundObject.SetActive(true);
                        return;
                    }
                }
            }
            m_darkenBackgroundObject.SetActive(false);
        }

        public void DarkenBackground()
        {
            m_darkenBackgroundObject.SetActive(true);
            m_darkenBackgroundObject.transform.SetAsLastSibling();
        }
    }
}