using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Viktor.Modes
{
    class Harass : Viktor
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.Harass.ManaLimit) return;
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Harass.UseQ && Q.IsReady())
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
                {
                    Q.Cast(target);
                }
            }
            if (MenuValue.Harass.UseW && W.IsReady())
            {
                W.CastIfItWillHit(MenuValue.Harass.Whit, MenuValue.General.WHitChance);
            }
            if (MenuValue.Harass.UseE && E.IsReady())
            {
                var target = E.GetTarget(Champ);
                if (target != null)
                {
                    CastE(target);
                }
            }
        }
    }
}
