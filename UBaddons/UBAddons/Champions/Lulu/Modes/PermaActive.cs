using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Lulu.Modes
{
    class PermaActive : Lulu
    {
        public static void Execute()
        {
            if (MenuValue.Misc.QKS && Q.IsReady())
            {
                var Target = Q.GetKillableTarget();
                var Target2 = QPix.GetKillableTarget();
                if (Target != null && Target2 != null)
                {
                    var pred = Q.GetPrediction(Target);
                    var pred2 = QPix.GetPrediction(Target2);
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
            if (MenuValue.Auto.Enable)
            {
                var Allies = EntityManager.Heroes.Allies.Where(x => !x.IsDead && x.IsValidTarget() && MenuValue.Auto.EnableWith(x) && x.HealthPercent <= MenuValue.Auto.HP(x) && R.IsInRange(x)).OrderByDescending(x => MenuValue.Auto.ChampPriority(x));
                foreach (var Ally in Allies)
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
            }
        }
    }
}
