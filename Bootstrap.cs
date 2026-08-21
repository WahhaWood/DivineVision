namespace DivineVision;

using Divine.Menu;
using Divine.Service;
using Divine.Update;
using DivineVision.Core;

internal sealed class Bootstrap : Bootstrapper
{
    private PluginManager? _manager;

    protected override void OnMainActivate()
    {
        var mainMenu = MenuManager.AddMenu("Divine Vision");
        _manager = new PluginManager(mainMenu);
    }

    protected override void OnMainDeactivate()
    {
        _manager?.Dispose();
        _manager = null;
    }

    protected override void OnActivate()
    {
        UpdateManager.CreateIngameUpdate(0, _manager.OnUpdate);
    }

    protected override void OnDeactivate()
    {
        UpdateManager.DestroyIngameUpdate(_manager.OnUpdate);
    }
}
