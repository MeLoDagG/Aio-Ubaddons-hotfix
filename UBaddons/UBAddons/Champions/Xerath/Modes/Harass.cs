using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Xerath.Modes
{
    class Harass : Xerath
    {
        public static void Execute()
        {
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Harass.UseQ && (Q.IsReady() || Q.IsCharging))
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
                {
                    if (Q.IsCharging)
                    {
                        var pred = Q.GetPrediction(target);
                        if (pred.CanNext(Q, MenuValue.General.QHitChance, true))
                        {
                            Q.Cast(pred.CastPosition);
                        }
                    }
                    else
                    {
                        Q.StartCharging();
                    }
                }
            }
            if (!Q.IsCharging)
            {
                if (MenuValue.Harass.UseW && W.IsReady())
                {
                    var target = W.GetTarget(Champ);
                    if (target != null)
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
                    if (target != null)
                    {
                        var pred = E.GetPrediction(target);
                        if (pred.CanNext(E, MenuValue.General.EHitChance, false))
                        {
                            E.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }
    }
}
