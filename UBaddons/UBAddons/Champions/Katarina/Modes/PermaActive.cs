using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Katarina.Modes
{
    class PermaActive : Katarina
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
            if (MenuValue.Misc.EKS && E.IsReady())
            {
                var Target = E.GetKillableTarget();
                if (Target != null)
                {
                    var pred = E.GetPrediction(Target);
                    E.Cast(pred.CastPosition);
                }
                var Target2 = TargetSelector.GetTarget(
                    EntityManager.Heroes.Enemies.Where(t => t != null
                        && t.IsValidTarget()
                        && !t.HasUndyingBuff(true)
                        && !t.HasBuffOfType(BuffType.SpellImmunity)
                        && t.Health <= PassiveDamage(t)), DamageType.Magical);
                if (Target != null)
                {
                    foreach (var dagger in Dagger.Keys.OrderBy(x => x.Distance(Target2)))
                    {
                        if (dagger != null)
                        {
                            E.Cast(dagger.Position);
                        }
                    }
                }
            }
            if (MenuValue.Misc.RKS && R.IsReady())
            {
                var Target = R.GetKillableTarget();
                if (Target != null)
                {
                    R.Cast();
                }
            }
        }
    }
}
