using System.Collections;
using Events.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class EventEmitterButton : Button
    {
        [SerializeField]
        private VoidEventChannelSO m_OnClickEvent = default;

        private void Press()
        {
            if(!IsActive() || !IsInteractable())
                return;
            UISystemProfilerApi.AddMarker("Button.onClick", this);
            if (m_OnClickEvent != null)
            {
                m_OnClickEvent.RaiseEvent();
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning($"Tried to click button but with no registered event channel");
            }
#endif
            onClick?.Invoke();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            Press();
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }
        
        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}