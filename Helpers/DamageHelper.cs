namespace DivineVision.Helpers;

public static class DamageHelper
{
    /// <summary>
    /// Рассчитывает реальный урон по цели с учётом брони
    /// </summary>
    public static float CalculatePhysicalDamage(float damage, float armor)
    {
        var reduction = armor / (armor + 100);
        return damage * (1 - reduction);
    }
}