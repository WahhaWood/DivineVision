using System;
using System.Drawing;
using Divine;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Menu;
using Divine.Menu.Items;
using Divine.Renderer;

namespace DivineVision.Modules;

public class VisualModule : IModule
{
    private readonly MenuSwitcher _enabled;
    private readonly MenuSwitcher _showLastHitIndicators;
    private readonly LastHitModule _lastHitModule; // ссылка на модуль ластхита

    public bool Enabled { get; set; }

    public VisualModule(Menu mainMenu)
    {
        var menu = mainMenu.AddMenu("Visuals");
        _enabled = menu.AddSwitcher("Enabled", true);
        _showLastHitIndicators = menu.AddSwitcher("Show Last Hit Indicators", true);

        // Получаем ссылку на модуль ластхита (он должен быть зарегистрирован раньше)
        // Но в текущей архитектуре модули не знают друг о друге.
        // Чтобы это работало, нужно передать экземпляр LastHitModule в конструктор VisualModule.
        // Проще: сделаем статическое свойство или глобальный доступ.
        // Но для простоты пока оставим как есть, а позже переделаем.
        // Здесь мы просто объявим свойство, которое будем устанавливать из PluginManager.
        // Пока оставим заглушку.
    }

    // Временное решение: публичное свойство для установки ссылки
    public LastHitModule? LastHitModuleRef { get; set; }

    public void OnUpdate()
    {
        // Никакой логики в update не нужно, только отрисовка
    }

    public void OnDraw()
    {
        if (!_enabled || !_showLastHitIndicators || LastHitModuleRef is null)
            return;

        var target = LastHitModuleRef.CurrentTarget;
        if (target is null || !target.IsAlive)
            return;

        // Рисуем зелёный круг над целью
        var pos = target.Position;
        RendererManager.DrawCircle(pos, 50, Color.LimeGreen, 3);

        // Рисуем текст "LAST HIT"
        var screenPos = RendererManager.WorldToScreen(pos);
        if (screenPos.HasValue)
        {
            RendererManager.DrawText(
                "🔪 LAST HIT",
                screenPos.Value.X - 40,
                screenPos.Value.Y - 60,
                Color.LimeGreen,
                FontFlags.Outline
            );
        }
    }

    public void Dispose() { }
}