using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Kayle.Modes
{
    class Harass : Kayle
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
                var target = E.GetTarget();
                if (target != null)
                {
                    E.Cast();
                }
            }
        }
    }
}
