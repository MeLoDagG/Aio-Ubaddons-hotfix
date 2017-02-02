using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Orianna.Modes
{
    class PermaActive : Orianna
    {
        public static void Execute()
        {
            if (MenuValue.Misc.QKS && Q.IsReady())
            {
                var target = Q.GetKillableTarget();
                if (target != null)
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.CanNext(Q, MenuValue.General.QHitChance, false))
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
            }
            if (MenuValue.Misc.WKS && W.IsReady())
            {
                var target = W.GetKillableTarget();
                if (target != null)
                {
                    W.Cast();
                }
            }
            if (MenuValue.Misc.EKS && E.IsReady())
            {
                var target = E.GetKillableTarget();
                if (target != null)
                {
                    CastE(target);
                }
            }
            if (MenuValue.Misc.RKS && R.IsReady())
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
