using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Kayle.Modes
{
    class PermaActive : Kayle
    {
        public static void Execute()
        {
            if (MenuValue.Misc.QKS && Q.IsReady())
            {
                var Target = Q.GetKillableTarget();
                if (Target != null)
                {
                    Q.Cast(Target);
                }
            }
            if (MenuValue.Auto.Enable && R.IsReady())
            {
                var Allies = EntityManager.Heroes.Allies.Where(x => !x.IsDead && x.IsValidTarget() && x.HealthPercent <= MenuValue.Auto.HP(x) && R.IsInRange(x)).OrderByDescending(x => MenuValue.Auto.ChampPriority(x));
                foreach (var Ally in Allies)
                {
                    if (MenuValue.Auto.EnableWith(Ally))
                    {
                        if (Ally.CountEnemyChampionsInRange(1000) >= 1 || Prediction.Health.GetPrediction(Ally, 1300) <= 1)
                        {
                            if (R.IsReady())
                            {
                                R.Cast(Ally);
                            }
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
            if (MenuValue.General.Enable && W.IsReady())
            {
                var Allies = EntityManager.Heroes.Allies.Where(x => x.IsValidTarget() && x.HealthPercent <= MenuValue.General.HP && W.IsInRange(x)).OrderByDescending(x => MenuValue.Auto.ChampPriority(x));
                foreach (var Ally in Allies)
                {
                    if (MenuValue.General.EnableWith(Ally))
                    {
                        if (W.IsReady())
                        {
                            W.Cast(Ally);
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
        }
    }
}
