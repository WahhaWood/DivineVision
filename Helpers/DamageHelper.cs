namespace DivineVision.Helpers;

using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Buildings;
using Divine.Entity.Entities.Units.Creeps;

public static class DamageHelper
{
    public static float CalculateRealDamageToEntity(Hero hero, Entity entity)
    {
        float armor = 0;
        if (entity is Creep c)
            armor = c.Armor;
        else if (entity is Tower t)
            armor = t.Armor;
        else
            return hero.TotalDamage; // упрощённо

        var armorReduction = armor / (armor + 100);
        return hero.TotalDamage * (1 - armorReduction);
    }

    public static float CalculateRealDamage(Hero hero, Creep creep)
    {
        return CalculateRealDamageToEntity(hero, creep);
    }

    public static float HealthAfterHit(Hero hero, Creep creep)
    {
        return creep.Health - CalculateRealDamageToEntity(hero, creep);
    }
}
