using Nez;

namespace PlatformerGame.Components
{
    /// <summary>
    /// SettingsMenu class that provides a simple way to create a settings menu
    /// </summary>
    public class SettingsMenu : BasicUI
    {
        /// <summary>
        /// Creates a new SettingsMenu object with a UICanvas
        /// </summary>
        /// <param name="uiCanvas"></param>
        public SettingsMenu(UICanvas uiCanvas) : base(uiCanvas, "Settings", 3.0f)
        {
            // Full screen
            var fullscreenCheckbox = AddCheckbox("Fullscreen", Screen.IsFullscreen);
            fullscreenCheckbox.OnChanged += (checkbox) => { Screen.IsFullscreen = fullscreenCheckbox.IsChecked; Screen.ApplyChanges(); };

            // Back
            AddButton("Back", (button) => { Hide(); });
        }
    }
}
