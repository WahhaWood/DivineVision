namespace DivineVision.Modules;

using System;
using System.Collections.Generic;
using System.Linq;
using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Buildings;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Extensions;
using Divine.Input;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.Orbwalker;
using DivineVision.Core;
using DivineVision.Helpers;
using Divine.Logger;

public class LastHitModule : IModule
{
    private readonly MenuHoldKey _lastHitKey;
    private readonly MenuSlider _range;
    private readonly MenuSwitcher _includeTowers;
    private readonly MenuSwitcher _debugMode;
    private readonly VisualModule _visual;
    private readonly PluginSettings _settings;

    private Hero? _localHero;
    private Creep? _bestCreep;
    private Tower? _bestTower;
    private bool _lastHitActive;

    public bool IsActive => _lastHitKey.Value;

    public LastHitModule(Menu mainMenu, VisualModule visual, PluginSettings settings)
    {
        _visual = visual;
        _settings = settings;

        var menu = mainMenu.AddMenu("Last Hit");
        _lastHitKey = menu.AddHoldKey("Activation Key", Key.Space);
        _range = menu.AddSlider("Range", 1000, 400, 2500);
        _includeTowers = menu.AddSwitcher("Include Towers", true);
        _debugMode = menu.AddSwitcher("Debug Mode", false);

        // Привязываем к глобальному дебагу (если в настройках включен)
        _debugMode.ValueChanged += (s, e) => { _settings.DebugMode.Value = e.Value; };
    }

    public void OnUpdate()
    {
        _lastHitActive = _lastHitKey.Value;
        _visual.SetLastHitActive(_lastHitActive);

        if (!_lastHitActive)
            return;

        _localHero = EntityManager.LocalHero;
        if (_localHero is null || !_localHero.IsAlive || !_localHero.CanAttack)
            return;

        var range = _range.Value;

        // Поиск крипов
        var enemyCreeps = EntityHelper.GetEnemyCreepsInRange(_localHero, range).ToList();
        var allyCreeps = EntityHelper.GetAllyCreepsInRange(_localHero, range).ToList();

        // Поиск башен (вражеских)
        var towers = _includeTowers.Value
            ? EntityHelper.GetEnemyTowersInRange(_localHero, range).ToList()
            : new List<Tower>();

        // Выбираем лучшую цель среди крипов и башен
        _bestCreep = FindBestCreep(enemyCreeps);
        _bestTower = FindBestTower(towers);

        // Приоритет: если есть крип для добивания — бьём его, иначе башню
        var targetEntity = (Entity)_bestCreep ?? _bestTower;

        if (targetEntity is not null)
        {
            _visual.UpdateTargets(_bestCreep, null); // для деная пока отдельно не делаем
            OrbwalkerManager.Attack(targetEntity);

            // Дебаг-вывод
            if (_debugMode.Value || _settings.DebugMode.Value)
            {
                var damage = _localHero.TotalDamage;
                var armor = targetEntity is Creep c ? c.Armor : (_bestTower?.Armor ?? 0);
                var realDamage = DamageHelper.CalculateRealDamageToEntity(_localHero, targetEntity);
                Logger.Log($"[LH] Цель: {targetEntity.Name} | HP: {targetEntity.Health:F0} | Урон: {realDamage:F0} | Броня: {armor:F1} | Дист: {_localHero.Distance(targetEntity):F0}");
            }
        }
        else
        {
            // Если нет целей — очищаем визуалку
            _visual.UpdateTargets(null, null);
        }

        // Денай (союзные крипы) — пока пропустим для упрощения
    }

    private Creep? FindBestCreep(IEnumerable<Creep> creeps)
    {
        if (_localHero is null) return null;
        return creeps
            .Where(c => EntityHelper.CanLastHit(c, _localHero))
            .OrderBy(c => c.Health)
            .FirstOrDefault();
    }

    private Tower? FindBestTower(IEnumerable<Tower> towers)
    {
        if (_localHero is null) return null;
        return towers
            .Where(t => EntityHelper.CanLastHitTower(t, _localHero))
            .OrderBy(t => t.Health)
            .FirstOrDefault();
    }

    public void Dispose() { }
}
