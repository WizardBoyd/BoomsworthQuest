namespace UI.Window
{
    public class WindowHistoryEntry
    {
        public readonly IWindowController Screen;
        public readonly IScreenProperties Properties;

        public WindowHistoryEntry(IWindowController screen, IScreenProperties properties)
        {
            this.Screen = screen;
            this.Properties = properties;
        }

        public void Show() => Screen.Show(Properties);
    }
}