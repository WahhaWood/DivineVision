using Divine;
using Divine.Menu;
using Divine.Service;
using Divine.Update;

namespace DivineVision;

internal sealed class Bootstrap : Bootstrapper
{
    private PluginManager _manager = null!;

    protected override void OnMainActivate()
    {
        Console.WriteLine("✅ Divine Vision загружен!");

        var mainMenu = MenuManager.AddMenu("Divine Vision");
        _manager = new PluginManager(mainMenu);
    }

    protected override void OnMainDeactivate()
    {
        Console.WriteLine("❌ Divine Vision выгружен");
        _manager?.Dispose();
    }

    protected override void OnActivate()
    {
        // Подписываемся на игровые обновления (тик ~20 мс)
        UpdateManager.CreateIngameUpdate(0, _manager.OnUpdate);
        // Подписываемся на рендеринг (отрисовка каждый кадр)
        RendererManager.Draw += _manager.OnDraw;
    }

    protected override void OnDeactivate()
    {
        UpdateManager.DestroyIngameUpdate(_manager.OnUpdate);
        RendererManager.Draw -= _manager.OnDraw;
    }
}
