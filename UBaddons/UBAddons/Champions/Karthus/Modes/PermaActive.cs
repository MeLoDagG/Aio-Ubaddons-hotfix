using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Karthus.Modes
{
    class PermaActive : Karthus
    {
        public static void Execute()
        {
            if (MenuValue.Misc.QKS && Q.IsReady())
            {
                var Target = Q.GetKillableTarget();
                if (Target != null)
                {
                    var pred = Q.GetPrediction(Target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, true))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Misc.EKS && E.IsReady() && E.ToggleState != 2)
            {
                var target = E.GetKillableTarget();
                if (target != null)
                {
                    E.Cast();
                }
            }
            if (MenuValue.Misc.RKS && R.IsReady() && (player.CountEnemyChampionsInRange(1000) < float.Epsilon || player.HasBuff("")))
            {
                var target = R.GetKillableTarget();
                if (target != null)
                {
                    R.Cast();
                }
            }
        }
    }
}
