namespace DivineVision.Modules;

using System;
using System.Linq;
using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Extensions;
using Divine.Input;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.Orbwalker;
using DivineVision.Helpers;

public class LastHitModule : IModule
{
    private readonly MenuHoldKey _lastHitKey;
    private readonly MenuSlider _range;
    private readonly MenuSlider _reactionDelay;
    private readonly VisualModule _visual;

    private Hero? _localHero;
    private Creep? _bestLastHit;
    private Creep? _bestDeny;

    public bool IsActive => _lastHitKey.Value;

    public LastHitModule(Menu mainMenu, VisualModule visual)
    {
        _visual = visual;

        var menu = mainMenu.AddMenu("Last Hit");
        _lastHitKey = menu.AddHoldKey("Activation Key", Key.Space);
        _range = menu.AddSlider("Range", 800, 400, 1200);
        _reactionDelay = menu.AddSlider("Delay (ms)", 100, 0, 300);
    }

    public void OnUpdate()
    {
        // Сообщаем визуалке, активен ли ластхит
        _visual.SetLastHitActive(_lastHitKey.Value);

        if (!_lastHitKey.Value)
            return;

        _localHero = EntityManager.LocalHero;
        if (_localHero is null || !_localHero.IsAlive || !_localHero.CanAttack)
            return;

        _bestLastHit = FindBestCreep(EntityHelper.GetEnemyCreepsInRange(_localHero, _range.Value));
        _bestDeny = FindBestCreep(EntityHelper.GetAllyCreepsInRange(_localHero, _range.Value));

        _visual.UpdateTargets(_bestLastHit, _bestDeny);

        if (_bestLastHit is not null)
            OrbwalkerManager.Attack(_bestLastHit);
        else if (_bestDeny is not null)
            OrbwalkerManager.Attack(_bestDeny);
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
