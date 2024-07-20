using System;
using DG.Tweening;
using Events.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using WizardOptimizations.Runtime.Singelton;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class FadeController : MonoBehaviorSingleton<FadeController>
    {
        [SerializeField] private FadeEventChannelSO m_fadeChannel;
        private Image m_imageComponent;

        protected override void Awake()
        {
            base.Awake();
            m_imageComponent = GetComponent<Image>();
        }

        private void OnEnable()
        {
            m_fadeChannel.OnEventRaised += InitiateFade;
        }

        private void OnDisable()
        {
            m_fadeChannel.OnEventRaised -= InitiateFade;
        }

        private void InitiateFade(bool fadeIn, float duration, Color desiredColor)
        {
            m_imageComponent.DOBlendableColor(desiredColor, duration);
        }
    }
}