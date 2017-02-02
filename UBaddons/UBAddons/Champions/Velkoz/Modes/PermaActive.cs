using EloBuddy.SDK;
using UBAddons.Libs;

namespace UBAddons.Champions.Velkoz.Modes
{
    class PermaActive : Velkoz
    {
        public static void Execute()
        {
            if (MenuValue.Misc.QKS && Q.IsReady() && !(Q.ToggleState == 2 || Q.Name.Equals("VelkozQSplitActivate")) && Core.GameTickCount - LastQTick > 120)
            {
                var Target = Q.GetKillableTarget();
                if (Target != null)
                {
                    QCast(Target);
                    LastQTick = Core.GameTickCount;
                }
            }
            if (MenuValue.Misc.WKS && W.IsReady())
            {
                var Target = W.GetKillableTarget();
                if (Target != null)
                {
                    var pred = W.GetPrediction(Target);
                    if (pred.CanNext(W, MenuValue.General.WHitChance, false))
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
                    var pred = E.GetPrediction(Target);
                    if (pred.CanNext(E, MenuValue.General.EHitChance, false))
                    {
                        E.Cast(pred.CastPosition);
                    }
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
