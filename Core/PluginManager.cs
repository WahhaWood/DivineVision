using System;
using System.Collections.Generic;
using Divine.Game;
using Divine.Menu;
using Divine.Menu.Items;
using DivineVision.Modules;

namespace DivineVision.Core;

public class PluginManager : IDisposable
{
    private readonly Menu _mainMenu;
    private readonly MenuSwitcher _enabledSwitcher;
    private readonly List<IModule> _modules = new();

	public PluginManager(Menu mainMenu)
	{
	    _mainMenu = mainMenu;
	    _enabledSwitcher = _mainMenu.AddSwitcher("Enabled", true);
	
    	// Создаём модули
	    var lastHit = new LastHitModule(_mainMenu);
    	var visual = new VisualModule(_mainMenu);
    	// Передаём ссылку на ластхит модуль в визуал
    	visual.LastHitModuleRef = lastHit;

    	_modules.Add(lastHit);
    	_modules.Add(visual);
	}

    public void OnUpdate()
    {
        if (!_enabledSwitcher || GameManager.GameState != GameState.InGame)
            return;

        foreach (var module in _modules)
        {
            if (module.Enabled)
                module.OnUpdate();
        }
    }

    public void OnDraw(EventArgs args)
    {
        if (!_enabledSwitcher || GameManager.GameState != GameState.InGame)
            return;

        foreach (var module in _modules)
        {
            if (module.Enabled)
                module.OnDraw();
        }
    }

    public void Dispose()
    {
        foreach (var module in _modules)
        {
            module.Dispose();
        }
        _modules.Clear();
    }
}