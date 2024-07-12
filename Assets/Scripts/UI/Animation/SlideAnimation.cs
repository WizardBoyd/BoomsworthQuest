using System;
using DG.Tweening;
using UnityEngine;
using WizardUI.ScreenTransitions;

namespace UI.Animation
{
    [RequireComponent(typeof(RectTransform))]
    public class SlideAnimation : ATransitionComponent
    {
        [SerializeField]
        private Vector2 m_StartPosition;
        [SerializeField]
        private Vector2 m_EndPosition;
        
        [SerializeField]
        private Ease m_Ease = Ease.Linear;
        
        private RectTransform m_RectTransform;

        private void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
        }

        public override void Animate(Transform target, Action finishedCallback)
        {
            m_RectTransform.transform.localPosition = new Vector3(m_StartPosition.x, m_StartPosition.y, 0);
            var animation = m_RectTransform.DOAnchorPos(m_EndPosition, 0.5f, false).SetEase(m_Ease);
            animation.onComplete += () => finishedCallback?.Invoke();
        }
    }
}