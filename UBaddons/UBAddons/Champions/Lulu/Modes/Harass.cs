using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Lulu.Modes
{
    class Harass : Lulu
    {
        public static void Execute()
        {
            if (player.ManaPercent < MenuValue.Harass.ManaLimit) return;
            var Champ = EntityManager.Heroes.Enemies.Where(x => x.Health < HandleDamageIndicator(x));
            if (MenuValue.Harass.UseQ && Q.IsReady())
            {
                var target = Q.GetTarget(Champ);
                var target2 = QPix.GetTarget(Champ);
                if (target != null || target2 != null)
                {
                    PredictionResult pred = null;
                    PredictionResult pred2 = null;
                    if (target != null)
                    {
                        pred = Q.GetPrediction(target);
                    }
                    if (target2 != null)
                    {
                        pred2 = QPix.GetPrediction(target2);
                    }
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false) || pred2.CanNext(QPix, MenuValue.General.QHitChance, false))
                    {
                        if (pred.HitChance > pred2.HitChance)
                        {
                            Q.Cast(pred.CastPosition);
                        }
                        else
                        {
                            QPix.Cast(pred2.CastPosition);
                        }

                    }
                }
            }
            if (MenuValue.Harass.UseE && E.IsReady())
            {
                var target = Q.GetTarget(Champ);
                if (target != null)
                {
                    E.Cast(target);
                }
            }
        }
    }
}
