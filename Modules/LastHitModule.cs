namespace DivineVision.Modules;

using System;
using System.Linq;
using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Extensions;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.Orbwalker;
using DivineVision.Helpers;

public class LastHitModule : IModule
{
    private readonly MenuSwitcher _enabled;
    private readonly MenuSlider _range;
    private readonly MenuSlider _reactionDelay;
    private readonly VisualModule _visual;

    private Hero? _localHero;
    private Creep? _bestLastHit;
    private Creep? _bestDeny;

    public bool Enabled { get; set; }

    public LastHitModule(Menu mainMenu, VisualModule visual)
    {
        _visual = visual;

        var menu = mainMenu.AddMenu("Last Hit");
        _enabled = menu.AddSwitcher("Enabled", true);
        _range = menu.AddSlider("Range", 800, 400, 1200);
        _reactionDelay = menu.AddSlider("Delay (ms)", 100, 0, 300);

        Enabled = true;
    }

    public void OnUpdate()
    {
        if (!_enabled) return;

        _localHero = EntityManager.LocalHero;
        if (_localHero is null || !_localHero.IsAlive || !_localHero.CanAttack)
            return;

        // Находим цели
        _bestLastHit = FindBestCreep(EntityHelper.GetEnemyCreepsInRange(_localHero, _range.Value));
        _bestDeny = FindBestCreep(EntityHelper.GetAllyCreepsInRange(_localHero, _range.Value));

        // Передаём цели визуальному модулю
        _visual.UpdateTargets(_bestLastHit, _bestDeny);

        // Выполняем действие
        if (_bestLastHit is not null)
        {
            OrbwalkerManager.Attack(_bestLastHit);
        }
        else if (_bestDeny is not null)
        {
            OrbwalkerManager.Attack(_bestDeny);
        }
    }

    private Creep? FindBestCreep(System.Collections.Generic.IEnumerable<Creep> creeps)
    {
        if (_localHero is null) return null;

        return creeps
            .OrderBy(c => c.Health)
            .FirstOrDefault(c => EntityHelper.CanLastHit(c, _localHero));
    }

    public void Dispose() { }
}
