using UBMiddle.Libs;

namespace UBMiddle.Champions.Camille.Modes
{
    class PermaActive : Camille
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
            if (MenuValue.Misc.WKS && W.IsReady())
            {
                var Target = W.GetKillableTarget();
                if (Target != null)
                {
                    W.Cast();
                }
            }
            if (MenuValue.Misc.EKS && E.IsReady())
            {
                var Target = E.GetKillableTarget();
                if (Target != null)
                {
                    var pred = E.GetPrediction(Target);
                    if (pred.CanNext(E, MenuValue.General.EHitChance, true))
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
