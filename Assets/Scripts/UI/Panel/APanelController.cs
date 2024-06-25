namespace UI.Panel
{
    /// <summary>
    /// Base class for panels that need no special properties
    /// </summary>
    public abstract class APanelController : APanelController<PanelProperties>{}
    
    public abstract class APanelController<T> : AUIScreenController<T> where T : IScreenProperties
    {
        public override ScreenPriority ScreenPriority
        {
            get
            {
                if (Properties != null)
                    return ScreenPriority.Prioritary;
                else
                    return ScreenPriority.None;
            }
        }
    }
}