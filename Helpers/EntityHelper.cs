namespace DivineVision.Helpers;

using System.Collections.Generic;
using System.Linq;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Extensions;

public static class EntityHelper
{
    public static IEnumerable<Creep> GetEnemyCreepsInRange(Hero hero, float range)
    {
        return EntityManager.GetEntities<Creep>()
            .Where(c => c.IsAlive && c.IsEnemy(hero) && c.Distance(hero) < range);
    }

    public static IEnumerable<Creep> GetAllyCreepsInRange(Hero hero, float range)
    {
        return EntityManager.GetEntities<Creep>()
            .Where(c => c.IsAlive && c.IsAlly(hero) && c.Distance(hero) < range);
    }

    public static bool CanLastHit(Creep creep, Hero hero, float bonusDamage = 0)
    {
        var damage = hero.TotalDamage + bonusDamage;
        var armorReduction = creep.Armor / (creep.Armor + 100);
        var realDamage = damage * (1 - armorReduction);
        return creep.Health < realDamage + 10;
    }
}
