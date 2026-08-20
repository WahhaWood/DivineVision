using System;
using System.Linq;
using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Extensions;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.Orbwalker;

namespace DivineVision.Modules;

public class LastHitModule : IModule
{
    private readonly MenuSwitcher _enabled;
    private readonly MenuSlider _range;
    private readonly MenuSlider _reactionDelay;
    private Hero? _localHero;

    // Публичное свойство для доступа к текущей цели (для VisualModule)
    public Creep? CurrentTarget { get; private set; }

    public bool Enabled { get; set; }

    public LastHitModule(Menu mainMenu)
    {
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
        {
            CurrentTarget = null;
            return;
        }

        var bestCreep = FindBestCreep();
        CurrentTarget = bestCreep;

        if (bestCreep is not null)
        {
            OrbwalkerManager.Attack(bestCreep);
        }
    }

    private Creep? FindBestCreep()
    {
        if (_localHero is null) return null;

        var creeps = EntityManager.GetEntities<Creep>()
            .Where(c => c.IsAlive && c.IsEnemy(_localHero) && c.Distance(_localHero) < _range.Value)
            .OrderBy(c => c.Health)
            .ToList();

        foreach (var creep in creeps)
        {
            var damage = _localHero.TotalDamage;
            var health = creep.Health;
            // Упрощённый расчёт с учётом брони
            var armorReduction = creep.Armor / (creep.Armor + 100);
            var realDamage = damage * (1 - armorReduction);

            if (health < realDamage + 10)
                return creep;
        }

        return null;
    }

    public void OnDraw() { } // Визуализация вынесена в отдельный модуль

    public void Dispose() { }
}