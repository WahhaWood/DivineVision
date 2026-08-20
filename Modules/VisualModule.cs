namespace DivineVision.Modules;

using System;
using System.Drawing;
using Divine.Entity;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Helpers;
using Divine.Input;
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
    private readonly MenuSwitcher _showRangeOnLastHit;
    private readonly MenuHoldKey _showRangeKey;
    private readonly MenuSwitcher _alwaysShowRange;

    private Creep? _lastHitTarget;
    private Creep? _denyTarget;
    private bool _lastHitActive;

    public bool Enabled { get; set; }

    public VisualModule(Menu mainMenu)
    {
        var menu = mainMenu.AddMenu("Visuals");
        _enabled = menu.AddSwitcher("Enabled", true);
        _showLastHitMarker = menu.AddSwitcher("Show Last Hit Marker", true);
        _showDenyMarker = menu.AddSwitcher("Show Deny Marker", true);
        _showDamageText = menu.AddSwitcher("Show Damage Text", true);

        var rangeMenu = menu.AddMenu("Attack Range");
        _showRangeOnLastHit = rangeMenu.AddSwitcher("Show Range on Last Hit", true);
        _showRangeKey = rangeMenu.AddHoldKey("Show Range Key (optional)", Key.X);
        _alwaysShowRange = rangeMenu.AddSwitcher("Always Show", false);

        Enabled = true;
    }

    public void SetLastHitActive(bool active)
    {
        _lastHitActive = active;
    }

    public void UpdateTargets(Creep? lastHit, Creep? deny)
    {
        _lastHitTarget = lastHit;
        _denyTarget = deny;
    }

    public void OnUpdate() { }

    public void OnDraw()
    {
        if (!_enabled) return;

        if (_showLastHitMarker && _lastHitTarget is not null)
            DrawLastHitMarker(_lastHitTarget);

        if (_showDenyMarker && _denyTarget is not null)
            DrawDenyMarker(_denyTarget);

        if (_showDamageText)
        {
            if (_lastHitTarget is not null)
                DrawDamageText(_lastHitTarget, Color.LimeGreen);
            if (_denyTarget is not null)
                DrawDamageText(_denyTarget, Color.Red);
        }

        DrawAttackRange();
    }

    private void DrawAttackRange()
    {
        bool show = false;

        if (_alwaysShowRange.Value)
            show = true;
        if (_showRangeKey.Value)
            show = true;
        if (_showRangeOnLastHit.Value && _lastHitActive)
            show = true;

        if (!show) return;

        var hero = EntityManager.LocalHero;
        if (hero is null || !hero.IsAlive) return;

        var range = hero.AttackRange;
        var pos = hero.Position;

        RendererManager.DrawCircle(pos, range, Color.FromArgb(60, 0, 255, 0));
        RendererManager.DrawCircle(pos, range, Color.Lime, 2);

        var screenPos = RendererManager.WorldToScreen(pos);
        if (screenPos is not null)
        {
            RendererManager.DrawText(
                $"Range: {range:F0}",
                screenPos.Value.X - 30,
                screenPos.Value.Y - range - 20,
                Color.White,
                FontFlags.Outline
            );
        }
    }

    private void DrawLastHitMarker(Creep creep)
    {
        var pos = creep.Position;
        RendererManager.DrawCircle(pos, 60, Color.LimeGreen, 3);

        var time = Environment.TickCount / 1000f;
        var offset = 30 * Math.Sin(time * 2);
        RendererManager.DrawCircle(pos, 70 + (float)offset, Color.Lime, 1);
    }

    private void DrawDenyMarker(Creep creep)
    {
        var pos = creep.Position;
        RendererManager.DrawCircle(pos, 60, Color.Red, 3);

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
