using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Malzahar.Modes
{
    class Harass : Malzahar
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
                    var pred = Q.GetPrediction(target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Harass.UseW && W.IsReady())
            {
                var target = W.GetTarget(250);
                if (target != null)
                {
                    CastW(target);
                }
            }
            if (MenuValue.Harass.UseE && E.IsReady())
            {
                var target = E.GetTarget(Champ);
                if (target != null)
                {
                    E.Cast(target);
                }
            }
        }
    }
}
