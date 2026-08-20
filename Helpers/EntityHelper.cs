using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Creeps;
using System.Collections.Generic;
using System.Linq;

namespace DivineVision.Helpers;

public static class EntityHelper
{
    /// <summary>
    /// Получить всех вражеских крипов в радиусе от героя
    /// </summary>
    public static List<Creep> GetEnemyCreepsInRange(Hero hero, float range)
    {
        return EntityManager.GetEntities<Creep>()
            .Where(c => c.IsAlive && c.IsEnemy(hero) && c.Distance(hero) < range)
            .ToList();
    }

    /// <summary>
    /// Получить всех союзных крипов в радиусе от героя (для деная)
    /// </summary>
    public static List<Creep> GetAllyCreepsInRange(Hero hero, float range)
    {
        return EntityManager.GetEntities<Creep>()
            .Where(c => c.IsAlive && c.IsAlly(hero) && c.Distance(hero) < range)
            .ToList();
    }
}