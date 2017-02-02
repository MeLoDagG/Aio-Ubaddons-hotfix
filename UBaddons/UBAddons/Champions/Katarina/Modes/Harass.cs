using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Katarina.Modes
{
    class Harass : Katarina
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Harass.UseQ && Q.IsReady())
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
                {
                    Q.Cast(target);
                }
            }
            if (MenuValue.Harass.UseE && E.IsReady())
            {
                var dagger = Dagger.Where(x => x.Value.Item1 && !x.Key.Position.IsUnderEnemyTurret()).OrderBy(x => x.Key.Distance(TargetSelector.GetTarget(500, DamageType.Magical, x.Key.Position, true)));
                if (dagger.Any())
                {
                    if (MenuValue.Harass.UseW && W.IsReady())
                    {
                        if (W.Cast())
                        {
                            E.Cast(dagger.FirstOrDefault().Key.Position);
                        }
                    }
                }
            }
        }
    }
}
