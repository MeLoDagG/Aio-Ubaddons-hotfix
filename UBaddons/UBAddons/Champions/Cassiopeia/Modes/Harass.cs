using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Cassiopeia.Modes
{
    class Harass : Cassiopeia
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.Harass.ManaLimit) return;
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Harass.UseQ)
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, true))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Harass.UseE && E.IsReady())
            {
                var poisoned = Champ.Where(x => x.HasBuffOfType(BuffType.Poison));
                var target = E.GetTarget(poisoned);
                if (target != null)
                {
                    if (target.HasBuffOfType(BuffType.Poison) || !MenuValue.Combo.PoisonOnly)
                    {
                        E.Cast(target);
                    }
                }
            }    
        }
    }
}
