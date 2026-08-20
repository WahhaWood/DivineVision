namespace Divine.ProjectTemplate;

using System;

using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Game;
using Divine.Input;
using Divine.Menu;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Particle;
using Divine.Service;
using Divine.Update;

internal sealed class Bootstrap : Bootstrapper
{
    private MenuSwitcher EnableSwitcher = null!;

    private MenuHoldKey ComboHoldKey = null!;

    protected override void OnMainActivate()
    {
        Console.WriteLine("OnMainActivate: Hello World");

        var menu = MenuManager.AddMenu("Divine.ProjectTemplate");
        EnableSwitcher = menu.AddSwitcher("Enable");
        ComboHoldKey = menu.AddHoldKey("Combo", Key.D);
    }

    protected override void OnMainDeactivate()
    {
        Console.WriteLine("OnMainDeactivate: Hello World");
    }

    protected override void OnActivate()
    {
        Console.WriteLine("OnActivate: Hello World");

        EnableSwitcher.ValueChanged += OnEnableChanged;
    }

    protected override void OnDeactivate()
    {
        Console.WriteLine("OnDeactivate: Hello World");

        EnableSwitcher.ValueChanged -= OnEnableChanged;
    }

    private void OnEnableChanged(MenuSwitcher sender, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            ComboHoldKey.ValueChanged += OnComboChanged;
            UpdateManager.CreateIngameUpdate(100, OnIngameUpdate);
        }
        else
        {
            ComboHoldKey.ValueChanged -= OnComboChanged;
            UpdateManager.DestroyIngameUpdate(OnIngameUpdate);
        }
    }

    private void OnComboChanged(MenuHoldKey sender, HoldKeyChangedEventArgs e)
    {
        Console.WriteLine($"OnComboChanged: {e.Value}");

        if (!e.Value)
        {
            ParticleManager.DestroyParticle("Divine.ProjectTemplate.TargetLine");
            ParticleManager.DestroyParticle("Divine.ProjectTemplate.TargetDistance");
        }
    }

    private void OnIngameUpdate()
    {
        if (!ComboHoldKey)
        {
            return;
        }

        var localHero = EntityManager.LocalHero;
        if (localHero is null)
        {
            return;
        }

        var mousePosition = GameManager.MousePosition;
        var targetDistance = 250;

        var target = EntityManager.GetEntities<Unit>()
            .Where(x => !x.IsAlly(localHero) && x.Distance(mousePosition) < targetDistance)
            .OrderBy(x => x.Distance(mousePosition))
            .FirstOrDefault();

        if (target is not null)
        {
            ParticleManager.CreateTargetLineParticle("Divine.ProjectTemplate.TargetLine", localHero, target.Position, Color.Yellow);
            localHero.Attack(target);
        }
        else
        {
            ParticleManager.DestroyParticle("Divine.ProjectTemplate.TargetLine");
            localHero.Move(GameManager.MousePosition);
        }

        ParticleManager.CreateCircleParticle("Divine.ProjectTemplate.TargetDistance", mousePosition, targetDistance, Color.Red);
    }
}
