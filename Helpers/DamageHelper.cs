namespace DivineVision.Helpers;

using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Creeps;

public static class DamageHelper
{
    /// <summary>
    /// Рассчитывает реальный урон по крипу с учётом брони и резистов
    /// </summary>
    public static float CalculateRealDamage(Hero hero, Creep creep)
    {
        var damage = hero.TotalDamage;
        var armor = creep.Armor;
        var armorReduction = armor / (armor + 100);
        return damage * (1 - armorReduction);
    }

    /// <summary>
    /// Рассчитывает, сколько HP останется у крипа после нашего удара
    /// </summary>
    public static float HealthAfterHit(Hero hero, Creep creep)
    {
        return creep.Health - CalculateRealDamage(hero, creep);
    }
}
