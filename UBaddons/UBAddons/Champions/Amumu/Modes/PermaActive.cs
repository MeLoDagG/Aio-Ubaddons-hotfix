using EloBuddy;
using EloBuddy.SDK;
using System.Linq;
using UBAddons.Libs;

namespace UBAddons.Champions.Amumu.Modes
{
    class PermaActive : Amumu
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
            if (MenuValue.Misc.WKS && W.IsReady() && W.ToggleState != 2)
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
                    E.Cast();
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
