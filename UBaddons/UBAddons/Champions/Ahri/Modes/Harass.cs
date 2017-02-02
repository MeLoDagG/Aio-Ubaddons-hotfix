using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Ahri.Modes
{
    class Harass : Ahri
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.Harass.ManaLimit) return;
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health <= HandleDamageIndicator(x));
            if (MenuValue.Harass.UseQ)
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
                var target = W.GetTarget(-50);
                if (target != null)
                {
                    W.Cast();
                }
            }
        }
    }
}
