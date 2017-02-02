using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Zilean.Modes
{
    internal class PermaActive : Zilean
    {
        public static void Execute()
        {
            if (MenuValue.Auto.Enable)
            {
                var Allies = EntityManager.Heroes.Allies.Where(x => !x.IsDead && x.IsValidTarget() && x.HealthPercent <= MenuValue.Auto.HP(x) && R.IsInRange(x)).OrderByDescending(x => MenuValue.Auto.ChampPriority(x));
                foreach (var Ally in Allies)
                {
                    if (MenuValue.Auto.EnableWith(Ally))
                    {
                        if ((MenuValue.Auto.EnablePred(Ally) && Prediction.Health.GetPrediction(Ally, 2000) <= 0) || !MenuValue.Auto.EnablePred(Ally) || Ally.CountEnemyChampionsInRange(1000) >= 1)
                        {
                            if (R.IsReady())
                                R.Cast(Ally);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            {
                var target = Q.GetKillableTarget();
                if (target == null) return;
                if (Q.IsReady() && MenuValue.Misc.QKS)
                {
                    var pred = Q.GetPrediction(target);
                    Q.Cast(pred.CastPosition);
                }
                if (W.IsReady() && !Q.IsReady() && MenuValue.Misc.WKS)
                {
                    W.Cast();
                }
            }
        }
    }
}
