using Interaction;
using UnityEngine;
using UnityEngine.Localization;

namespace UI.ScriptableObject
{
    [CreateAssetMenu(fileName = "InteractionSO", menuName = "UI/Interaction")]
    public class InteractionSO : UnityEngine.ScriptableObject
    {
        [SerializeField] private LocalizedString _interactionName = default;
        [SerializeField] private Sprite _interactionIcon = default;
        [SerializeField] private InteractionType _interactionType = default;

        public Sprite InteractionIcon => _interactionIcon;
        public LocalizedString InteractionName => _interactionName;
        public InteractionType InteractionType => _interactionType;
    }
}