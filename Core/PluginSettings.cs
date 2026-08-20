namespace DivineVision.Core;

using Divine.Menu;
using Divine.Menu.Items;

public class PluginSettings
{
    public MenuSwitcher Enabled { get; private set; }
    public MenuSwitcher DebugMode { get; private set; }

    public PluginSettings(Menu menu)
    {
        Enabled = menu.AddSwitcher("Enabled", true);
        DebugMode = menu.AddSwitcher("Debug Mode", false);
    }
}
