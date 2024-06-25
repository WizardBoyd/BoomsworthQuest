namespace UI.Window
{
    /// <summary>
    /// Base implementation for window screenControllers that need no special Properties
    /// </summary>
    public abstract class AWindowController : AWindowController<WindowProperties>{}

    public abstract class AWindowController<TProps> : AUIScreenController<TProps>, IWindowController
        where TProps : IScreenProperties
    {
        public bool HideOnForegroundLost => Properties.HideOnForegroundLost;
        public bool IsPopUp => Properties.IsPopup;

        public override ScreenPriority ScreenPriority => Properties.Priority;

        /// <summary>
        /// Requests this Window to be closed, handy for rigging it directly in the Editor.
        /// I use the UI_ prefix to group all the methods that should be rigged in the Editor so that it's
        /// easy to find the screen-specific methods. It breaks naming convention, but does more good than harm as
        /// the amount of methods grow.
        /// This is *not* called every time it is closed, just upon user input - for that behaviour, see
        /// WhileHiding();
        /// </summary>
        public virtual void UI_Close() => CloseRequest(this);

        protected sealed override void SetProperties(TProps props)
        {
            if (props != null)
            {
                // If the Properties set on the prefab should not be overwritten,
                // copy the default values to the passed in properties
                if (!props.SuppressPrefabProperties)
                {
                    props.HideOnForegroundLost = Properties.HideOnForegroundLost;
                    props.Priority = Properties.Priority;
                    props.IsPopup = Properties.IsPopup;
                }

                Properties = props;
            }
        }
        
        protected override void HierarchyFixOnShow() => transform.SetAsLastSibling();
    }
}