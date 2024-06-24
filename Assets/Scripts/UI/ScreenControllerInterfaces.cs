using System;

namespace UI
{
    /// <summary>
    /// Interface that all UI screens must implement directly or indirectly,
    /// all panels must implement this interface
    /// </summary>
    public interface IUIScreenController
    {
        string ScreenId { get; set; }
        bool IsVisible { get; }
        ScreenPriority ScreenPriority { get; }

        void Show(IScreenProperties properties = null);
        void Hide(bool animate = true);
        
        Action<IUIScreenController> InTransitionFinished { get; set; }
        Action<IUIScreenController> OutTransitionFinished { get; set; }
        Action<IUIScreenController> CloseRequest { get; set; }
        Action<IUIScreenController> ScreenDestroyed { get; set; }
    }

    /// <summary>
    /// Interface that all windows must implement
    /// </summary>
    public interface IWindowController : IUIScreenController
    {
        bool HideOnForegroundLost { get; }
        bool IsPopUp { get; }
    }
}