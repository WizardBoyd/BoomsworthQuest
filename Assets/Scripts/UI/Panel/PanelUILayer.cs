using UnityEngine;

namespace UI.Panel
{
    public class PanelUILayer : AUILayer<IUIScreenController>
    {
        [Tooltip("Settings for the priority para-layers. " +
                 "A panel registered to this layer will be re-parented to a different para-layer object" +
                 " depending on its priority")]
        [SerializeField]
        private ScreenPriorityLayerList m_priortyLayers = null;

        public override void ReparentScreen(IUIScreenController controller, Transform screenTransform)
        {
            if (controller != null)
                ReparentToParaLayer(controller.ScreenPriority, screenTransform);
            else
                base.ReparentScreen(null,screenTransform);
        }

        public override void ShowScreen(IUIScreenController screen) => screen.Show();

        public override void ShowScreen<TProps>(IUIScreenController screen, TProps properties) => screen.Show(properties);

        public override void HideScreen(IUIScreenController screen) => screen.Hide();

        public bool IsPanelVisible(string panelId)
        {
            IUIScreenController panel;
            if (m_registeredScreens.TryGetValue(panelId, out panel))
                return panel.IsVisible;
            return false;
        }
        
        private void ReparentToParaLayer(ScreenPriority controllerScreenPriority, Transform screenTransform)
        {
            Transform trans;
            if (!m_priortyLayers.paraLayerLookup.TryGetValue(controllerScreenPriority, out trans))
                trans = transform;
            screenTransform.SetParent(trans, false);
        }
    }
}