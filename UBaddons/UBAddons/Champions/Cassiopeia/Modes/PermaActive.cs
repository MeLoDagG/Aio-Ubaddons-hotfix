using EloBuddy.SDK;
using UBAddons.Libs;

namespace UBAddons.Champions.Cassiopeia.Modes
{
    class PermaActive : Cassiopeia
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
            if (MenuValue.Misc.WKS && W.IsReady())
            {
                var Target = W.GetKillableTarget();
                if (Target != null)
                {
                    var pred = W.GetPrediction(Target);
                    if (pred.CanNext(W, MenuValue.General.WHitChance, false) && !pred.CastPosition.IsInRange(player, WMinRange))
                    {
                        W.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Misc.EKS && E.IsReady())
            {
                var Target = E.GetKillableTarget();
                if (Target != null)
                {
                    E.Cast(Target);
                }
            }
            if (MenuValue.Misc.RKS && R.IsReady())
            {
                var Target = R.GetKillableTarget();
                if (Target != null)
                {
                    var pred = R.GetPrediction(Target);
                    if (pred.CanNext(R, MenuValue.General.RHitChance, true))
                    {
                        R.Cast(pred.CastPosition);
                    }
                }
            }
        }
    }
}
