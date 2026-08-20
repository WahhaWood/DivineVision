namespace DivineVision.Modules;

using System;
using System.Drawing;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Helpers;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.Renderer;
using DivineVision.Helpers;

public class VisualModule : IModule
{
    private readonly MenuSwitcher _enabled;
    private readonly MenuSwitcher _showLastHitMarker;
    private readonly MenuSwitcher _showDenyMarker;
    private readonly MenuSwitcher _showDamageText;

    private Creep? _lastHitTarget;
    private Creep? _denyTarget;

    public bool Enabled { get; set; }

    public VisualModule(Menu mainMenu)
    {
        var menu = mainMenu.AddMenu("Visuals");
        _enabled = menu.AddSwitcher("Enabled", true);
        _showLastHitMarker = menu.AddSwitcher("Show Last Hit Marker", true);
        _showDenyMarker = menu.AddSwitcher("Show Deny Marker", true);
        _showDamageText = menu.AddSwitcher("Show Damage Text", true);

        Enabled = true;
    }

    public void UpdateTargets(Creep? lastHit, Creep? deny)
    {
        _lastHitTarget = lastHit;
        _denyTarget = deny;
    }

    public void OnUpdate()
    {
        // Визуальный модуль не требует обновления логики, только отрисовка
    }

    public void OnDraw()
    {
        if (!_enabled || _lastHitTarget is null && _denyTarget is null)
            return;

        // Рисуем маркер для ластхита
        if (_showLastHitMarker && _lastHitTarget is not null)
        {
            DrawLastHitMarker(_lastHitTarget);
        }

        // Рисуем маркер для деная
        if (_showDenyMarker && _denyTarget is not null)
        {
            DrawDenyMarker(_denyTarget);
        }

        // Отображаем урон над крипами
        if (_showDamageText)
        {
            if (_lastHitTarget is not null)
                DrawDamageText(_lastHitTarget, Color.LimeGreen);
            if (_denyTarget is not null)
                DrawDamageText(_denyTarget, Color.Red);
        }
    }

    private void DrawLastHitMarker(Creep creep)
    {
        var pos = creep.Position;
        RendererManager.DrawCircle(pos, 60, Color.LimeGreen, 3);

        // Вращающийся эффект (имитация через несколько кругов)
        var time = Environment.TickCount / 1000f;
        var offset = 30 * Math.Sin(time * 2);
        RendererManager.DrawCircle(pos, 70 + (float)offset, Color.Lime, 1);
    }

    private void DrawDenyMarker(Creep creep)
    {
        var pos = creep.Position;
        RendererManager.DrawCircle(pos, 60, Color.Red, 3);

        // Перекрёстие сверху
        var screenPos = RendererManager.WorldToScreen(pos);
        if (screenPos is null) return;

        var x = screenPos.Value.X;
        var y = screenPos.Value.Y - 80;

        RendererManager.DrawLine(
            new Vector2(x - 15, y - 15),
            new Vector2(x + 15, y + 15),
            Color.Red, 2
        );
        RendererManager.DrawLine(
            new Vector2(x + 15, y - 15),
            new Vector2(x - 15, y + 15),
            Color.Red, 2
        );
    }

    private void DrawDamageText(Creep creep, Color color)
    {
        var screenPos = RendererManager.WorldToScreen(creep.Position);
        if (screenPos is null) return;

        var hero = EntityManager.LocalHero;
        if (hero is null) return;

        var damage = DamageHelper.CalculateRealDamage(hero, creep);
        var text = $"⚔ {damage:F0}";

        RendererManager.DrawText(
            text,
            screenPos.Value.X - 15,
            screenPos.Value.Y - 100,
            color,
            FontFlags.Outline
        );
    }

    public void Dispose() { }
}
