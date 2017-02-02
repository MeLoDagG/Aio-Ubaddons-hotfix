using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Brand.Modes
{
    class Harass : Brand
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Harass.UseQ && Q.IsReady())
            {
                var target = Q.GetTarget(Champ);
                if (target != null && (HasPassiveBuff(target) || !MenuValue.Harass.OnlyStun))
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
                var target = W.GetTarget(Champ);
                if (target != null && (HasPassiveBuff(target) || !MenuValue.Harass.OnlyExtra))
                {
                    var pred = W.GetPrediction(target);
                    if (pred.CanNext(W, MenuValue.General.WHitChance, true))
                    {
                        W.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Harass.UseE && E.IsReady())
            {
                var target = E.GetTarget(Champ);
                if (target != null && (HasPassiveBuff(target) || !MenuValue.Harass.OnlySpread))
                {
                    E.Cast(target);
                }
            }

        }
    }
}
