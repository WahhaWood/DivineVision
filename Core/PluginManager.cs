namespace DivineVision.Core;

using System;
using System.Collections.Generic;
using Divine.Game;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.Renderer;
using DivineVision.Modules;

public class PluginManager : IDisposable
{
    private readonly Menu _mainMenu;
    private readonly PluginSettings _settings;
    private readonly List<IModule> _modules = new();
    private readonly VisualModule _visual;

    public PluginManager(Menu mainMenu)
    {
        _mainMenu = mainMenu;
        _settings = new PluginSettings(_mainMenu);

        _visual = new VisualModule(_mainMenu);
        _modules.Add(new LastHitModule(_mainMenu, _visual, _settings));
        _modules.Add(_visual);

        RendererManager.Draw += OnDraw;
    }

    public void OnUpdate()
    {
        if (!_settings.Enabled || GameManager.GameState != GameState.InGame)
            return;

        foreach (var module in _modules)
        {
            if (module.Enabled)
                module.OnUpdate();
        }
    }

    private void OnDraw(EventArgs args)
    {
        if (!_settings.Enabled || GameManager.GameState != GameState.InGame)
            return;

        if (_visual.Enabled)
            _visual.OnDraw();
    }

    public void Dispose()
    {
        RendererManager.Draw -= OnDraw;
        foreach (var module in _modules)
            module.Dispose();
        _modules.Clear();
    }
}
